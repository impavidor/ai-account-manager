namespace AccountManager.Domain.SelfService;

public interface IProviderAdminRepository
{
    Task<ProviderAdmin?> GetByIdAsync(ProviderAdminId id);
    Task Add(ProviderAdmin admin);
    Task Update(ProviderAdmin admin);
}
