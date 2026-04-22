using AccountManager.Domain.Administration;
using AccountManager.Domain.SelfService;
using AccountManager.Infrastructure.Dtos;

namespace AccountManager.Infrastructure.SelfService;

public class FileProviderRepository : IProviderRepository
{
    private readonly JsonFileStores _stores;

    public FileProviderRepository(JsonFileStores stores) => _stores = stores;

    public Task<Provider?> GetById(ProviderId id)
    {
        var dto = _stores.Contacts.FirstOrDefault(x => x.Id == id.Value && x.Type == ContactType.Provider);
        return Task.FromResult(dto is null ? null : ToDomain(dto));
    }

    public Task Add(Provider provider)
    {
        if (_stores.Contacts.Any(x => x.Id == provider.Id.Value))
            throw new InvalidOperationException($"Provider {provider.Id.Value} already exists.");
        _stores.Contacts.Add(ToDto(provider));
        return Task.CompletedTask;
    }

    public Task Update(Provider provider)
    {
        var idx = _stores.Contacts.FindIndex(x => x.Id == provider.Id.Value && x.Type == ContactType.Provider);
        if (idx < 0)
            throw new InvalidOperationException($"Provider {provider.Id.Value} not found.");
        _stores.Contacts[idx] = ToDto(provider);
        return Task.CompletedTask;
    }

    private static Provider ToDomain(ContactDto dto) =>
        new(new ProviderId(dto.Id), new ProviderName(dto.FirstName!, dto.LastName!), new Npi(dto.Npi!), dto.Status);

    private static ContactDto ToDto(Provider p) => new()
    {
        Id = p.Id.Value,
        Type = ContactType.Provider,
        Status = p.Status,
        FirstName = p.Name.FirstName,
        LastName = p.Name.LastName,
        Npi = p.Npi.Value
    };
}
