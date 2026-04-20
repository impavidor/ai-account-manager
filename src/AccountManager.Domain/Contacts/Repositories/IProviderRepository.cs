using AccountManager.Domain.Contacts.ValueObjects;

namespace AccountManager.Domain.Contacts.Repositories;

public interface IProviderRepository
{
    Task<Provider?> GetByIdAsync(ProviderId id, CancellationToken ct = default);
    Task SaveAsync(Provider provider, CancellationToken ct = default);
}
