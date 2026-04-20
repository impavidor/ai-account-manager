using AccountManager.Common.Errors;
using AccountManager.Domain.Contacts.Errors;
using AccountManager.Domain.Contacts.Repositories;
using AccountManager.Domain.Contacts.ValueObjects;
using CSharpFunctionalExtensions;

namespace AccountManager.Domain.Contacts.Services;

public class ActivateContactService : IActivateContactService
{
    private readonly IContactRepository _repository;

    public ActivateContactService(IContactRepository repository) => _repository = repository;

    public async Task<UnitResult<Error>> ActivateAsync(ContactId contactId, SystemAdminId actorId, CancellationToken ct = default)
    {
        if (contactId.Value == actorId.Value)
            return UnitResult.Failure<Error>(new SelfActionForbiddenError());

        var contact = await _repository.GetByIdAsync(contactId, ct);
        if (contact is null)
            return UnitResult.Failure<Error>(new ContactNotFoundError(contactId));

        var result = contact.Activate();
        if (result.IsFailure)
            return result;

        await _repository.SaveAsync(contact, ct);
        return UnitResult.Success<Error>();
    }
}
