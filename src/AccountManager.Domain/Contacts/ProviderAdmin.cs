using AccountManager.Domain.Contacts.ValueObjects;

namespace AccountManager.Domain.Contacts;

public class ProviderAdmin
{
    public ProviderAdminId Id { get; }
    public ProviderName Name { get; private set; }
    public ContactStatus Status { get; }

    private ProviderAdmin(ProviderAdminId id, ProviderName name)
    {
        Id = id;
        Name = name;
        Status = ContactStatus.Pending;
    }

    public static ProviderAdmin Register(ProviderName name) =>
        new(new ProviderAdminId(Guid.NewGuid()), name);

    public void ChangeName(ProviderName name) => Name = name;

    public override bool Equals(object? obj) => obj is ProviderAdmin a && a.Id == Id;
    public override int GetHashCode() => Id.GetHashCode();
}
