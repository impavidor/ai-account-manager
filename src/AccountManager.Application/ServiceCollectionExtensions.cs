using AccountManager.Domain.Administration;
using Microsoft.Extensions.DependencyInjection;

namespace AccountManager.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IActivateContactService, ActivateContactService>();
        services.AddScoped<IDeleteContactService, DeleteContactService>();
        return services;
    }
}
