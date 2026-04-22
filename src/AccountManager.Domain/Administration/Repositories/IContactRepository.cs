namespace AccountManager.Domain.Administration;

public interface IContactRepository
{
    Task<Contact?> GetById(ContactId id);
    Task Add(Contact contact);
    Task Update(Contact contact);
}
