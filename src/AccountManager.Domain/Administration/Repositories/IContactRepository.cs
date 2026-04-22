namespace AccountManager.Domain.Administration;

public interface IContactRepository
{
    Task<Contact?> GetByIdAsync(ContactId id);
    Task Add(Contact contact);
    Task Update(Contact contact);
}
