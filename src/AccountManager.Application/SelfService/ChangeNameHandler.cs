using AccountManager.Common.Errors;
using AccountManager.Common.Persistence;
using AccountManager.Domain.Administration;
using AccountManager.Domain.SelfService;
using CSharpFunctionalExtensions;

namespace AccountManager.Application.SelfService;

public sealed class ChangeNameHandler
{
    private readonly IProviderRepository _providerRepo;
    private readonly IProviderAdminRepository _providerAdminRepo;
    private readonly ISystemAdminRepository _systemAdminRepo;
    private readonly IUnitOfWork _uow;
    private readonly ICurrentActor _actor;

    public ChangeNameHandler(
        IProviderRepository providerRepo,
        IProviderAdminRepository providerAdminRepo,
        ISystemAdminRepository systemAdminRepo,
        IUnitOfWork uow,
        ICurrentActor actor)
    {
        _providerRepo = providerRepo;
        _providerAdminRepo = providerAdminRepo;
        _systemAdminRepo = systemAdminRepo;
        _uow = uow;
        _actor = actor;
    }

    public async Task<Result<CommandResult, Error>> Handle(ChangeNameCommand command)
    {
        var id = _actor.ContactId;

        switch (_actor.ContactType)
        {
            case ContactType.Provider:
            {
                var provider = await _providerRepo.GetById(new ProviderId(id.Value));
                if (provider is null)
                    return Result.Failure<CommandResult, Error>(new ContactNotFoundError(id));
                provider.ChangeName(command.Name);
                await _providerRepo.Update(provider);
                break;
            }
            case ContactType.ProviderAdmin:
            {
                var admin = await _providerAdminRepo.GetById(new ProviderAdminId(id.Value));
                if (admin is null)
                    return Result.Failure<CommandResult, Error>(new ContactNotFoundError(id));
                admin.ChangeName(command.Name);
                await _providerAdminRepo.Update(admin);
                break;
            }
            case ContactType.SystemAdmin:
            {
                var admin = await _systemAdminRepo.GetById(new SystemAdminId(id.Value));
                if (admin is null)
                    return Result.Failure<CommandResult, Error>(new ContactNotFoundError(id));
                admin.ChangeName(command.Name);
                await _systemAdminRepo.Update(admin);
                break;
            }
            default:
                return Result.Failure<CommandResult, Error>(
                    new ContactNotFoundError(id));
        }

        await _uow.SaveChanges();
        return CommandResult.Ok();
    }
}
