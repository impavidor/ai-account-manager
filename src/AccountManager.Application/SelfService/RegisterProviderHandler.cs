using AccountManager.Common.Errors;
using AccountManager.Common.Persistence;
using AccountManager.Domain.SelfService;
using CSharpFunctionalExtensions;

namespace AccountManager.Application.SelfService;

public sealed class RegisterProviderHandler
{
    private readonly IProviderRepository _repository;
    private readonly IUnitOfWork _uow;

    public RegisterProviderHandler(IProviderRepository repository, IUnitOfWork uow)
    {
        _repository = repository;
        _uow = uow;
    }

    public async Task<Result<CommandResult, Error>> Handle(RegisterProviderCommand command)
    {
        var provider = Provider.Register(command.Name, command.Npi).Value;
        await _repository.Add(provider);
        await _uow.SaveChanges();
        return CommandResult.Created(provider.Id.Value);
    }
}
