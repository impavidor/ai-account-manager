namespace AccountManager.Domain.SelfService;

public interface ISystemAdminRepository
{
    Task<SystemAdmin?> GetByIdAsync(SystemAdminId id);
    Task Add(SystemAdmin admin);
    Task Update(SystemAdmin admin);
}
