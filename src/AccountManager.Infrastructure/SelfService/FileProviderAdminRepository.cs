using AccountManager.Domain.Administration;
using AccountManager.Domain.SelfService;
using AccountManager.Infrastructure.Dtos;

namespace AccountManager.Infrastructure.SelfService;

public class FileProviderAdminRepository : IProviderAdminRepository
{
    private readonly JsonFileStore<ContactDto> _store;

    public FileProviderAdminRepository(FileRepositoryOptions options)
        => _store = new JsonFileStore<ContactDto>(Path.Combine(options.BasePath, "contacts.json"));

    public async Task<ProviderAdmin?> GetById(ProviderAdminId id)
    {
        var all = await _store.LoadAllAsync();
        var dto = all.FirstOrDefault(x => x.Id == id.Value && x.Type == ContactType.ProviderAdmin);
        return dto is null ? null : ToDomain(dto);
    }

    public async Task Add(ProviderAdmin admin)
    {
        var all = (await _store.LoadAllAsync()).ToList();
        if (all.Any(x => x.Id == admin.Id.Value))
            throw new InvalidOperationException($"ProviderAdmin {admin.Id.Value} already exists.");
        all.Add(ToDto(admin));
        await _store.SaveAllAsync(all);
    }

    public async Task Update(ProviderAdmin admin)
    {
        var all = (await _store.LoadAllAsync()).ToList();
        var idx = all.FindIndex(x => x.Id == admin.Id.Value && x.Type == ContactType.ProviderAdmin);
        if (idx < 0)
            throw new InvalidOperationException($"ProviderAdmin {admin.Id.Value} not found.");
        all[idx] = ToDto(admin);
        await _store.SaveAllAsync(all);
    }

    private static ProviderAdmin ToDomain(ContactDto dto) =>
        new(new ProviderAdminId(dto.Id), new ProviderName(dto.FirstName!, dto.LastName!), dto.Status);

    private static ContactDto ToDto(ProviderAdmin a) => new()
    {
        Id = a.Id.Value,
        Type = ContactType.ProviderAdmin,
        Status = a.Status,
        FirstName = a.Name.FirstName,
        LastName = a.Name.LastName
    };
}
