using AccountManager.Domain.Contacts.ValueObjects;

namespace AccountManager.Domain.Contacts.Repositories;

public interface IContactRepository
{
    Task<Contact?> GetByIdAsync(ContactId id, CancellationToken ct = default);
    Task SaveAsync(Contact contact, CancellationToken ct = default);
}
