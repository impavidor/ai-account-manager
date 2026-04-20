using AccountManager.Domain.Contacts.ValueObjects;

namespace AccountManager.Domain.Contacts.Repositories;

public interface IProviderAdminRepository
{
    Task<ProviderAdmin?> GetByIdAsync(ProviderAdminId id, CancellationToken ct = default);
    Task SaveAsync(ProviderAdmin admin, CancellationToken ct = default);
}
