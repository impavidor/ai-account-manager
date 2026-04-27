using System.Net;
using System.Net.Http.Json;
using FluentAssertions;

namespace AccountManager.IntegrationTests.Administration;

[TestFixture]
public class GetContactTests : IntegrationTestBase
{
    private Guid _contactId;
    private Guid _actorId;

    [OneTimeSetUp]
    public void SetUp()
    {
        _contactId = Guid.NewGuid();
        _actorId = Guid.NewGuid();

        var dataStore = new DataStore(DataPath);
        dataStore.WriteContacts(
        [
            new ContactRecord(
                Id: _contactId,
                Type: DataStore.ContactTypeSystemAdmin,
                Status: DataStore.ContactStatusActive,
                FirstName: "Alice",
                LastName: "Smith",
                OrgName: null,
                Npi: null)
        ]);
    }

    [Test]
    public async Task HappyPath_SeededContact_Returns200()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"/administration/contacts/{_contactId}")
            .WithActor(_actorId, "SystemAdmin");

        var response = await Client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content.ReadFromJsonAsync<ContactResponse>();
        body.Should().NotBeNull();
        body!.Id.Should().Be(_contactId);
        body.FirstName.Should().Be("Alice");
        body.LastName.Should().Be("Smith");
    }

    private record ContactResponse(Guid Id, int Type, int Status, string FirstName, string LastName);
}
