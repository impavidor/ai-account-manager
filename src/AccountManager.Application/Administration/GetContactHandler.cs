using AccountManager.Common.Errors;
using AccountManager.Domain.Administration;
using CSharpFunctionalExtensions;

namespace AccountManager.Application.Administration;

public sealed class GetContactHandler
{
    private readonly IContactProjector _projector;

    public GetContactHandler(IContactProjector projector) => _projector = projector;

    public async Task<Result<ContactDto, Error>> Handle(GetContactQuery query)
    {
        var dto = await _projector.GetById(query.ContactId);
        if (dto is null)
            return Result.Failure<ContactDto, Error>(new ContactNotFoundError(query.ContactId));
        return dto;
    }
}
