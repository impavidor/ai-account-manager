using AccountManager.Common.Errors;
using AccountManager.Domain.Administration;
using CSharpFunctionalExtensions;

namespace AccountManager.Application.Administration;

public sealed class GetContactQuery
{
    public ContactId ContactId { get; }

    private GetContactQuery(ContactId contactId) => ContactId = contactId;

    public static Result<GetContactQuery, Error> Create(Guid contactId) =>
        new GetContactQuery(new ContactId(contactId));
}
