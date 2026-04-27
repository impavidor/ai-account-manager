using AccountManager.Common.Errors;
using AccountManager.Domain.Administration;
using CSharpFunctionalExtensions;

namespace AccountManager.Application.Administration;

public sealed class GetContactHandler : IQueryHandler<GetContactQuery, ContactDto>
{
    private readonly IContactProjector _projector;

    public GetContactHandler(IContactProjector projector) => _projector = projector;

    public async Task<Result<ContactDto, Error>> Handle(GetContactQuery query) =>
        await _projector.GetById(query.ContactId) is { } dto
            ? Result.Success<ContactDto, Error>(dto)
            : Result.Failure<ContactDto, Error>(new ContactNotFoundError(query.ContactId));
}
