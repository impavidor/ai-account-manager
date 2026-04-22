namespace AccountManager.Domain.Administration;

public interface IContactRepository
{
    Task<Contact?> GetByIdAsync(ContactId id, CancellationToken ct = default);
    Task Add(Contact contact, CancellationToken ct = default);
    Task Update(Contact contact, CancellationToken ct = default);
}
