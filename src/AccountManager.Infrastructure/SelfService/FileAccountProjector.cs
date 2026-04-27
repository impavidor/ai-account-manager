using AccountManager.Application.SelfService;
using AccountManager.Domain.Administration;

namespace AccountManager.Infrastructure.SelfService;

public class FileAccountProjector : IAccountProjector
{
    private readonly JsonFileStores _stores;

    public FileAccountProjector(JsonFileStores stores) => _stores = stores;

    public Task<AccountDto?> GetById(ContactId id, ContactType type)
    {
        var dto = _stores.Contacts.FirstOrDefault(x => x.Id == id.Value && x.Type == type);
        if (dto is null) return Task.FromResult<AccountDto?>(null);

        return Task.FromResult<AccountDto?>(new AccountDto(
            dto.Id, dto.Type, dto.Status,
            dto.FirstName ?? "", dto.LastName ?? "",
            dto.Type == ContactType.Provider ? dto.Npi : null));
    }
}
