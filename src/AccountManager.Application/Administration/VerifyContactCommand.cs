using AccountManager.Common.Errors;
using AccountManager.Domain.Administration;
using CSharpFunctionalExtensions;

namespace AccountManager.Application.Administration;

public sealed class VerifyContactCommand
{
    public ContactId ContactId { get; }

    private VerifyContactCommand(ContactId contactId) => ContactId = contactId;

    public static Result<VerifyContactCommand, Error> Create(Guid contactId) =>
        new VerifyContactCommand(new ContactId(contactId));
}
