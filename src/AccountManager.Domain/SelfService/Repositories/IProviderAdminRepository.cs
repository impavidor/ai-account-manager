namespace AccountManager.Domain.SelfService;

public interface IProviderAdminRepository
{
    Task<ProviderAdmin?> GetByIdAsync(ProviderAdminId id, CancellationToken ct = default);
    Task Add(ProviderAdmin admin, CancellationToken ct = default);
    Task Update(ProviderAdmin admin, CancellationToken ct = default);
}
