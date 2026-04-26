using AccountManager.Application;
using Microsoft.Extensions.DependencyInjection;

namespace AccountManager.WebAPI;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddWebApiServices(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentActor, HttpContextCurrentActor>();
        services.AddScoped<ResultMapper>();
        return services;
    }
}
