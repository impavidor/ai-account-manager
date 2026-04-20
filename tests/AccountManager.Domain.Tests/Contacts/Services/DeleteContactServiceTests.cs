using AccountManager.Domain.Contacts;
using AccountManager.Domain.Contacts.Errors;
using AccountManager.Domain.Contacts.Repositories;
using AccountManager.Domain.Contacts.Services;
using AccountManager.Domain.Contacts.ValueObjects;
using FluentAssertions;

namespace AccountManager.Domain.Tests.Contacts.Services;

[TestFixture]
public class DeleteContactServiceTests
{
    private IContactRepository _repository = null!;
    private DeleteContactService _service = null!;
    private SystemAdminId _actorId = null!;

    [SetUp]
    public void SetUp()
    {
        _repository = new InMemoryContactRepository();
        _service = new DeleteContactService(_repository);
        _actorId = new SystemAdminId(Guid.NewGuid());
    }

    [Test]
    public async Task DeleteAsync_WithPendingContact_Succeeds()
    {
        var contact = ContactWithStatus(ContactStatus.Pending);
        await _repository.SaveAsync(contact);

        var result = await _service.DeleteAsync(contact.Id, _actorId);

        result.IsSuccess.Should().BeTrue();
        contact.Status.Should().Be(ContactStatus.Deleted);
    }

    [Test]
    public async Task DeleteAsync_WithActiveContact_Succeeds()
    {
        var contact = ContactWithStatus(ContactStatus.Active);
        await _repository.SaveAsync(contact);

        var result = await _service.DeleteAsync(contact.Id, _actorId);

        result.IsSuccess.Should().BeTrue();
        contact.Status.Should().Be(ContactStatus.Deleted);
    }

    [Test]
    public async Task DeleteAsync_WhenActorTargetsOwnRecord_Fails()
    {
        var sharedId = Guid.NewGuid();
        var contact = new Contact(
            new ContactId(sharedId),
            ContactType.SystemAdmin,
            ContactStatus.Active,
            new FullName("Alice", "Smith"));
        await _repository.SaveAsync(contact);

        var result = await _service.DeleteAsync(contact.Id, new SystemAdminId(sharedId));

        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<SelfActionForbiddenError>();
    }

    [Test]
    public async Task DeleteAsync_WhenContactNotFound_Fails()
    {
        var result = await _service.DeleteAsync(new ContactId(Guid.NewGuid()), _actorId);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<ContactNotFoundError>();
    }

    [Test]
    public async Task DeleteAsync_WhenContactAlreadyDeleted_Fails()
    {
        var contact = ContactWithStatus(ContactStatus.Deleted);
        await _repository.SaveAsync(contact);

        var result = await _service.DeleteAsync(contact.Id, _actorId);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<InvalidStatusTransitionError>();
    }

    private static Contact ContactWithStatus(ContactStatus status) => new(
        new ContactId(Guid.NewGuid()),
        ContactType.Provider,
        status,
        new FullName("Alice", "Smith"));
}

file sealed class InMemoryContactRepository : IContactRepository
{
    private readonly Dictionary<Guid, Contact> _store = new();

    public Task<Contact?> GetByIdAsync(ContactId id, CancellationToken ct = default) =>
        Task.FromResult(_store.TryGetValue(id.Value, out var c) ? c : null);

    public Task SaveAsync(Contact contact, CancellationToken ct = default)
    {
        _store[contact.Id.Value] = contact;
        return Task.CompletedTask;
    }
}
