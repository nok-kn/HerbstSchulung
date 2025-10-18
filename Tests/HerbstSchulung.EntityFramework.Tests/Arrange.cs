using HerbstSchulung.EntityFramework.DesignTime;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace HerbstSchulung.EntityFramework.Tests;

/// <summary>
/// Statische Hilfsklasse f�r das Erstellen von DbContext-Instanzen in Tests.
/// Bietet Methoden f�r InMemory- und SQL Server-Kontexte.
/// </summary>
public static class Arrange
{
    private static AppDbContextFactory? _contextFactory;

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
