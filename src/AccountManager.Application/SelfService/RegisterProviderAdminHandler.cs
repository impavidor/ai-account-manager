using AccountManager.Common.Errors;
using AccountManager.Common.Persistence;
using AccountManager.Domain.SelfService;
using CSharpFunctionalExtensions;

namespace AccountManager.Application.SelfService;

public sealed class RegisterProviderAdminHandler
{
    private readonly IProviderAdminRepository _repository;
    private readonly IUnitOfWork _uow;

    public RegisterProviderAdminHandler(IProviderAdminRepository repository, IUnitOfWork uow)
    {
        _repository = repository;
        _uow = uow;
    }

    public async Task<Result<CommandResult, Error>> Handle(RegisterProviderAdminCommand command)
    {
        var admin = ProviderAdmin.Register(command.Name).Value;
        await _repository.Add(admin);
        await _uow.SaveChanges();
        return CommandResult.Created(admin.Id.Value);
    }
}
