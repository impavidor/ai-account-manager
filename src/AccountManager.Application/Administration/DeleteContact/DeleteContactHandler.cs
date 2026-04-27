using AccountManager.Common.Errors;
using AccountManager.Domain.Administration;
using CSharpFunctionalExtensions;

namespace AccountManager.Application.Administration;

public sealed class DeleteContactHandler : ICommandHandler<DeleteContactCommand>
{
    private readonly IDeleteContactService _service;
    private readonly ICurrentActor _actor;

    public DeleteContactHandler(IDeleteContactService service, ICurrentActor actor)
    {
        _service = service;
        _actor = actor;
    }

    public async Task<Result<CommandResult, Error>> Handle(DeleteContactCommand command) =>
        await _service.DeleteAsync(command.ContactId, _actor.ContactId)
            .Map(() => (CommandResult)CommandResult.Ok());
}
