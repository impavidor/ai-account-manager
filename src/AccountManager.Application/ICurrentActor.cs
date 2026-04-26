using AccountManager.Domain.Administration;

namespace AccountManager.Application;

public interface ICurrentActor
{
    ContactId ContactId { get; }
    ContactType ContactType { get; }
}
