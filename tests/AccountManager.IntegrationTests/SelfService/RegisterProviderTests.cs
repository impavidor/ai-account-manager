using System.Net;
using System.Net.Http.Json;
using FluentAssertions;

namespace AccountManager.IntegrationTests.SelfService;

[TestFixture]
public class RegisterProviderTests : IntegrationTestBase
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
        var request = new HttpRequestMessage(HttpMethod.Post, "/self-service/providers")
        {
            Content = JsonContent.Create(new { FirstName = "John", LastName = "Doe", Npi = "1234567890" })
        };
        request.WithActor(_actorId, "Provider");

        var response = await Client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var body = await response.Content.ReadFromJsonAsync<CreatedResponse>();
        body.Should().NotBeNull();
        body!.Id.Should().NotBeEmpty();

        var dataStore = new DataStore(DataPath);
        var contacts = dataStore.ReadContacts();
        contacts.Should().Contain(c =>
            c.Id == body.Id &&
            c.Type == DataStore.ContactTypeProvider &&
            c.FirstName == "John" &&
            c.LastName == "Doe" &&
            c.Npi == "1234567890");
    }

    private record CreatedResponse(Guid Id);
}
