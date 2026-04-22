using AccountManager.Domain.Administration;
using AccountManager.Domain.SelfService;
using AccountManager.Infrastructure.Dtos;

namespace AccountManager.Infrastructure.SelfService;

public class FileSystemAdminRepository : ISystemAdminRepository
{
    private readonly JsonFileStores _stores;

    public FileSystemAdminRepository(JsonFileStores stores) => _stores = stores;

    public Task<SystemAdmin?> GetById(SystemAdminId id)
    {
        var dto = _stores.Contacts.FirstOrDefault(x => x.Id == id.Value && x.Type == ContactType.SystemAdmin);
        return Task.FromResult(dto is null ? null : ToDomain(dto));
    }

    public Task Add(SystemAdmin admin)
    {
        if (_stores.Contacts.Any(x => x.Id == admin.Id.Value))
            throw new InvalidOperationException($"SystemAdmin {admin.Id.Value} already exists.");
        _stores.Contacts.Add(ToDto(admin));
        return Task.CompletedTask;
    }

    public Task Update(SystemAdmin admin)
    {
        var idx = _stores.Contacts.FindIndex(x => x.Id == admin.Id.Value && x.Type == ContactType.SystemAdmin);
        if (idx < 0)
            throw new InvalidOperationException($"SystemAdmin {admin.Id.Value} not found.");
        _stores.Contacts[idx] = ToDto(admin);
        return Task.CompletedTask;
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
