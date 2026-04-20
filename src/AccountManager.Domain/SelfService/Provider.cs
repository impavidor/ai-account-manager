using AccountManager.Common.Domain;
using AccountManager.Common.Errors;
using AccountManager.Domain.Shared;
using CSharpFunctionalExtensions;

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

    public static Result<Provider, Error> Register(ProviderName name, Npi npi) =>
        Result.Success<Provider, Error>(new Provider(new ProviderId(Guid.NewGuid()), name, npi));

    public void ChangeName(ProviderName name) => Name = name;
    public void ChangeNpi(Npi npi) => Npi = npi;
}
