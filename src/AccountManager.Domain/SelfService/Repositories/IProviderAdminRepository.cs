namespace AccountManager.Domain.SelfService;

public interface IProviderAdminRepository
{
    Task<ProviderAdmin?> GetById(ProviderAdminId id);
    Task Add(ProviderAdmin admin);
    Task Update(ProviderAdmin admin);
}
