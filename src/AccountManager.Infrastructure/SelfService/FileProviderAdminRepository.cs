using AccountManager.Domain.SelfService;
using AccountManager.Infrastructure.Dtos;

namespace AccountManager.Infrastructure.SelfService;

public class FileProviderAdminRepository : IProviderAdminRepository
{
    private readonly JsonFileStore<ProviderAdminDto> _store;

    public FileProviderAdminRepository(FileRepositoryOptions options)
        => _store = new JsonFileStore<ProviderAdminDto>(Path.Combine(options.BasePath, "provideradmins.json"));

    public async Task<ProviderAdmin?> GetByIdAsync(ProviderAdminId id, CancellationToken ct = default)
    {
        var all = await _store.LoadAllAsync(ct);
        var dto = all.FirstOrDefault(x => x.Id == id.Value);
        return dto is null ? null : ToDomain(dto);
    }

    public async Task Add(ProviderAdmin admin, CancellationToken ct = default)
    {
        var all = (await _store.LoadAllAsync(ct)).ToList();
        if (all.Any(x => x.Id == admin.Id.Value))
            throw new InvalidOperationException($"ProviderAdmin {admin.Id.Value} already exists.");
        all.Add(ToDto(admin));
        await _store.SaveAllAsync(all, ct);
    }

    public async Task Update(ProviderAdmin admin, CancellationToken ct = default)
    {
        var all = (await _store.LoadAllAsync(ct)).ToList();
        var idx = all.FindIndex(x => x.Id == admin.Id.Value);
        if (idx < 0)
            throw new InvalidOperationException($"ProviderAdmin {admin.Id.Value} not found.");
        all[idx] = ToDto(admin);
        await _store.SaveAllAsync(all, ct);
    }

    private static ProviderAdmin ToDomain(ProviderAdminDto dto) =>
        new(new ProviderAdminId(dto.Id), new ProviderName(dto.FirstName, dto.LastName), dto.Status);

    private static ProviderAdminDto ToDto(ProviderAdmin a) => new()
    {
        Id = a.Id.Value,
        FirstName = a.Name.FirstName,
        LastName = a.Name.LastName,
        Status = a.Status
    };
}
