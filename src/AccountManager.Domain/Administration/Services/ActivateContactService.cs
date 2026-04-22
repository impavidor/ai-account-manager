using AccountManager.Common.Errors;
using CSharpFunctionalExtensions;

namespace AccountManager.Domain.Administration;

public class ActivateContactService : IActivateContactService
{
    private readonly IContactRepository _repository;

    public ActivateContactService(IContactRepository repository) => _repository = repository;

    public async Task<UnitResult<Error>> ActivateAsync(ContactId contactId, ContactId actorId, CancellationToken ct = default)
    {
        if (contactId == actorId)
            return UnitResult.Failure<Error>(new SelfActionForbiddenError());

        var contact = await _repository.GetByIdAsync(contactId, ct);
        if (contact is null)
            return UnitResult.Failure<Error>(new ContactNotFoundError(contactId));

        var result = contact.Activate();
        if (result.IsFailure)
            return result;

        await _repository.Update(contact, ct);
        return UnitResult.Success<Error>();
    }
}
