using AccountManager.Domain.Administration;

namespace AccountManager.Application.Administration;

public interface IContactProjector
{
    Task<ContactDto?> GetById(ContactId id);
    Task<IReadOnlyList<ContactSummaryDto>> GetAll();
}
