using AccountManager.Common.Errors;
using AccountManager.Common.Persistence;
using AccountManager.Domain.Administration;
using CSharpFunctionalExtensions;

namespace AccountManager.Application.Administration;

public sealed class VerifyContactHandler
{
    private readonly IActivateContactService _service;
    private readonly IUnitOfWork _uow;
    private readonly ICurrentActor _actor;

    public VerifyContactHandler(IActivateContactService service, IUnitOfWork uow, ICurrentActor actor)
    {
        _service = service;
        _uow = uow;
        _actor = actor;
    }

    public async Task<Result<CommandResult, Error>> Handle(VerifyContactCommand command)
    {
        var result = await _service.ActivateAsync(command.ContactId, _actor.ContactId);
        if (result.IsFailure)
            return Result.Failure<CommandResult, Error>(result.Error);

        await _uow.SaveChanges();
        return CommandResult.Ok();
    }
}
