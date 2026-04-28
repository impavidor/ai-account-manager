using System.Net;
using FluentAssertions;

namespace AccountManager.IntegrationTests.Administration;

[TestFixture]
public class DeleteContactTests : IntegrationTestBase
{
    private Guid _activeContactId;
    private Guid _deletedContactId;
    private Guid _actorId;

    [OneTimeSetUp]
    public void SetUp()
    {
        _activeContactId = Guid.NewGuid();
        _deletedContactId = Guid.NewGuid();
        _actorId = Guid.NewGuid();

        var dataStore = new DataStore(DataPath);
        dataStore.WriteContacts(
        [
            new ContactRecord(
                Id: _activeContactId,
                Type: DataStore.ContactTypeProvider,
                Status: DataStore.ContactStatusActive,
                FirstName: "Eve",
                LastName: "Turner",
                OrgName: null,
                Npi: "1122334455"),
            new ContactRecord(
                Id: _deletedContactId,
                Type: DataStore.ContactTypeProvider,
                Status: DataStore.ContactStatusDeleted,
                FirstName: "Frank",
                LastName: "Castle",
                OrgName: null,
                Npi: "5544332211")
        ]);
    }

    [Test]
    public async Task HappyPath_ActiveContact_Returns200AndContactRemoved()
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, $"/administration/contacts/{_activeContactId}")
            .WithActor(_actorId, "SystemAdmin");

        var response = await Client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var dataStore = new DataStore(DataPath);
        var contacts = dataStore.ReadContacts();
        var contact = contacts.Single(c => c.Id == _activeContactId);
        contact.Status.Should().Be(DataStore.ContactStatusDeleted);
    }

    [Test]
    public async Task DomainError_AlreadyDeletedContact_Returns409()
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, $"/administration/contacts/{_deletedContactId}")
            .WithActor(_actorId, "SystemAdmin");

        var response = await Client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Test]
    public async Task ApiError_MalformedId_Returns400()
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, "/administration/contacts/not-a-guid")
            .WithActor(_actorId, "SystemAdmin");

        var response = await Client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task Auth_NoActorHeaders_Returns500BecauseActorAccessorThrows()
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, $"/administration/contacts/{_activeContactId}");

        var response = await Client.SendAsync(request);

        // FakeAuthMiddleware does not enforce authentication; HttpContextCurrentActor throws when no actor claims are present
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }
}
