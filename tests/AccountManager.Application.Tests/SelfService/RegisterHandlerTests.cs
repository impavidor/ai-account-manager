using AccountManager.Application;
using AccountManager.Application.SelfService;
using AccountManager.Common.Errors;
using AccountManager.Domain.SelfService;
using CSharpFunctionalExtensions;
using FluentAssertions;

namespace AccountManager.Application.Tests.SelfService;

[TestFixture]
public class RegisterHandlerTests
{
    private IProviderRepository _providerRepo = null!;
    private IProviderAdminRepository _providerAdminRepo = null!;
    private ISystemAdminRepository _systemAdminRepo = null!;

    [SetUp]
    public void SetUp()
    {
        _providerRepo = new InMemoryProviderRepository();
        _providerAdminRepo = new InMemoryProviderAdminRepository();
        _systemAdminRepo = new InMemorySystemAdminRepository();
    }

    // --- RegisterProvider ---

    [Test]
    public async Task RegisterProvider_ValidInputs_ReturnsCreatedAndPersists()
    {
        var handler = new RegisterProviderHandler(_providerRepo);
        var command = RegisterProviderCommand.Create("Alice", "Smith", "1234567890").Value;

        var result = await handler.Handle(command);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeOfType<CommandResult.CreatedResult>();
        var id = ((CommandResult.CreatedResult)result.Value).Id;
        (await _providerRepo.GetById(new ProviderId(id))).Should().NotBeNull();
    }

    [Test]
    public async Task RegisterProvider_InvalidNpi_ReturnsError()
    {
        var commandResult = RegisterProviderCommand.Create("Alice", "Smith", "bad-npi");

        commandResult.IsFailure.Should().BeTrue();
        commandResult.Error.Should().BeOfType<InvalidNpiError>();
    }

    [Test]
    public async Task RegisterProvider_InvalidName_ReturnsError()
    {
        var commandResult = RegisterProviderCommand.Create("", "Smith", "1234567890");

        commandResult.IsFailure.Should().BeTrue();
        commandResult.Error.Should().BeOfType<InvalidProviderNameError>();
    }

    // --- RegisterProviderAdmin ---

    [Test]
    public async Task RegisterProviderAdmin_ValidInputs_ReturnsCreatedAndPersists()
    {
        var handler = new RegisterProviderAdminHandler(_providerAdminRepo);
        var command = RegisterProviderAdminCommand.Create("Bob", "Jones").Value;

        var result = await handler.Handle(command);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeOfType<CommandResult.CreatedResult>();
        var id = ((CommandResult.CreatedResult)result.Value).Id;
        (await _providerAdminRepo.GetById(new ProviderAdminId(id))).Should().NotBeNull();
    }

    [Test]
    public async Task RegisterProviderAdmin_InvalidName_ReturnsError()
    {
        var commandResult = RegisterProviderAdminCommand.Create("Bob", "");

        commandResult.IsFailure.Should().BeTrue();
        commandResult.Error.Should().BeOfType<InvalidProviderNameError>();
    }

    // --- RegisterSystemAdmin ---

    [Test]
    public async Task RegisterSystemAdmin_ValidInputs_ReturnsCreatedAndPersists()
    {
        var handler = new RegisterSystemAdminHandler(_systemAdminRepo);
        var command = RegisterSystemAdminCommand.Create("Carol", "White").Value;

        var result = await handler.Handle(command);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeOfType<CommandResult.CreatedResult>();
        var id = ((CommandResult.CreatedResult)result.Value).Id;
        (await _systemAdminRepo.GetById(new SystemAdminId(id))).Should().NotBeNull();
    }

    [Test]
    public async Task RegisterSystemAdmin_InvalidName_ReturnsError()
    {
        var commandResult = RegisterSystemAdminCommand.Create("Carol", "");

        commandResult.IsFailure.Should().BeTrue();
        commandResult.Error.Should().BeOfType<InvalidProviderNameError>();
    }
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

