using AccountManager.Common.Errors;
using AccountManager.Common.Persistence;
using AccountManager.Domain.SelfService;
using CSharpFunctionalExtensions;

namespace AccountManager.Application.SelfService;

public sealed class RegisterSystemAdminHandler
{
    private readonly ISystemAdminRepository _repository;
    private readonly IUnitOfWork _uow;

    public RegisterSystemAdminHandler(ISystemAdminRepository repository, IUnitOfWork uow)
    {
        _repository = repository;
        _uow = uow;
    }

    public async Task<Result<CommandResult, Error>> Handle(RegisterSystemAdminCommand command)
    {
        var admin = SystemAdmin.Register(command.Name).Value;
        await _repository.Add(admin);
        await _uow.SaveChanges();
        return CommandResult.Created(admin.Id.Value);
    }
}
