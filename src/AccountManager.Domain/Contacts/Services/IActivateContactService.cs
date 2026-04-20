using AccountManager.Common.Errors;
using AccountManager.Domain.Contacts.ValueObjects;
using CSharpFunctionalExtensions;

namespace AccountManager.Domain.Contacts.Services;

public interface IActivateContactService
{
    Task<UnitResult<Error>> ActivateAsync(ContactId contactId, SystemAdminId actorId, CancellationToken ct = default);
}
