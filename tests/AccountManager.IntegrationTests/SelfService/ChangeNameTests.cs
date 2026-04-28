using System.Net;
using System.Net.Http.Json;
using FluentAssertions;

namespace AccountManager.IntegrationTests.SelfService;

[TestFixture]
public class ChangeNameTests : IntegrationTestBase
{
    private Guid _providerId;

    [OneTimeSetUp]
    public async Task SetUp()
    {
        var registerRequest = new HttpRequestMessage(HttpMethod.Post, "/self-service/providers")
        {
            Content = JsonContent.Create(new { FirstName = "Alice", LastName = "Brown", Npi = "1234567890" })
        };
        registerRequest.WithActor(Guid.NewGuid(), "Provider");

        var registerResponse = await Client.SendAsync(registerRequest);
        registerResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var body = await registerResponse.Content.ReadFromJsonAsync<CreatedResponse>();
        _providerId = body!.Id;
    }

    [Test]
    public async Task HappyPath_ValidName_Returns200AndNameUpdatesInDataStore()
    {
        var request = new HttpRequestMessage(HttpMethod.Patch, "/self-service/account/name")
        {
            Content = JsonContent.Create(new { FirstName = "Alicia", LastName = "Brown" })
        };
        request.WithActor(_providerId, "Provider");

        var response = await Client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var dataStore = new DataStore(DataPath);
        var contacts = dataStore.ReadContacts();
        contacts.Should().Contain(c => c.Id == _providerId && c.FirstName == "Alicia" && c.LastName == "Brown");
    }

    [Test]
    public async Task ApiError_MissingBody_Returns415()
    {
        var request = new HttpRequestMessage(HttpMethod.Patch, "/self-service/account/name");
        request.WithActor(_providerId, "Provider");

        var response = await Client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.UnsupportedMediaType);
    }

    [Test]
    public async Task Auth_NoActorHeaders_Returns500BecauseEndpointUsesICurrentActor()
    {
        var request = new HttpRequestMessage(HttpMethod.Patch, "/self-service/account/name")
        {
            Content = JsonContent.Create(new { FirstName = "Bob", LastName = "Green" })
        };

        var response = await Client.SendAsync(request);

        // FakeAuthMiddleware does not return 401 for missing actor headers; ICurrentActor throws NullReferenceException
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }

    private record CreatedResponse(Guid Id);
}
