using AccountManager.Domain.Shared;

namespace AccountManager.Domain.SelfService;

public class Provider
{
    public ProviderId Id { get; }
    public ProviderName Name { get; private set; }
    public Npi Npi { get; private set; }
    public ContactStatus Status { get; }

    private Provider(ProviderId id, ProviderName name, Npi npi)
    {
        Id = id;
        Name = name;
        Npi = npi;
        Status = ContactStatus.Pending;
    }

    public static Provider Register(ProviderName name, Npi npi) =>
        new(new ProviderId(Guid.NewGuid()), name, npi);

    public void ChangeName(ProviderName name) => Name = name;
    public void ChangeNpi(Npi npi) => Npi = npi;

    public override bool Equals(object? obj) => obj is Provider p && p.Id == Id;
    public override int GetHashCode() => Id.GetHashCode();
}
