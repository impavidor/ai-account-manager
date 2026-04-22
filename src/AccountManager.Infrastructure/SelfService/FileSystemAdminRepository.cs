using AccountManager.Domain.SelfService;
using AccountManager.Infrastructure.Dtos;

namespace AccountManager.Infrastructure.SelfService;

public class FileSystemAdminRepository : ISystemAdminRepository
{
    private readonly JsonFileStore<SystemAdminDto> _store;

    public FileSystemAdminRepository(FileRepositoryOptions options)
        => _store = new JsonFileStore<SystemAdminDto>(Path.Combine(options.BasePath, "systemadmins.json"));

    public async Task<SystemAdmin?> GetByIdAsync(SystemAdminId id)
    {
        var all = await _store.LoadAllAsync();
        var dto = all.FirstOrDefault(x => x.Id == id.Value);
        return dto is null ? null : ToDomain(dto);
    }

    public async Task Add(SystemAdmin admin)
    {
        var all = (await _store.LoadAllAsync()).ToList();
        if (all.Any(x => x.Id == admin.Id.Value))
            throw new InvalidOperationException($"SystemAdmin {admin.Id.Value} already exists.");
        all.Add(ToDto(admin));
        await _store.SaveAllAsync(all);
    }

    public async Task Update(SystemAdmin admin)
    {
        var all = (await _store.LoadAllAsync()).ToList();
        var idx = all.FindIndex(x => x.Id == admin.Id.Value);
        if (idx < 0)
            throw new InvalidOperationException($"SystemAdmin {admin.Id.Value} not found.");
        all[idx] = ToDto(admin);
        await _store.SaveAllAsync(all);
    }

    private static SystemAdmin ToDomain(SystemAdminDto dto) =>
        new(new SystemAdminId(dto.Id), new ProviderName(dto.FirstName, dto.LastName), dto.Status);

    private static SystemAdminDto ToDto(SystemAdmin a) => new()
    {
        Id = a.Id.Value,
        FirstName = a.Name.FirstName,
        LastName = a.Name.LastName,
        Status = a.Status
    };
}
