using AccountManager.Domain.Contacts.ValueObjects;

namespace AccountManager.Domain.Contacts.Repositories;

public interface ISystemAdminRepository
{
    Task<SystemAdmin?> GetByIdAsync(SystemAdminId id, CancellationToken ct = default);
    Task SaveAsync(SystemAdmin admin, CancellationToken ct = default);
}
