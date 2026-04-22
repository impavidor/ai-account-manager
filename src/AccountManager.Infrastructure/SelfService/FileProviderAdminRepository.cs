using AccountManager.Domain.Administration;
using AccountManager.Domain.SelfService;
using AccountManager.Infrastructure.Dtos;

namespace AccountManager.Infrastructure.SelfService;

public class FileProviderAdminRepository : IProviderAdminRepository
{
    private readonly JsonFileStores _stores;

    public FileProviderAdminRepository(JsonFileStores stores) => _stores = stores;

    public Task<ProviderAdmin?> GetById(ProviderAdminId id)
    {
        var dto = _stores.Contacts.FirstOrDefault(x => x.Id == id.Value && x.Type == ContactType.ProviderAdmin);
        return Task.FromResult(dto is null ? null : ToDomain(dto));
    }

    public Task Add(ProviderAdmin admin)
    {
        if (_stores.Contacts.Any(x => x.Id == admin.Id.Value))
            throw new InvalidOperationException($"ProviderAdmin {admin.Id.Value} already exists.");
        _stores.Contacts.Add(ToDto(admin));
        return Task.CompletedTask;
    }

    public Task Update(ProviderAdmin admin)
    {
        var idx = _stores.Contacts.FindIndex(x => x.Id == admin.Id.Value && x.Type == ContactType.ProviderAdmin);
        if (idx < 0)
            throw new InvalidOperationException($"ProviderAdmin {admin.Id.Value} not found.");
        _stores.Contacts[idx] = ToDto(admin);
        return Task.CompletedTask;
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
