using AccountManager.Common.Errors;
using AccountManager.Domain.Administration;
using CSharpFunctionalExtensions;

namespace AccountManager.Application.SelfService;

public sealed class GetAccountHandler : IQueryHandler<GetAccountQuery, AccountDto>
{
    private readonly IAccountProjector _projector;
    private readonly ICurrentActor _actor;

    public GetAccountHandler(IAccountProjector projector, ICurrentActor actor)
    {
        _projector = projector;
        _actor = actor;
    }

    public async Task<Result<AccountDto, Error>> Handle(GetAccountQuery query) =>
        await _projector.GetById(_actor.ContactId, _actor.ContactType) is { } dto
            ? Result.Success<AccountDto, Error>(dto)
            : Result.Failure<AccountDto, Error>(new ContactNotFoundError(_actor.ContactId));
}
