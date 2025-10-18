using Microsoft.EntityFrameworkCore;

namespace HerbstSchulung.EntityFramework.Tests;

/// <summary>
/// InMemory Implementierung der IAppDbContextFactory f�r Unit Tests
/// </summary>
public class InMemoryAppDbContextFactory : IAppDbContextFactory
{
    private readonly string _databaseName;
    private readonly bool _sharedDatabase;

    /// <summary>
    /// Erstellt eine Factory f�r isolierte InMemory Datenbanken (jeder Kontext erh�lt eine neue DB).
    /// </summary>
    public InMemoryAppDbContextFactory() : this(sharedDatabase: false, databaseName: null)
    {
    }

    /// <summary>
    /// Erstellt eine Factory mit optionaler gemeinsamer Datenbank.
    /// </summary>
    /// <param name="sharedDatabase">True f�r gemeinsame DB �ber alle Kontexte, False f�r isolierte DBs.</param>
    /// <param name="databaseName">Name der Datenbank (optional, wird bei shared ben�tigt).</param>
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
    /// Erstellt DbContextOptions f�r InMemory-Provider.
    /// </summary>
    /// <param name="enableTracking">
    /// True f�r normales Tracking (Standard), False f�r NoTracking (Read-Only).
    /// </param>
    /// <returns>Konfigurierte DbContextOptions.</returns>
    private DbContextOptions<AppDbContext> BuildOptions(bool enableTracking)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

        // F�r isolierte Tests: Jeder Kontext erh�lt eine neue Datenbank
        var dbName = _sharedDatabase ? _databaseName : Guid.NewGuid().ToString();
        optionsBuilder.UseInMemoryDatabase(dbName);

        // NoTracking f�r Read-Only Kontexte
        if (!enableTracking)
        {
            optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        }

        // Entwickleroptionen
        optionsBuilder.EnableDetailedErrors();

        return optionsBuilder.Options;
    }

    /// <summary>
    /// Erstellt eine Factory-Instanz f�r gemeinsame InMemory-Datenbank.
    /// Alle erstellten Kontexte teilen sich die gleiche Datenbank.
    /// N�tzlich wenn mehrere Kontexte auf die gleichen Daten zugreifen sollen.
    /// </summary>
    /// <param name="databaseName">Name der gemeinsamen Datenbank.</param>
    /// <returns>Eine neue Factory-Instanz f�r gemeinsame DB.</returns>
    public static InMemoryAppDbContextFactory CreateShared(string? databaseName = null)
    {
        return new InMemoryAppDbContextFactory(sharedDatabase: true, databaseName: databaseName ?? Guid.NewGuid().ToString());
    }
}
