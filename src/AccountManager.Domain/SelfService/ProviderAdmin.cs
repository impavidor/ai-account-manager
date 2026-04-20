using AccountManager.Common.Domain;
using AccountManager.Domain.Shared;

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

    public static ProviderAdmin Register(ProviderName name) =>
        new(new ProviderAdminId(Guid.NewGuid()), name);

    public void ChangeName(ProviderName name) => Name = name;
}
