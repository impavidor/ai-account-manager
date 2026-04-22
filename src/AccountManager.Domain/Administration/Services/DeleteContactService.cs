using AccountManager.Common.Errors;
using CSharpFunctionalExtensions;

namespace AccountManager.Domain.Administration;

public class DeleteContactService : IDeleteContactService
{
    private readonly IContactRepository _repository;

    public DeleteContactService(IContactRepository repository) => _repository = repository;

    public async Task<UnitResult<Error>> DeleteAsync(ContactId contactId, ContactId actorId)
    {
        if (contactId == actorId)
            return UnitResult.Failure<Error>(new SelfActionForbiddenError());

        var contact = await _repository.GetById(contactId);
        if (contact is null)
            return UnitResult.Failure<Error>(new ContactNotFoundError(contactId));

        var result = contact.Delete();
        if (result.IsFailure)
            return result;

        await _repository.Update(contact);
        return UnitResult.Success<Error>();
    }
}
