namespace AccountManager.Domain.SelfService;

public interface IProviderRepository
{
    Task<Provider?> GetByIdAsync(ProviderId id, CancellationToken ct = default);
    Task SaveAsync(Provider provider, CancellationToken ct = default);
}
