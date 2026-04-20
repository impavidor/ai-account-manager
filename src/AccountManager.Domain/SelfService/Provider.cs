using AccountManager.Common.Domain;
using AccountManager.Domain.Shared;

namespace AccountManager.Domain.SelfService;

public class Provider : AggregateRoot<ProviderId>
{
    public ProviderName Name { get; private set; }
    public Npi Npi { get; private set; }
    public ContactStatus Status { get; }

    private Provider(ProviderId id, ProviderName name, Npi npi) : base(id)
    {
        Name = name;
        Npi = npi;
        Status = ContactStatus.Pending;
    }

    public static Provider Register(ProviderName name, Npi npi) =>
        new(new ProviderId(Guid.NewGuid()), name, npi);

    public void ChangeName(ProviderName name) => Name = name;
    public void ChangeNpi(Npi npi) => Npi = npi;
}
