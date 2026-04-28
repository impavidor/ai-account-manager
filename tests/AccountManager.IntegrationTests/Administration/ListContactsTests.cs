using System.Net;
using System.Net.Http.Json;
using FluentAssertions;

namespace AccountManager.IntegrationTests.Administration;

[TestFixture]
public class ListContactsTests : IntegrationTestBase
{
    private Guid _actorId;

    [OneTimeSetUp]
    public void SetUp()
    {
        _actorId = Guid.NewGuid();

        var dataStore = new DataStore(DataPath);
        dataStore.WriteContacts(
        [
            new ContactRecord(
                Id: Guid.NewGuid(),
                Type: DataStore.ContactTypeSystemAdmin,
                Status: DataStore.ContactStatusActive,
                FirstName: "Alice",
                LastName: "Smith",
                OrgName: null,
                Npi: null),
            new ContactRecord(
                Id: Guid.NewGuid(),
                Type: DataStore.ContactTypeProvider,
                Status: DataStore.ContactStatusPending,
                FirstName: "Bob",
                LastName: "Jones",
                OrgName: null,
                Npi: "1234567890")
        ]);
    }

    [Test]
    public async Task HappyPath_SeededContacts_Returns200()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "/administration/contacts")
            .WithActor(_actorId, "SystemAdmin");

        var response = await Client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content.ReadFromJsonAsync<List<ContactSummaryResponse>>();
        body.Should().NotBeNull();
        body!.Count.Should().BeGreaterThanOrEqualTo(2);
    }

    [Test]
    public async Task Auth_NoActorHeaders_Returns200BecauseEndpointIsUnprotected()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "/administration/contacts");

        var response = await Client.SendAsync(request);

        // FakeAuthMiddleware does not enforce authentication; unauthenticated requests reach the handler
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    private record ContactSummaryResponse(Guid Id, int Type, int Status);
}
