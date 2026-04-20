using AccountManager.Common.Errors;
using CSharpFunctionalExtensions;

namespace AccountManager.Domain.Administration;

public interface IDeleteContactService
{
    Task<UnitResult<Error>> DeleteAsync(ContactId contactId, ContactId actorId, CancellationToken ct = default);
}
