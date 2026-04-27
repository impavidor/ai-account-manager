using AccountManager.Common.Errors;
using AccountManager.Domain.Administration;
using CSharpFunctionalExtensions;

namespace AccountManager.Application.Administration;

public sealed class VerifyContactHandler : ICommandHandler<VerifyContactCommand>
{
    private readonly IActivateContactService _service;
    private readonly ICurrentActor _actor;

    public VerifyContactHandler(IActivateContactService service, ICurrentActor actor)
    {
        _service = service;
        _actor = actor;
    }

    public async Task<Result<CommandResult, Error>> Handle(VerifyContactCommand command) =>
        await _service.ActivateAsync(command.ContactId, _actor.ContactId)
            .Map(() => (CommandResult)CommandResult.Ok());
}
