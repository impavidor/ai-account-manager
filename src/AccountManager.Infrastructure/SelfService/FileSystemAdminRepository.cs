using AccountManager.Domain.Administration;
using AccountManager.Domain.SelfService;
using AccountManager.Infrastructure.Dtos;

namespace AccountManager.Infrastructure.SelfService;

public class FileSystemAdminRepository : ISystemAdminRepository
{
    private readonly JsonFileStore<ContactDto> _store;

    public FileSystemAdminRepository(FileRepositoryOptions options)
        => _store = new JsonFileStore<ContactDto>(Path.Combine(options.BasePath, "contacts.json"));

    public async Task<SystemAdmin?> GetById(SystemAdminId id)
    {
        var all = await _store.LoadAllAsync();
        var dto = all.FirstOrDefault(x => x.Id == id.Value && x.Type == ContactType.SystemAdmin);
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
        var idx = all.FindIndex(x => x.Id == admin.Id.Value && x.Type == ContactType.SystemAdmin);
        if (idx < 0)
            throw new InvalidOperationException($"SystemAdmin {admin.Id.Value} not found.");
        all[idx] = ToDto(admin);
        await _store.SaveAllAsync(all);
    }

    private static SystemAdmin ToDomain(ContactDto dto) =>
        new(new SystemAdminId(dto.Id), new ProviderName(dto.FirstName!, dto.LastName!), dto.Status);

    private static ContactDto ToDto(SystemAdmin a) => new()
    {
        Id = a.Id.Value,
        Type = ContactType.SystemAdmin,
        Status = a.Status,
        FirstName = a.Name.FirstName,
        LastName = a.Name.LastName
    };
}
