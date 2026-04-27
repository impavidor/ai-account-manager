using AccountManager.Common.Errors;
using AccountManager.Domain.SelfService;
using CSharpFunctionalExtensions;

namespace AccountManager.Application.SelfService;

public sealed class RegisterProviderAdminHandler : ICommandHandler<RegisterProviderAdminCommand>
{
    private readonly IProviderAdminRepository _repository;

    public RegisterProviderAdminHandler(IProviderAdminRepository repository) => _repository = repository;

    public async Task<Result<CommandResult, Error>> Handle(RegisterProviderAdminCommand command) =>
        await ProviderAdmin.Register(command.Name)
            .Tap(admin => _repository.Add(admin))
            .Map(admin => (CommandResult)CommandResult.Created(admin.Id.Value));
}
