using AccountManager.Application.Administration;
using AccountManager.Application.SelfService;
using AccountManager.Common.Persistence;
using AccountManager.Domain.Administration;
using AccountManager.Domain.SelfService;
using AccountManager.Infrastructure.Administration;
using AccountManager.Infrastructure.SelfService;
using Microsoft.Extensions.DependencyInjection;

namespace AccountManager.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, string basePath)
    {
        services.AddSingleton(_ => new JsonFileStores(basePath));
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<JsonFileStores>());
        services.AddScoped<IProviderRepository, FileProviderRepository>();
        services.AddScoped<IProviderAdminRepository, FileProviderAdminRepository>();
        services.AddScoped<ISystemAdminRepository, FileSystemAdminRepository>();
        services.AddScoped<IContactRepository, FileContactRepository>();
        services.AddScoped<IAccountProjector, FileAccountProjector>();
        services.AddScoped<IContactProjector, FileContactProjector>();
        return services;
    }
}
