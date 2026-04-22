using AccountManager.Domain.Administration;
using AccountManager.Infrastructure.Dtos;

namespace AccountManager.Infrastructure.Administration;

public class FileContactRepository : IContactRepository
{
    private readonly JsonFileStores _stores;

    public FileContactRepository(JsonFileStores stores) => _stores = stores;

    public Task<Contact?> GetById(ContactId id)
    {
        var dto = _stores.Contacts.FirstOrDefault(x => x.Id == id.Value);
        return Task.FromResult(dto is null ? null : ToDomain(dto));
    }

    public Task Add(Contact contact)
    {
        if (_stores.Contacts.Any(x => x.Id == contact.Id.Value))
            throw new InvalidOperationException($"Contact {contact.Id.Value} already exists.");
        _stores.Contacts.Add(ToDto(contact));
        return Task.CompletedTask;
    }

    public Task Update(Contact contact)
    {
        var idx = _stores.Contacts.FindIndex(x => x.Id == contact.Id.Value);
        if (idx < 0)
            throw new InvalidOperationException($"Contact {contact.Id.Value} not found.");
        _stores.Contacts[idx] = ToDto(contact);
        return Task.CompletedTask;
    }

    private static Contact ToDomain(ContactDto dto)
    {
        ContactName name = dto.Type == ContactType.Organization
            ? new OrganizationName(dto.OrgName!)
            : new FullName(dto.FirstName!, dto.LastName!);
        return new Contact(new ContactId(dto.Id), dto.Type, dto.Status, name);
    }

    private static ContactDto ToDto(Contact c) => new()
    {
        Id = c.Id.Value,
        Type = c.Type,
        Status = c.Status,
        FirstName = c.Name is FullName fn ? fn.FirstName : null,
        LastName = c.Name is FullName fn2 ? fn2.LastName : null,
        OrgName = c.Name is OrganizationName org ? org.Name : null
    };
}
