using System.Net;
using System.Net.Http.Json;
using FluentAssertions;

namespace AccountManager.IntegrationTests.SelfService;

[TestFixture]
public class RegisterProviderAdminTests : IntegrationTestBase
{
    private Guid _actorId;

    [OneTimeSetUp]
    public void SetUp()
    {
        _actorId = Guid.NewGuid();
    }

    [Test]
    public async Task HappyPath_ValidBody_Returns201AndContactAppearsInJsonFile()
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "/self-service/provider-admins")
        {
            Content = JsonContent.Create(new { FirstName = "Alice", LastName = "Brown" })
        };
        request.WithActor(_actorId, "ProviderAdmin");

        var response = await Client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var body = await response.Content.ReadFromJsonAsync<CreatedResponse>();
        body.Should().NotBeNull();
        body!.Id.Should().NotBeEmpty();

        var dataStore = new DataStore(DataPath);
        var contacts = dataStore.ReadContacts();
        contacts.Should().Contain(c =>
            c.Id == body.Id &&
            c.Type == DataStore.ContactTypeProviderAdmin &&
            c.FirstName == "Alice" &&
            c.LastName == "Brown");
    }

    [Test]
    public async Task DomainError_InvalidName_Returns422()
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "/self-service/provider-admins")
        {
            Content = JsonContent.Create(new { FirstName = "", LastName = "Brown" })
        };
        request.WithActor(_actorId, "ProviderAdmin");

        var response = await Client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Test]
    public async Task ApiError_MissingBody_Returns415()
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "/self-service/provider-admins");
        request.WithActor(_actorId, "ProviderAdmin");

        var response = await Client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.UnsupportedMediaType);
    }

    [Test]
    public async Task Auth_NoActorHeaders_Returns201BecauseEndpointIsUnprotected()
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "/self-service/provider-admins")
        {
            Content = JsonContent.Create(new { FirstName = "Bob", LastName = "Green" })
        };

        var response = await Client.SendAsync(request);

        // FakeAuthMiddleware does not enforce authentication; registration endpoints do not use ICurrentActor
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    private record CreatedResponse(Guid Id);
}
