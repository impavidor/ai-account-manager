using AccountManager.Domain.Administration;
using AccountManager.Domain.Shared;
using FluentAssertions;

namespace AccountManager.Domain.Tests.Contacts.Services;

[TestFixture]
public class ActivateContactServiceTests
{
    private IContactRepository _repository = null!;
    private ActivateContactService _service = null!;
    private ContactId _actorId = null!;

    [SetUp]
    public void SetUp()
    {
        _repository = new InMemoryContactRepository();
        _service = new ActivateContactService(_repository);
        _actorId = new ContactId(Guid.NewGuid());
    }

    [Test]
    public async Task ActivateAsync_WithPendingContact_Succeeds()
    {
        var contact = Contact.Register(ContactType.Provider, new FullName("Alice", "Smith")).Value;
        await _repository.Add(contact);

        var result = await _service.ActivateAsync(contact.Id, _actorId);

        result.IsSuccess.Should().BeTrue();
        contact.Status.Should().Be(ContactStatus.Active);
    }

    [Test]
    public async Task ActivateAsync_WhenActorTargetsOwnRecord_Fails()
    {
        var contact = Contact.Register(ContactType.SystemAdmin, new FullName("Alice", "Smith")).Value;
        await _repository.Add(contact);

        var result = await _service.ActivateAsync(contact.Id, contact.Id);

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
        var contact = Contact.Register(ContactType.Provider, new FullName("Alice", "Smith")).Value;
        contact.Activate();
        await _repository.Add(contact);

        var result = await _service.ActivateAsync(contact.Id, _actorId);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<InvalidStatusTransitionError>();
    }
}

file sealed class InMemoryContactRepository : IContactRepository
{
    private readonly Dictionary<Guid, Contact> _store = new();

    public Task<Contact?> GetById(ContactId id) =>
        Task.FromResult(_store.TryGetValue(id.Value, out var c) ? c : null);

    public Task Add(Contact contact)
    {
        if (_store.ContainsKey(contact.Id.Value))
            throw new InvalidOperationException($"Contact {contact.Id.Value} already exists.");
        _store[contact.Id.Value] = contact;
        return Task.CompletedTask;
    }

    public Task Update(Contact contact)
    {
        if (!_store.ContainsKey(contact.Id.Value))
            throw new InvalidOperationException($"Contact {contact.Id.Value} not found.");
        _store[contact.Id.Value] = contact;
        return Task.CompletedTask;
    }
}
