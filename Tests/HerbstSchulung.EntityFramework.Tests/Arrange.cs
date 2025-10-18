using HerbstSchulung.EntityFramework.DesignTime;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace HerbstSchulung.EntityFramework.Tests;

/// <summary>
/// Statische Hilfsklasse für das Erstellen von DbContext-Instanzen in Tests.
/// Bietet Methoden für InMemory- und SQL Server-Kontexte.
/// </summary>
public static class Arrange
{
    private static AppDbContextFactory? _contextFactory;

    /// <summary>
    /// Erstellt einen InMemory-AppDbContext für schnelle Unit-Tests.
    /// InMemory ist ideal für logikfokussierte Tests ohne relationale Anforderungen,
    /// nutzt aber LINQ-to-Objects Semantik und lässt inkonsistente Zustände durch,
    /// die in SQL scheitern würden.
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
    /// True für InMemory-Datenbank (schnelle Unit-Tests),
    /// False für SQL Server (Integration-Tests mit relationaler Datenbank).
    /// </param>
    /// <returns>Eine neue AppDbContext-Instanz.</returns>
    public static AppDbContext CreateDbContext(bool inMemory)
    {
        if (inMemory)
        {
            // InMemory ist super für schnelle, logikfokussierte Unit-Tests ohne relationale Anforderungen
            // aber InMemory nutzt LINQ-to-Objects Semantik und lässt inkonsistente Zustände durch, die in SQL scheitern würden
            return CreateInMemoryDbContext();
        }
        else
        {
            // sobald Relationalität, SQL, Constraints oder komplexe Abfragen eine Rolle spielen (also fast immer :) ) , nimm eine echte DB oder Testcontainer
            // benutze [Trait("Category", "Integration")] um Integrationstests zu markieren
            
            if (_contextFactory == null)
            {
                // Konfiguration aus appsettings.json laden
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                    .Build();

                var connectionString = configuration.GetConnectionString("DefaultConnection");

                if (string.IsNullOrEmpty(connectionString))
                {
                    throw new InvalidOperationException("ConnectionString 'DefaultConnection' nicht in appsettings.json gefunden.");
                }

                _contextFactory = new AppDbContextFactory(connectionString);
            }

            return _contextFactory.CreateContext();
        }
    }
}
