using AccountManager.Domain.SelfService;
using AccountManager.Infrastructure.Dtos;

namespace AccountManager.Infrastructure.SelfService;

public class FileProviderRepository : IProviderRepository
{
    private readonly JsonFileStore<ProviderDto> _store;

    public FileProviderRepository(FileRepositoryOptions options)
        => _store = new JsonFileStore<ProviderDto>(Path.Combine(options.BasePath, "providers.json"));

    public async Task<Provider?> GetByIdAsync(ProviderId id, CancellationToken ct = default)
    {
        var all = await _store.LoadAllAsync(ct);
        var dto = all.FirstOrDefault(x => x.Id == id.Value);
        return dto is null ? null : ToDomain(dto);
    }

    public async Task SaveAsync(Provider provider, CancellationToken ct = default)
    {
        var all = (await _store.LoadAllAsync(ct)).ToList();
        var idx = all.FindIndex(x => x.Id == provider.Id.Value);
        var dto = ToDto(provider);
        if (idx >= 0) all[idx] = dto; else all.Add(dto);
        await _store.SaveAllAsync(all, ct);
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
