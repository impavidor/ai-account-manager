using AccountManager.Common.Errors;
using AccountManager.Domain.Administration;
using CSharpFunctionalExtensions;

namespace AccountManager.Application.Administration;

public sealed class DeleteContactCommand
{
    public ContactId ContactId { get; }

    private DeleteContactCommand(ContactId contactId) => ContactId = contactId;

    public static Result<DeleteContactCommand, Error> Create(Guid contactId) =>
        new DeleteContactCommand(new ContactId(contactId));
}
