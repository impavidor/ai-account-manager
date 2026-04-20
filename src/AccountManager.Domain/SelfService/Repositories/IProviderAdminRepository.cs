namespace AccountManager.Domain.SelfService;

public interface IProviderAdminRepository
{
    Task<ProviderAdmin?> GetByIdAsync(ProviderAdminId id, CancellationToken ct = default);
    Task SaveAsync(ProviderAdmin admin, CancellationToken ct = default);
}
