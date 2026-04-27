using AccountManager.Application;
using AccountManager.Application.Administration;
using AccountManager.Common.Errors;
using AccountManager.Domain.Administration;
using AccountManager.Domain.Shared;
using FluentAssertions;

namespace AccountManager.Application.Tests.Administration;

[TestFixture]
public class AdminCommandHandlerTests
{
    private IContactRepository _repo = null!;

    [SetUp]
    public void SetUp()
    {
        _repo = new InMemoryContactRepository();
    }

    // --- VerifyContact ---

    [Test]
    public async Task VerifyContact_PendingContact_ReturnsOkAndActivates()
    {
        var contact = Contact.Register(ContactType.Provider, new FullName("Alice", "Smith")).Value;
        await _repo.Add(contact);
        var actor = new FakeCurrentActor(Guid.NewGuid(), ContactType.SystemAdmin);
        var service = new ActivateContactService(_repo);
        var handler = new VerifyContactHandler(service, actor);
        var command = VerifyContactCommand.Create(contact.Id.Value).Value;

        var result = await handler.Handle(command);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeOfType<CommandResult.OkResult>();
        (await _repo.GetById(contact.Id))!.Status.Should().Be(ContactStatus.Active);
    }

    [Test]
    public async Task VerifyContact_ContactNotFound_ReturnsError()
    {
        var actor = new FakeCurrentActor(Guid.NewGuid(), ContactType.SystemAdmin);
        var service = new ActivateContactService(_repo);
        var handler = new VerifyContactHandler(service, actor);
        var command = VerifyContactCommand.Create(Guid.NewGuid()).Value;

        var result = await handler.Handle(command);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<ContactNotFoundError>();
    }

    [Test]
    public async Task VerifyContact_SelfAction_ReturnsForbidden()
    {
        var contact = Contact.Register(ContactType.SystemAdmin, new FullName("Alice", "Smith")).Value;
        await _repo.Add(contact);
        var actor = new FakeCurrentActor(contact.Id.Value, ContactType.SystemAdmin);
        var service = new ActivateContactService(_repo);
        var handler = new VerifyContactHandler(service, actor);
        var command = VerifyContactCommand.Create(contact.Id.Value).Value;

        var result = await handler.Handle(command);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<SelfActionForbiddenError>();
    }

    [Test]
    public async Task VerifyContact_AlreadyActive_ReturnsConflict()
    {
        var contact = Contact.Register(ContactType.Provider, new FullName("Alice", "Smith")).Value;
        contact.Activate();
        await _repo.Add(contact);
        var actor = new FakeCurrentActor(Guid.NewGuid(), ContactType.SystemAdmin);
        var service = new ActivateContactService(_repo);
        var handler = new VerifyContactHandler(service, actor);
        var command = VerifyContactCommand.Create(contact.Id.Value).Value;

        var result = await handler.Handle(command);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<InvalidStatusTransitionError>();
    }

    // --- DeleteContact ---

    [Test]
    public async Task DeleteContact_ActiveContact_ReturnsOkAndDeletes()
    {
        var contact = Contact.Register(ContactType.Provider, new FullName("Bob", "Jones")).Value;
        contact.Activate();
        await _repo.Add(contact);
        var actor = new FakeCurrentActor(Guid.NewGuid(), ContactType.SystemAdmin);
        var service = new DeleteContactService(_repo);
        var handler = new DeleteContactHandler(service, actor);
        var command = DeleteContactCommand.Create(contact.Id.Value).Value;

        var result = await handler.Handle(command);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeOfType<CommandResult.OkResult>();
        (await _repo.GetById(contact.Id))!.Status.Should().Be(ContactStatus.Deleted);
    }

    [Test]
    public async Task DeleteContact_SelfAction_ReturnsForbidden()
    {
        var contact = Contact.Register(ContactType.SystemAdmin, new FullName("Bob", "Jones")).Value;
        contact.Activate();
        await _repo.Add(contact);
        var actor = new FakeCurrentActor(contact.Id.Value, ContactType.SystemAdmin);
        var service = new DeleteContactService(_repo);
        var handler = new DeleteContactHandler(service, actor);
        var command = DeleteContactCommand.Create(contact.Id.Value).Value;

        var result = await handler.Handle(command);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<SelfActionForbiddenError>();
    }
}

file sealed class FakeCurrentActor : ICurrentActor
{
    public FakeCurrentActor(Guid id, ContactType type) { ContactId = new ContactId(id); ContactType = type; }
    public ContactId ContactId { get; }
    public ContactType ContactType { get; }
}

file sealed class InMemoryContactRepository : IContactRepository
{
    private readonly Dictionary<Guid, Contact> _store = new();
    public Task<Contact?> GetById(ContactId id) =>
        Task.FromResult(_store.TryGetValue(id.Value, out var c) ? c : null);
    public Task Add(Contact c) { _store[c.Id.Value] = c; return Task.CompletedTask; }
    public Task Update(Contact c) { _store[c.Id.Value] = c; return Task.CompletedTask; }
}
