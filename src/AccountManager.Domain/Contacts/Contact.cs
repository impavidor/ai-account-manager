using AccountManager.Common.Errors;
using AccountManager.Domain.Contacts.Errors;
using AccountManager.Domain.Contacts.ValueObjects;
using CSharpFunctionalExtensions;

namespace AccountManager.Domain.Contacts;

public class Contact
{
    public ContactId Id { get; }
    public ContactType Type { get; }
    public ContactStatus Status { get; private set; }
    public ContactName Name { get; }

    public Contact(ContactId id, ContactType type, ContactStatus status, ContactName name)
    {
        Id = id;
        Type = type;
        Status = status;
        Name = name;
    }

    public UnitResult<Error> Activate()
    {
        if (Status != ContactStatus.Pending)
            return UnitResult.Failure<Error>(
                new InvalidStatusTransitionError(Status.ToString(), ContactStatus.Active.ToString()));

        Status = ContactStatus.Active;
        return UnitResult.Success<Error>();
    }

    public UnitResult<Error> Delete()
    {
        if (Status == ContactStatus.Deleted)
            return UnitResult.Failure<Error>(
                new InvalidStatusTransitionError(Status.ToString(), ContactStatus.Deleted.ToString()));

        Status = ContactStatus.Deleted;
        return UnitResult.Success<Error>();
    }

    public override bool Equals(object? obj) => obj is Contact c && c.Id == Id;
    public override int GetHashCode() => Id.GetHashCode();
}
