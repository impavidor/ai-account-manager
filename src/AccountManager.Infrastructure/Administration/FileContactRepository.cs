using AccountManager.Domain.Administration;
using AccountManager.Infrastructure.Dtos;

namespace AccountManager.Infrastructure.Administration;

public class FileContactRepository : IContactRepository
{
    private readonly JsonFileStore<ContactDto> _store;

    public FileContactRepository(FileRepositoryOptions options)
        => _store = new JsonFileStore<ContactDto>(Path.Combine(options.BasePath, "contacts.json"));

    public async Task<Contact?> GetById(ContactId id)
    {
        var all = await _store.LoadAllAsync();
        var dto = all.FirstOrDefault(x => x.Id == id.Value);
        return dto is null ? null : ToDomain(dto);
    }

    public async Task Add(Contact contact)
    {
        var all = (await _store.LoadAllAsync()).ToList();
        if (all.Any(x => x.Id == contact.Id.Value))
            throw new InvalidOperationException($"Contact {contact.Id.Value} already exists.");
        all.Add(ToDto(contact));
        await _store.SaveAllAsync(all);
    }

    public async Task Update(Contact contact)
    {
        var all = (await _store.LoadAllAsync()).ToList();
        var idx = all.FindIndex(x => x.Id == contact.Id.Value);
        if (idx < 0)
            throw new InvalidOperationException($"Contact {contact.Id.Value} not found.");
        all[idx] = ToDto(contact);
        await _store.SaveAllAsync(all);
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
