using AccountManager.Common.Errors;
using AccountManager.Domain.Administration;
using AccountManager.Domain.SelfService;
using CSharpFunctionalExtensions;

namespace AccountManager.Application.SelfService;

public sealed class ChangeProviderNpiHandler : ICommandHandler<ChangeProviderNpiCommand>
{
    private readonly IProviderRepository _repository;
    private readonly ICurrentActor _actor;

    public ChangeProviderNpiHandler(IProviderRepository repository, ICurrentActor actor)
    {
        _repository = repository;
        _actor = actor;
    }

    public async Task<Result<CommandResult, Error>> Handle(ChangeProviderNpiCommand command)
    {
        var provider = await _repository.GetById(new ProviderId(_actor.ContactId.Value));
        if (provider is null)
            return Result.Failure<CommandResult, Error>(new ContactNotFoundError(_actor.ContactId));

        provider.ChangeNpi(command.Npi);
        await _repository.Update(provider);
        return (CommandResult)CommandResult.Ok();
    }
}
