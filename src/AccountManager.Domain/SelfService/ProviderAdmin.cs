using AccountManager.Common.Domain;
using AccountManager.Common.Errors;
using AccountManager.Domain.Shared;
using CSharpFunctionalExtensions;

namespace AccountManager.Domain.SelfService;

public class ProviderAdmin : AggregateRoot<ProviderAdminId>
{
    public ProviderName Name { get; private set; }
    public ContactStatus Status { get; }

    private ProviderAdmin(ProviderAdminId id, ProviderName name) : base(id)
    {
        Name = name;
        Status = ContactStatus.Pending;
    }

    public static Result<ProviderAdmin, Error> Register(ProviderName name) =>
        Result.Success<ProviderAdmin, Error>(new ProviderAdmin(new ProviderAdminId(Guid.NewGuid()), name));

    public void ChangeName(ProviderName name) => Name = name;
}
