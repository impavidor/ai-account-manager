using AccountManager.Common.Domain;
using AccountManager.Common.Errors;
using AccountManager.Domain.Shared;
using CSharpFunctionalExtensions;

namespace AccountManager.Domain.Administration;

public class Contact : AggregateRoot<ContactId>
{
    public ContactType Type { get; }
    public ContactStatus Status { get; private set; }
    public ContactName Name { get; }

    internal Contact(Guid id, ContactType type, ContactStatus status, ContactName name)
        : base(new ContactId(id))
    {
        Type = type;
        Status = status;
        Name = name;
    }

    private Contact(ContactId id, ContactType type, ContactStatus status, ContactName name) : base(id)
    {
        Type = type;
        Status = status;
        Name = name;
    }

    public static Result<Contact, Error> Register(ContactType type, ContactName name) =>
        Result.Success<Contact, Error>(new Contact(new ContactId(Guid.NewGuid()), type, ContactStatus.Pending, name));

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
}
