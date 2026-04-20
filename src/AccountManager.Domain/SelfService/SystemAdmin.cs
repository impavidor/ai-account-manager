using AccountManager.Common.Domain;
using AccountManager.Domain.Shared;

namespace AccountManager.Domain.SelfService;

public class SystemAdmin : AggregateRoot<SystemAdminId>
{
    public ProviderName Name { get; private set; }
    public ContactStatus Status { get; }

    private SystemAdmin(SystemAdminId id, ProviderName name) : base(id)
    {
        Name = name;
        Status = ContactStatus.Pending;
    }

    public static SystemAdmin Register(ProviderName name) =>
        new(new SystemAdminId(Guid.NewGuid()), name);

    public void ChangeName(ProviderName name) => Name = name;
}
