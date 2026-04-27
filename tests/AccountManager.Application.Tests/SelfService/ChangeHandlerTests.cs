using AccountManager.Application;
using AccountManager.Application.SelfService;
using AccountManager.Common.Errors;
using AccountManager.Common.Persistence;
using AccountManager.Domain.Administration;
using AccountManager.Domain.SelfService;
using AccountManager.Domain.Shared;
using CSharpFunctionalExtensions;
using FluentAssertions;

namespace AccountManager.Application.Tests.SelfService;

[TestFixture]
public class ChangeHandlerTests
{
    private IProviderRepository _providerRepo = null!;
    private IProviderAdminRepository _providerAdminRepo = null!;
    private ISystemAdminRepository _systemAdminRepo = null!;
    private IUnitOfWork _uow = null!;

    [SetUp]
    public void SetUp()
    {
        _providerRepo = new InMemoryProviderRepository();
        _providerAdminRepo = new InMemoryProviderAdminRepository();
        _systemAdminRepo = new InMemorySystemAdminRepository();
        _uow = new NullUnitOfWork();
    }

    // --- ChangeProviderNpi ---

    [Test]
    public async Task ChangeProviderNpi_ValidNpi_ReturnsOkAndUpdates()
    {
        var provider = Provider.Register(
            ProviderName.Create("Alice", "Smith").Value,
            Npi.Create("1234567890").Value).Value;
        await _providerRepo.Add(provider);
        var actor = new FakeCurrentActor(provider.Id.Value, ContactType.Provider);
        var handler = new ChangeProviderNpiHandler(_providerRepo, _uow, actor);
        var command = ChangeProviderNpiCommand.Create("0987654321").Value;

        var result = await handler.Handle(command);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeOfType<CommandResult.OkResult>();
        (await _providerRepo.GetById(provider.Id))!.Npi.Value.Should().Be("0987654321");
    }

    [Test]
    public async Task ChangeProviderNpi_InvalidNpi_ReturnsError()
    {
        var commandResult = ChangeProviderNpiCommand.Create("bad");

        commandResult.IsFailure.Should().BeTrue();
        commandResult.Error.Should().BeOfType<InvalidNpiError>();
    }

    [Test]
    public async Task ChangeProviderNpi_ProviderNotFound_ReturnsError()
    {
        var actor = new FakeCurrentActor(Guid.NewGuid(), ContactType.Provider);
        var handler = new ChangeProviderNpiHandler(_providerRepo, _uow, actor);
        var command = ChangeProviderNpiCommand.Create("1234567890").Value;

        var result = await handler.Handle(command);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<ContactNotFoundError>();
    }

    // --- ChangeName ---

    [Test]
    public async Task ChangeName_Provider_ReturnsOkAndUpdates()
    {
        var provider = Provider.Register(
            ProviderName.Create("Alice", "Smith").Value,
            Npi.Create("1234567890").Value).Value;
        await _providerRepo.Add(provider);
        var actor = new FakeCurrentActor(provider.Id.Value, ContactType.Provider);
        var handler = new ChangeNameHandler(_providerRepo, _providerAdminRepo, _systemAdminRepo, _uow, actor);
        var command = ChangeNameCommand.Create("Alice", "Updated").Value;

        var result = await handler.Handle(command);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeOfType<CommandResult.OkResult>();
        (await _providerRepo.GetById(provider.Id))!.Name.LastName.Should().Be("Updated");
    }

    [Test]
    public async Task ChangeName_ProviderAdmin_ReturnsOkAndUpdates()
    {
        var admin = ProviderAdmin.Register(ProviderName.Create("Bob", "Jones").Value).Value;
        await _providerAdminRepo.Add(admin);
        var actor = new FakeCurrentActor(admin.Id.Value, ContactType.ProviderAdmin);
        var handler = new ChangeNameHandler(_providerRepo, _providerAdminRepo, _systemAdminRepo, _uow, actor);
        var command = ChangeNameCommand.Create("Bob", "Updated").Value;

        var result = await handler.Handle(command);

        result.IsSuccess.Should().BeTrue();
        (await _providerAdminRepo.GetById(admin.Id))!.Name.LastName.Should().Be("Updated");
    }

    [Test]
    public async Task ChangeName_SystemAdmin_ReturnsOkAndUpdates()
    {
        var admin = SystemAdmin.Register(ProviderName.Create("Carol", "White").Value).Value;
        await _systemAdminRepo.Add(admin);
        var actor = new FakeCurrentActor(admin.Id.Value, ContactType.SystemAdmin);
        var handler = new ChangeNameHandler(_providerRepo, _providerAdminRepo, _systemAdminRepo, _uow, actor);
        var command = ChangeNameCommand.Create("Carol", "Updated").Value;

        var result = await handler.Handle(command);

        result.IsSuccess.Should().BeTrue();
        (await _systemAdminRepo.GetById(admin.Id))!.Name.LastName.Should().Be("Updated");
    }

    [Test]
    public async Task ChangeName_InvalidName_ReturnsError()
    {
        var commandResult = ChangeNameCommand.Create("Alice", "");

        commandResult.IsFailure.Should().BeTrue();
        commandResult.Error.Should().BeOfType<InvalidProviderNameError>();
    }

    [Test]
    public async Task ChangeName_ActorNotFound_ReturnsError()
    {
        var actor = new FakeCurrentActor(Guid.NewGuid(), ContactType.Provider);
        var handler = new ChangeNameHandler(_providerRepo, _providerAdminRepo, _systemAdminRepo, _uow, actor);
        var command = ChangeNameCommand.Create("Alice", "Smith").Value;

        var result = await handler.Handle(command);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<ContactNotFoundError>();
    }
}

file sealed class FakeCurrentActor : ICurrentActor
{
    public FakeCurrentActor(Guid id, ContactType type)
    {
        ContactId = new ContactId(id);
        ContactType = type;
    }
    public ContactId ContactId { get; }
    public ContactType ContactType { get; }
}

file sealed class InMemoryProviderRepository : IProviderRepository
{
    private readonly Dictionary<Guid, Provider> _store = new();
    public Task<Provider?> GetById(ProviderId id) =>
        Task.FromResult(_store.TryGetValue(id.Value, out var p) ? p : null);
    public Task Add(Provider p) { _store[p.Id.Value] = p; return Task.CompletedTask; }
    public Task Update(Provider p) { _store[p.Id.Value] = p; return Task.CompletedTask; }
}

file sealed class InMemoryProviderAdminRepository : IProviderAdminRepository
{
    private readonly Dictionary<Guid, ProviderAdmin> _store = new();
    public Task<ProviderAdmin?> GetById(ProviderAdminId id) =>
        Task.FromResult(_store.TryGetValue(id.Value, out var a) ? a : null);
    public Task Add(ProviderAdmin a) { _store[a.Id.Value] = a; return Task.CompletedTask; }
    public Task Update(ProviderAdmin a) { _store[a.Id.Value] = a; return Task.CompletedTask; }
}

file sealed class InMemorySystemAdminRepository : ISystemAdminRepository
{
    private readonly Dictionary<Guid, SystemAdmin> _store = new();
    public Task<SystemAdmin?> GetById(SystemAdminId id) =>
        Task.FromResult(_store.TryGetValue(id.Value, out var a) ? a : null);
    public Task Add(SystemAdmin a) { _store[a.Id.Value] = a; return Task.CompletedTask; }
    public Task Update(SystemAdmin a) { _store[a.Id.Value] = a; return Task.CompletedTask; }
}

file sealed class NullUnitOfWork : IUnitOfWork
{
    public Task SaveChanges() => Task.CompletedTask;
}
