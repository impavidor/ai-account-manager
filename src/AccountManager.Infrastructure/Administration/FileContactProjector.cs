using AccountManager.Application.Administration;
using AccountManager.Domain.Administration;

namespace AccountManager.Infrastructure.Administration;

public class FileContactProjector : IContactProjector
{
    private readonly JsonFileStores _stores;

    public FileContactProjector(JsonFileStores stores) => _stores = stores;

    public Task<ContactDto?> GetById(ContactId id)
    {
        var dto = _stores.Contacts.FirstOrDefault(x => x.Id == id.Value);
        if (dto is null) return Task.FromResult<ContactDto?>(null);

        return Task.FromResult<ContactDto?>(new ContactDto(
            dto.Id, dto.Type, dto.Status,
            dto.FirstName ?? "", dto.LastName ?? ""));
    }

    public Task<IReadOnlyList<ContactSummaryDto>> GetAll()
    {
        IReadOnlyList<ContactSummaryDto> result = _stores.Contacts
            .Select(dto => new ContactSummaryDto(
                dto.Id, dto.Type, dto.Status,
                dto.FirstName ?? "", dto.LastName ?? ""))
            .ToList();
        return Task.FromResult(result);
    }
}
