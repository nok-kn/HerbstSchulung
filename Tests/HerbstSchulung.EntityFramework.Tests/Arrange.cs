using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace HerbstSchulung.EntityFramework.Tests;

/// <summary>
/// Statische Hilfsklasse f�r das Erstellen von DbContext-Instanzen in Tests.
/// Bietet Methoden f�r InMemory- und SQL Server-Kontexte.
/// </summary>
public static class Arrange
{
    private static ServiceProvider? _serviceProvider;

    /// <summary>
    /// Erstellt einen InMemory-AppDbContext f�r schnelle Unit-Tests.
    /// InMemory ist ideal f�r logikfokussierte Tests ohne relationale Anforderungen,
    /// nutzt aber LINQ-to-Objects Semantik und l�sst inkonsistente Zust�nde durch,
    /// die in SQL scheitern w�rden.
    /// </summary>
    /// <returns>Eine neue AppDbContext-Instanz mit InMemory-Provider.</returns>
    public static AppDbContext CreateInMemoryDbContext()
    {
        var factory = new InMemoryAppDbContextFactory();
        return factory.CreateContext();
    }

    /// <summary>
    /// Erstellt einen AppDbContext basierend auf dem angegebenen Modus.
    /// </summary>
    /// <param name="inMemory">
    /// True f�r InMemory-Datenbank (schnelle Unit-Tests),
    /// False f�r SQL Server (Integration-Tests mit relationaler Datenbank).
    /// </param>
    /// <returns>Eine neue AppDbContext-Instanz.</returns>
    public static AppDbContext CreateDbContext(bool inMemory)
    {
        if (inMemory)
        {
            // InMemory ist super f�r schnelle, logikfokussierte Unit-Tests ohne relationale Anforderungen
            // aber InMemory nutzt LINQ-to-Objects Semantik und l�sst inkonsistente Zust�nde durch, die in SQL scheitern w�rden
            return CreateInMemoryDbContext();
        }
        else
        {
            // sobald Relationalit�t, SQL, Constraints oder komplexe Abfragen eine Rolle spielen (also fast immer :) ) , nimm eine echte DB oder Testcontainer
            

            // wir verwend DI um den DbContext zu erstellen, damit Logging funktionieren kann
            if (_serviceProvider == null)
            {
                // Konfiguration aus appsettings.json laden
                var configuration = GetConfiguration();

                // ServiceCollection f�r DI erstellen
                var services = new ServiceCollection();

                // Logging mit Konfiguration aus appsettings.json registrieren
                services.AddLogging(builder =>
                {
                    builder.AddConfiguration(configuration.GetSection("Logging"));
                    builder.AddConsole();
                });
                
                services.AddHerbstSchulungPersistence(configuration);


                _serviceProvider = services.BuildServiceProvider();
            }

            // AppDbContextFactory aus DI-Container aufl�sen
            var factory = _serviceProvider.GetRequiredService<IAppDbContextFactory>();
            return factory.CreateContext();
        }
    }

    internal static IConfigurationRoot GetConfiguration()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
            .Build();
        return configuration;
    }

    internal static string CetConnectionString()
    {
        var configuration = GetConfiguration();
        var connectionString = configuration.GetConnectionString("Default");
        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException("Connection string 'Default' not found in configuration.");
        }
        return connectionString;
    }
}
