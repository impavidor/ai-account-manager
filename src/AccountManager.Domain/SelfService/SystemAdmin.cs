using AccountManager.Common.Domain;
using AccountManager.Common.Errors;
using AccountManager.Domain.Shared;
using CSharpFunctionalExtensions;

namespace AccountManager.Domain.SelfService;

public class SystemAdmin : AggregateRoot<SystemAdminId>
{
    public ProviderName Name { get; private set; }
    public ContactStatus Status { get; }

    internal SystemAdmin(Guid id, ProviderName name, ContactStatus status)
        : base(new SystemAdminId(id))
    {
        Name = name;
        Status = status;
    }

    private SystemAdmin(SystemAdminId id, ProviderName name) : base(id)
    {
        Name = name;
        Status = ContactStatus.Pending;
    }

    public static Result<SystemAdmin, Error> Register(ProviderName name) =>
        Result.Success<SystemAdmin, Error>(new SystemAdmin(new SystemAdminId(Guid.NewGuid()), name));

    public void ChangeName(ProviderName name) => Name = name;
}
