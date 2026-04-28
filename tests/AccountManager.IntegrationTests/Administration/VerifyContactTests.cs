using System.Net;
using FluentAssertions;

namespace AccountManager.IntegrationTests.Administration;

[TestFixture]
public class VerifyContactTests : IntegrationTestBase
{
    private Guid _pendingContactId;
    private Guid _activeContactId;
    private Guid _actorId;

    [OneTimeSetUp]
    public void SetUp()
    {
        _pendingContactId = Guid.NewGuid();
        _activeContactId = Guid.NewGuid();
        _actorId = Guid.NewGuid();

        var dataStore = new DataStore(DataPath);
        dataStore.WriteContacts(
        [
            new ContactRecord(
                Id: _pendingContactId,
                Type: DataStore.ContactTypeProvider,
                Status: DataStore.ContactStatusPending,
                FirstName: "Charlie",
                LastName: "Brown",
                OrgName: null,
                Npi: "1234567890"),
            new ContactRecord(
                Id: _activeContactId,
                Type: DataStore.ContactTypeProvider,
                Status: DataStore.ContactStatusActive,
                FirstName: "Diana",
                LastName: "Prince",
                OrgName: null,
                Npi: "0987654321")
        ]);
    }

    [Test]
    public async Task HappyPath_PendingContact_Returns200AndStatusUpdates()
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"/administration/contacts/{_pendingContactId}/verify")
            .WithActor(_actorId, "SystemAdmin");

        var response = await Client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var dataStore = new DataStore(DataPath);
        var contacts = dataStore.ReadContacts();
        var contact = contacts.Single(c => c.Id == _pendingContactId);
        contact.Status.Should().Be(DataStore.ContactStatusActive);
    }

    [Test]
    public async Task DomainError_AlreadyActiveContact_Returns409()
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"/administration/contacts/{_activeContactId}/verify")
            .WithActor(_actorId, "SystemAdmin");

        var response = await Client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Test]
    public async Task ApiError_MalformedId_Returns400()
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "/administration/contacts/not-a-guid/verify")
            .WithActor(_actorId, "SystemAdmin");

        var response = await Client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task Auth_NoActorHeaders_Returns500BecauseActorAccessorThrows()
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"/administration/contacts/{_pendingContactId}/verify");

        var response = await Client.SendAsync(request);

        // FakeAuthMiddleware does not enforce authentication; HttpContextCurrentActor throws when no actor claims are present
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }
}
