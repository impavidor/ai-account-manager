using AccountManager.Domain.SelfService;
using AccountManager.Infrastructure.Dtos;

namespace AccountManager.Infrastructure.SelfService;

public class FileSystemAdminRepository : ISystemAdminRepository
{
    private readonly JsonFileStore<SystemAdminDto> _store;

    public FileSystemAdminRepository(FileRepositoryOptions options)
        => _store = new JsonFileStore<SystemAdminDto>(Path.Combine(options.BasePath, "systemadmins.json"));

    public async Task<SystemAdmin?> GetByIdAsync(SystemAdminId id, CancellationToken ct = default)
    {
        var all = await _store.LoadAllAsync(ct);
        var dto = all.FirstOrDefault(x => x.Id == id.Value);
        return dto is null ? null : ToDomain(dto);
    }

    public async Task SaveAsync(SystemAdmin admin, CancellationToken ct = default)
    {
        var all = (await _store.LoadAllAsync(ct)).ToList();
        var idx = all.FindIndex(x => x.Id == admin.Id.Value);
        var dto = ToDto(admin);
        if (idx >= 0) all[idx] = dto; else all.Add(dto);
        await _store.SaveAllAsync(all, ct);
    }

    private static SystemAdmin ToDomain(SystemAdminDto dto) =>
        new(dto.Id, new ProviderName(dto.FirstName, dto.LastName), dto.Status);

    private static SystemAdminDto ToDto(SystemAdmin a) => new()
    {
        Id = a.Id.Value,
        FirstName = a.Name.FirstName,
        LastName = a.Name.LastName,
        Status = a.Status
    };
}
