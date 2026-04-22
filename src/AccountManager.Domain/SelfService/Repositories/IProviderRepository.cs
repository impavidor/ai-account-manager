namespace AccountManager.Domain.SelfService;

public interface IProviderRepository
{
    Task<Provider?> GetById(ProviderId id);
    Task Add(Provider provider);
    Task Update(Provider provider);
}
