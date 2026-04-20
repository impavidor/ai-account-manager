using AccountManager.Domain.Contacts;
using AccountManager.Domain.Contacts.Errors;
using AccountManager.Domain.Contacts.Repositories;
using AccountManager.Domain.Contacts.Services;
using AccountManager.Domain.Contacts.ValueObjects;
using FluentAssertions;

namespace AccountManager.Domain.Tests.Contacts.Services;

[TestFixture]
public class ActivateContactServiceTests
{
    private IContactRepository _repository = null!;
    private ActivateContactService _service = null!;
    private SystemAdminId _actorId = null!;

    [SetUp]
    public void SetUp()
    {
        _repository = new InMemoryContactRepository();
        _service = new ActivateContactService(_repository);
        _actorId = new SystemAdminId(Guid.NewGuid());
    }

    [Test]
    public async Task ActivateAsync_WithPendingContact_Succeeds()
    {
        var contact = PendingContact();
        await _repository.SaveAsync(contact);

        var result = await _service.ActivateAsync(contact.Id, _actorId);

        result.IsSuccess.Should().BeTrue();
        contact.Status.Should().Be(ContactStatus.Active);
    }

    [Test]
    public async Task ActivateAsync_WhenActorTargetsOwnRecord_Fails()
    {
        var sharedId = Guid.NewGuid();
        var contact = new Contact(
            new ContactId(sharedId),
            ContactType.SystemAdmin,
            ContactStatus.Pending,
            new FullName("Alice", "Smith"));
        await _repository.SaveAsync(contact);

        var result = await _service.ActivateAsync(contact.Id, new SystemAdminId(sharedId));

        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<SelfActionForbiddenError>();
    }

    [Test]
    public async Task ActivateAsync_WhenContactNotFound_Fails()
    {
        var result = await _service.ActivateAsync(new ContactId(Guid.NewGuid()), _actorId);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<ContactNotFoundError>();
    }

    [Test]
    public async Task ActivateAsync_WhenContactAlreadyActive_Fails()
    {
        var contact = new Contact(
            new ContactId(Guid.NewGuid()),
            ContactType.Provider,
            ContactStatus.Active,
            new FullName("Alice", "Smith"));
        await _repository.SaveAsync(contact);

        var result = await _service.ActivateAsync(contact.Id, _actorId);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<InvalidStatusTransitionError>();
    }

    private static Contact PendingContact() => new(
        new ContactId(Guid.NewGuid()),
        ContactType.Provider,
        ContactStatus.Pending,
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
