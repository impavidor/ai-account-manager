using AccountManager.Domain.Contacts.ValueObjects;

namespace AccountManager.Domain.Contacts;

public class SystemAdmin
{
    public SystemAdminId Id { get; }
    public ProviderName Name { get; private set; }
    public ContactStatus Status { get; }

    private SystemAdmin(SystemAdminId id, ProviderName name)
    {
        Id = id;
        Name = name;
        Status = ContactStatus.Pending;
    }

    public static SystemAdmin Register(ProviderName name) =>
        new(new SystemAdminId(Guid.NewGuid()), name);

    public void ChangeName(ProviderName name) => Name = name;

    public override bool Equals(object? obj) => obj is SystemAdmin a && a.Id == Id;
    public override int GetHashCode() => Id.GetHashCode();
}
