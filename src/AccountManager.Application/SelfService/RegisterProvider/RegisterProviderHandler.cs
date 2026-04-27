using AccountManager.Common.Errors;
using AccountManager.Domain.SelfService;
using CSharpFunctionalExtensions;

namespace AccountManager.Application.SelfService;

public sealed class RegisterProviderHandler : ICommandHandler<RegisterProviderCommand>
{
    private readonly IProviderRepository _repository;

    public RegisterProviderHandler(IProviderRepository repository) => _repository = repository;

    public async Task<Result<CommandResult, Error>> Handle(RegisterProviderCommand command) =>
        await Provider.Register(command.Name, command.Npi)
            .Tap(provider => _repository.Add(provider))
            .Map(provider => (CommandResult)CommandResult.Created(provider.Id.Value));
}
