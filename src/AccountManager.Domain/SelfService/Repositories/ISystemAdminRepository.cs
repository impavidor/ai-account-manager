namespace AccountManager.Domain.SelfService;

public interface ISystemAdminRepository
{
    Task<SystemAdmin?> GetByIdAsync(SystemAdminId id, CancellationToken ct = default);
    Task SaveAsync(SystemAdmin admin, CancellationToken ct = default);
}
