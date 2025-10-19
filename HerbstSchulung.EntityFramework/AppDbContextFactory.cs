using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HerbstSchulung.EntityFramework;

/// <summary>
/// Factory für das Erstellen von AppDbContext Instanzen mit SQL Server.
/// Bietet Methoden für normale Kontexte und Read Only Kontexte 
/// </summary>
public class AppDbContextFactory : IAppDbContextFactory
{
    private readonly string _connectionString;
    private readonly ILoggerFactory? _loggerFactory;

    /// <summary>
    /// Erstellt eine Factory-Instanz für SQL Server.
    /// </summary>
    /// <param name="connectionString">Connection String für SQL Server.</param>
    /// <param name="loggerFactory">Optionale LoggerFactory für EF Core Logging.</param>
    public AppDbContextFactory(string connectionString, ILoggerFactory? loggerFactory = null)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new ArgumentException("Connection string darf nicht leer sein.", nameof(connectionString));

        _connectionString = connectionString;
        _loggerFactory = loggerFactory;
    }

    /// <summary>
    /// Erstellt einen Standard-AppDbContext mit normalem Tracking Verhalten
    /// </summary>
    /// <returns>Eine neue AppDbContext Instan</returns>
    public AppDbContext CreateContext()
    {
        var options = BuildOptions(enableTracking: true);
        return new AppDbContext(options);
    }

    /// <summary>
    /// Erstellt einen Read-Only AppDbContext mit deaktiviertem Entity Tracking.
    /// Ideal für Lesevorgänge, bei denen keine Änderungen gespeichert werden sollen.
    /// Verbessert die Performance bei reinen Leseoperationen.
    /// Wirft eine Exception bei Versuchen, Änderungen zu speichern.
    /// </summary>
    /// <returns>Eine neue AppDbContext-Instanz mit QueryTrackingBehavior.NoTracking und IsReadOnly = true.</returns>
    public AppDbContext CreateReadOnlyContext()
    {
        var options = BuildOptions(enableTracking: false);
        return new AppDbContext(options) { IsReadOnly = true };
    }

    /// <summary>
    /// Erstellt DbContextOptions für SQL Server basierend auf der Konfiguration.
    /// </summary>
    /// <param name="enableTracking">
    /// True für normales Tracking (Standard), False für NoTracking (Read-Only).
    /// </param>
    /// <returns>Konfigurierte DbContextOptions.</returns>
    private DbContextOptions<AppDbContext> BuildOptions(bool enableTracking)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

        // LoggerFactory registrieren, falls vorhanden
        if (_loggerFactory != null)
        {
            optionsBuilder.UseLoggerFactory(_loggerFactory);
        }

        optionsBuilder.UseSqlServer(_connectionString, sqlOptions =>
        {
            // Retry-Logik für transient Fehler
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 3,
                maxRetryDelay: TimeSpan.FromSeconds(5),
                errorNumbersToAdd: null);
            
            sqlOptions.CommandTimeout(30);
        });

        // NoTracking für Read-Only Kontexte
        if (!enableTracking)
        {
            optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        }

        // Logging und Entwickleroptionen
        #if DEBUG
            // optionsBuilder.EnableSensitiveDataLogging();
            optionsBuilder.EnableDetailedErrors();
        #endif

        return optionsBuilder.Options;
    }
}
