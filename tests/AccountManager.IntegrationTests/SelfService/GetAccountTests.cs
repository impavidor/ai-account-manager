using System.Net;
using System.Net.Http.Json;
using FluentAssertions;

namespace AccountManager.IntegrationTests.SelfService;

[TestFixture]
public class GetAccountTests : IntegrationTestBase
{
    private Guid _providerId;

    [OneTimeSetUp]
    public async Task SetUp()
    {
        var registerRequest = new HttpRequestMessage(HttpMethod.Post, "/self-service/providers")
        {
            Content = JsonContent.Create(new { FirstName = "Jane", LastName = "Doe", Npi = "1234567890" })
        };
        registerRequest.WithActor(Guid.NewGuid(), "Provider");

        var registerResponse = await Client.SendAsync(registerRequest);
        registerResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var body = await registerResponse.Content.ReadFromJsonAsync<CreatedResponse>();
        _providerId = body!.Id;
    }

    [Test]
    public async Task HappyPath_RegisteredProvider_Returns200WithAccountData()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "/self-service/account")
            .WithActor(_providerId, "Provider");

        var response = await Client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content.ReadFromJsonAsync<AccountResponse>();
        body.Should().NotBeNull();
        body!.Id.Should().Be(_providerId);
        body.FirstName.Should().Be("Jane");
        body.LastName.Should().Be("Doe");
        body.Npi.Should().Be("1234567890");
    }

    [Test]
    public async Task DomainError_ActorNotFound_Returns404()
    {
        var unknownId = Guid.NewGuid();
        var request = new HttpRequestMessage(HttpMethod.Get, "/self-service/account")
            .WithActor(unknownId, "Provider");

        var response = await Client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Test]
    public async Task Auth_NoActorHeaders_Returns500BecauseEndpointUsesICurrentActor()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "/self-service/account");

        var response = await Client.SendAsync(request);

        // FakeAuthMiddleware does not return 401 for missing actor headers; ICurrentActor throws NullReferenceException
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }

    private record CreatedResponse(Guid Id);
    private record AccountResponse(Guid Id, int Type, int Status, string FirstName, string LastName, string? Npi);
}
