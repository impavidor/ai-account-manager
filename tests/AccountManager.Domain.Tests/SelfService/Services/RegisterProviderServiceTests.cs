using AccountManager.Domain.SelfService;
using AccountManager.Domain.Shared;
using FluentAssertions;

namespace AccountManager.Domain.Tests.Contacts.Services;

[TestFixture]
public class RegisterProviderServiceTests
{
    private IProviderRepository _repository = null!;
    private RegisterProviderService _service = null!;

    [SetUp]
    public void SetUp()
    {
        _repository = new InMemoryProviderRepository();
        _service = new RegisterProviderService(_repository);
    }

    [Test]
    public async Task RegisterAsync_CreatesProviderWithPendingStatus()
    {
        var name = ProviderName.Create("Alice", "Smith").Value;
        var npi = Npi.Create("1234567890").Value;

        var result = await _service.RegisterAsync(name, npi);

        result.IsSuccess.Should().BeTrue();
        result.Value.Status.Should().Be(ContactStatus.Pending);
    }

    [Test]
    public async Task RegisterAsync_AssignsNameAndNpi()
    {
        var name = ProviderName.Create("Alice", "Smith").Value;
        var npi = Npi.Create("1234567890").Value;

        var result = await _service.RegisterAsync(name, npi);

        result.Value.Name.Should().Be(name);
        result.Value.Npi.Should().Be(npi);
    }

    [Test]
    public async Task RegisterAsync_PersistsProvider()
    {
        var name = ProviderName.Create("Alice", "Smith").Value;
        var npi = Npi.Create("1234567890").Value;

        var result = await _service.RegisterAsync(name, npi);

        var saved = await _repository.GetByIdAsync(result.Value.Id);
        saved.Should().NotBeNull();
    }
}

file sealed class InMemoryProviderRepository : IProviderRepository
{
    private readonly Dictionary<Guid, Provider> _store = new();

    public Task<Provider?> GetByIdAsync(ProviderId id, CancellationToken ct = default) =>
        Task.FromResult(_store.TryGetValue(id.Value, out var p) ? p : null);

    public Task Add(Provider provider, CancellationToken ct = default)
    {
        if (_store.ContainsKey(provider.Id.Value))
            throw new InvalidOperationException($"Provider {provider.Id.Value} already exists.");
        _store[provider.Id.Value] = provider;
        return Task.CompletedTask;
    }

    public Task Update(Provider provider, CancellationToken ct = default)
    {
        if (!_store.ContainsKey(provider.Id.Value))
            throw new InvalidOperationException($"Provider {provider.Id.Value} not found.");
        _store[provider.Id.Value] = provider;
        return Task.CompletedTask;
    }
}
