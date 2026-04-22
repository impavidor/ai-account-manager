using AccountManager.Common.Errors;
using CSharpFunctionalExtensions;

namespace AccountManager.Domain.Administration;

public interface IActivateContactService
{
    Task<UnitResult<Error>> ActivateAsync(ContactId contactId, ContactId actorId);
}
