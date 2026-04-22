using AccountManager.Domain.Administration;
using AccountManager.Domain.SelfService;
using AccountManager.Infrastructure.Dtos;

namespace AccountManager.Infrastructure.SelfService;

public class FileProviderRepository : IProviderRepository
{
    private readonly JsonFileStore<ContactDto> _store;

    public FileProviderRepository(FileRepositoryOptions options)
        => _store = new JsonFileStore<ContactDto>(Path.Combine(options.BasePath, "contacts.json"));

    public async Task<Provider?> GetById(ProviderId id)
    {
        var all = await _store.LoadAllAsync();
        var dto = all.FirstOrDefault(x => x.Id == id.Value && x.Type == ContactType.Provider);
        return dto is null ? null : ToDomain(dto);
    }

    public async Task Add(Provider provider)
    {
        var all = (await _store.LoadAllAsync()).ToList();
        if (all.Any(x => x.Id == provider.Id.Value))
            throw new InvalidOperationException($"Provider {provider.Id.Value} already exists.");
        all.Add(ToDto(provider));
        await _store.SaveAllAsync(all);
    }

    public async Task Update(Provider provider)
    {
        var all = (await _store.LoadAllAsync()).ToList();
        var idx = all.FindIndex(x => x.Id == provider.Id.Value && x.Type == ContactType.Provider);
        if (idx < 0)
            throw new InvalidOperationException($"Provider {provider.Id.Value} not found.");
        all[idx] = ToDto(provider);
        await _store.SaveAllAsync(all);
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
