using AccountManager.Common.Errors;
using AccountManager.Common.Persistence;
using AccountManager.Domain.Administration;
using AccountManager.Domain.SelfService;
using CSharpFunctionalExtensions;

namespace AccountManager.Application.SelfService;

public sealed class ChangeProviderNpiHandler
{
    private readonly IProviderRepository _repository;
    private readonly IUnitOfWork _uow;
    private readonly ICurrentActor _actor;

    public ChangeProviderNpiHandler(IProviderRepository repository, IUnitOfWork uow, ICurrentActor actor)
    {
        _repository = repository;
        _uow = uow;
        _actor = actor;
    }

    public async Task<Result<CommandResult, Error>> Handle(ChangeProviderNpiCommand command)
    {
        var provider = await _repository.GetById(new ProviderId(_actor.ContactId.Value));
        if (provider is null)
            return Result.Failure<CommandResult, Error>(new ContactNotFoundError(_actor.ContactId));

        provider.ChangeNpi(command.Npi);
        await _repository.Update(provider);
        await _uow.SaveChanges();
        return CommandResult.Ok();
    }
}
