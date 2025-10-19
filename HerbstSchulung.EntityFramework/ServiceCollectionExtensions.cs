using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace HerbstSchulung.EntityFramework;

/// <summary>
/// DI-Helfer zum Registrieren des DbContext Factory.
/// </summary>
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddHerbstSchulungPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Default") 
            ?? throw new InvalidOperationException("Connection string 'Default' not found in configuration.");
        
        services.AddSingleton<IAppDbContextFactory>(sp => new AppDbContextFactory(connectionString, sp.GetRequiredService<ILoggerFactory>()));

        return services;
    }
}
