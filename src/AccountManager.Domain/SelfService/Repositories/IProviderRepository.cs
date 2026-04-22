namespace AccountManager.Domain.SelfService;

public interface IProviderRepository
{
    Task<Provider?> GetByIdAsync(ProviderId id, CancellationToken ct = default);
    Task Add(Provider provider, CancellationToken ct = default);
    Task Update(Provider provider, CancellationToken ct = default);
}
