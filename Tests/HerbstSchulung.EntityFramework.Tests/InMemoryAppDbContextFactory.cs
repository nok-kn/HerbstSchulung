using Microsoft.EntityFrameworkCore;

namespace HerbstSchulung.EntityFramework.Tests;

/// <summary>
/// InMemory Implementierung der IAppDbContextFactory für Unit Tests
/// </summary>
public class InMemoryAppDbContextFactory : IAppDbContextFactory
{
    private readonly string _databaseName;
    private readonly bool _sharedDatabase;

    /// <summary>
    /// Erstellt eine Factory für isolierte InMemory Datenbanken (jeder Kontext erhält eine neue DB).
    /// </summary>
    public InMemoryAppDbContextFactory() : this(sharedDatabase: false, databaseName: null)
    {
    }

    /// <summary>
    /// Erstellt eine Factory mit optionaler gemeinsamer Datenbank.
    /// </summary>
    /// <param name="sharedDatabase">True für gemeinsame DB über alle Kontexte, False für isolierte DBs.</param>
    /// <param name="databaseName">Name der Datenbank (optional, wird bei shared benötigt).</param>
    public InMemoryAppDbContextFactory(bool sharedDatabase, string? databaseName = null)
    {
        _sharedDatabase = sharedDatabase;
        _databaseName = databaseName ?? Guid.NewGuid().ToString();
    }

    /// <summary>
    /// Erstellt einen Standard-AppDbContext mit normalem Tracking-Verhalten.
    /// </summary>
    /// <returns>Eine neue AppDbContext-Instanz mit InMemory-Provider.</returns>
    public AppDbContext CreateContext()
    {
        var options = BuildOptions(enableTracking: true);
        return new AppDbContext(options);
    }

    /// <summary>
    /// Erstellt einen Read-Only AppDbContext mit deaktiviertem Entity Tracking.
    /// </summary>
    /// <returns>Eine neue AppDbContext-Instanz mit QueryTrackingBehavior.NoTracking.</returns>
    public AppDbContext CreateReadOnlyContext()
    {
        var options = BuildOptions(enableTracking: false);
        return new AppDbContext(options);
    }

    /// <summary>
    /// Erstellt DbContextOptions für InMemory-Provider.
    /// </summary>
    /// <param name="enableTracking">
    /// True für normales Tracking (Standard), False für NoTracking (Read-Only).
    /// </param>
    /// <returns>Konfigurierte DbContextOptions.</returns>
    private DbContextOptions<AppDbContext> BuildOptions(bool enableTracking)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

        // Für isolierte Tests: Jeder Kontext erhält eine neue Datenbank
        var dbName = _sharedDatabase ? _databaseName : Guid.NewGuid().ToString();
        optionsBuilder.UseInMemoryDatabase(dbName);

        // NoTracking für Read-Only Kontexte
        if (!enableTracking)
        {
            optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        }

        // Entwickleroptionen
        optionsBuilder.EnableDetailedErrors();

        return optionsBuilder.Options;
    }

    /// <summary>
    /// Erstellt eine Factory-Instanz für gemeinsame InMemory-Datenbank.
    /// Alle erstellten Kontexte teilen sich die gleiche Datenbank.
    /// Nützlich wenn mehrere Kontexte auf die gleichen Daten zugreifen sollen.
    /// </summary>
    /// <param name="databaseName">Name der gemeinsamen Datenbank.</param>
    /// <returns>Eine neue Factory-Instanz für gemeinsame DB.</returns>
    public static InMemoryAppDbContextFactory CreateShared(string? databaseName = null)
    {
        return new InMemoryAppDbContextFactory(sharedDatabase: true, databaseName: databaseName ?? Guid.NewGuid().ToString());
    }
}
