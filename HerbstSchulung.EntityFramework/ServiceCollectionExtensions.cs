using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace HerbstSchulung.EntityFramework;

/// <summary>
/// DI-Helfer zum Registrieren des DbContext.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registriert den AppDbContext. Für Workshops kann hier z. B. der Provider getauscht werden.
    /// Standard: SQLite In-Memory für schnelle Demos, optional SQL Server via ConnectionString.
    /// </summary>
    public static IServiceCollection AddHerbstSchulungDbContext(
        this IServiceCollection services,
        string? connectionString = null,
        bool useSqlServer = false)
    {
        if (useSqlServer)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(connectionString ?? "Server=(localdb)\\MSSQLLocalDB;Database=HerbstSchulungEf;Trusted_Connection=True;TrustServerCertificate=True;"));
        }
        else
        {
            // Für Demos/Test: SQLite In-Memory (persistiert während der Laufzeit des Prozesses)
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlite(connectionString ?? "Data Source=HerbstSchulungEf.sqlite"));
        }

        return services;
    }
}
