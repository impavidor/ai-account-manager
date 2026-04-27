using AccountManager.Application;
using AccountManager.Application.SelfService;
using AccountManager.Domain.Administration;
using AccountManager.Domain.SelfService;
using AccountManager.Domain.Shared;
using FluentAssertions;

namespace AccountManager.Application.Tests.SelfService;

[TestFixture]
public class GetAccountHandlerTests
{
    // --- GetAccountHandler ---

    [Test]
    public async Task GetAccount_ReturnsAccountDto_ForProvider()
    {
        var id = Guid.NewGuid();
        var projector = new FakeAccountProjector(new AccountDto(id, ContactType.Provider, ContactStatus.Active, "Alice", "Smith", "1234567890"));
        var actor = new FakeCurrentActor(id, ContactType.Provider);
        var handler = new GetAccountHandler(projector, actor);

        var result = await handler.Handle(new GetAccountQuery());

        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be(id);
        result.Value.Npi.Should().Be("1234567890");
    }

    [Test]
    public async Task GetAccount_ReturnsAccountDto_ForSystemAdmin()
    {
        var id = Guid.NewGuid();
        var projector = new FakeAccountProjector(new AccountDto(id, ContactType.SystemAdmin, ContactStatus.Active, "Carol", "White", null));
        var actor = new FakeCurrentActor(id, ContactType.SystemAdmin);
        var handler = new GetAccountHandler(projector, actor);

        var result = await handler.Handle(new GetAccountQuery());

        result.IsSuccess.Should().BeTrue();
        result.Value.Npi.Should().BeNull();
    }

    [Test]
    public async Task GetAccount_WhenNotFound_ReturnsError()
    {
        var projector = new FakeAccountProjector(null);
        var actor = new FakeCurrentActor(Guid.NewGuid(), ContactType.Provider);
        var handler = new GetAccountHandler(projector, actor);

        var result = await handler.Handle(new GetAccountQuery());

        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<ContactNotFoundError>();
    }
}

file sealed class FakeCurrentActor : ICurrentActor
{
    public FakeCurrentActor(Guid id, ContactType type) { ContactId = new ContactId(id); ContactType = type; }
    public ContactId ContactId { get; }
    public ContactType ContactType { get; }
}

file sealed class FakeAccountProjector : IAccountProjector
{
    private readonly AccountDto? _dto;
    public FakeAccountProjector(AccountDto? dto) => _dto = dto;
    public Task<AccountDto?> GetById(ContactId id, ContactType type) => Task.FromResult(_dto);
}
