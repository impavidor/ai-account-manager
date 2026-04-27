using AccountManager.Application.Administration;
using AccountManager.Application.SelfService;
using AccountManager.Common.Errors;
using AccountManager.Domain.Administration;
using AccountManager.Domain.SelfService;
using Microsoft.Extensions.DependencyInjection;

namespace AccountManager.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IActivateContactService, ActivateContactService>();
        services.AddScoped<IDeleteContactService, DeleteContactService>();

        services.AddScoped<ICommandHandler<VerifyContactCommand>, VerifyContactHandler>();
        services.AddScoped<ICommandHandler<DeleteContactCommand>, DeleteContactHandler>();
        services.AddScoped<IQueryHandler<GetContactQuery, ContactDto>, GetContactHandler>();
        services.AddScoped<IQueryHandler<ListContactsQuery, IReadOnlyList<ContactSummaryDto>>, ListContactsHandler>();

        services.AddScoped<ICommandHandler<RegisterProviderCommand>, RegisterProviderHandler>();
        services.AddScoped<ICommandHandler<RegisterProviderAdminCommand>, RegisterProviderAdminHandler>();
        services.AddScoped<ICommandHandler<RegisterSystemAdminCommand>, RegisterSystemAdminHandler>();
        services.AddScoped<ICommandHandler<ChangeProviderNpiCommand>, ChangeProviderNpiHandler>();
        services.AddScoped<ICommandHandler<ChangeNameCommand>, ChangeNameHandler>();
        services.AddScoped<IQueryHandler<GetAccountQuery, AccountDto>, GetAccountHandler>();

        services.Decorate(typeof(ICommandHandler<>), typeof(UnitOfWorkCommandHandlerDecorator<>));

        return services;
    }
}
