namespace AccountManager.Domain.SelfService;

public interface IProviderRepository
{
    Task<Provider?> GetByIdAsync(ProviderId id);
    Task Add(Provider provider);
    Task Update(Provider provider);
}
