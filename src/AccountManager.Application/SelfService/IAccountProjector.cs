using AccountManager.Domain.Administration;

namespace AccountManager.Application.SelfService;

public interface IAccountProjector
{
    Task<AccountDto?> GetById(ContactId id, ContactType type);
}
