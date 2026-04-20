namespace AccountManager.Domain.Administration;

public interface IContactRepository
{
    Task<Contact?> GetByIdAsync(ContactId id, CancellationToken ct = default);
    Task SaveAsync(Contact contact, CancellationToken ct = default);
}
