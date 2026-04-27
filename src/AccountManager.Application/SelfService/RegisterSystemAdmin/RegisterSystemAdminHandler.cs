using AccountManager.Common.Errors;
using AccountManager.Domain.SelfService;
using CSharpFunctionalExtensions;

namespace AccountManager.Application.SelfService;

public sealed class RegisterSystemAdminHandler : ICommandHandler<RegisterSystemAdminCommand>
{
    private readonly ISystemAdminRepository _repository;

    public RegisterSystemAdminHandler(ISystemAdminRepository repository) => _repository = repository;

    public async Task<Result<CommandResult, Error>> Handle(RegisterSystemAdminCommand command) =>
        await SystemAdmin.Register(command.Name)
            .Tap(admin => _repository.Add(admin))
            .Map(admin => (CommandResult)CommandResult.Created(admin.Id.Value));
}
