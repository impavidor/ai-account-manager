using AccountManager.Domain.SelfService;
using AccountManager.Infrastructure.Dtos;

namespace AccountManager.Infrastructure.SelfService;

public class FileProviderRepository : IProviderRepository
{
    private readonly JsonFileStore<ProviderDto> _store;

    public FileProviderRepository(FileRepositoryOptions options)
        => _store = new JsonFileStore<ProviderDto>(Path.Combine(options.BasePath, "providers.json"));

    public async Task<Provider?> GetByIdAsync(ProviderId id)
    {
        var all = await _store.LoadAllAsync();
        var dto = all.FirstOrDefault(x => x.Id == id.Value);
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
        var idx = all.FindIndex(x => x.Id == provider.Id.Value);
        if (idx < 0)
            throw new InvalidOperationException($"Provider {provider.Id.Value} not found.");
        all[idx] = ToDto(provider);
        await _store.SaveAllAsync(all);
    }

    private static Provider ToDomain(ProviderDto dto) =>
        new(new ProviderId(dto.Id), new ProviderName(dto.FirstName, dto.LastName), new Npi(dto.Npi), dto.Status);

    private static ProviderDto ToDto(Provider p) => new()
    {
        Id = p.Id.Value,
        FirstName = p.Name.FirstName,
        LastName = p.Name.LastName,
        Npi = p.Npi.Value,
        Status = p.Status
    };
}
