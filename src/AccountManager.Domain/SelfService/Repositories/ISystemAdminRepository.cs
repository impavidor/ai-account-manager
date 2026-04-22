namespace AccountManager.Domain.SelfService;

public interface ISystemAdminRepository
{
    Task<SystemAdmin?> GetByIdAsync(SystemAdminId id, CancellationToken ct = default);
    Task Add(SystemAdmin admin, CancellationToken ct = default);
    Task Update(SystemAdmin admin, CancellationToken ct = default);
}
