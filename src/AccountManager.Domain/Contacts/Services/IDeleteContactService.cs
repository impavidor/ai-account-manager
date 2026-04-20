using AccountManager.Common.Errors;
using AccountManager.Domain.Contacts.ValueObjects;
using CSharpFunctionalExtensions;

namespace AccountManager.Domain.Contacts.Services;

public interface IDeleteContactService
{
    Task<UnitResult<Error>> DeleteAsync(ContactId contactId, SystemAdminId actorId, CancellationToken ct = default);
}
