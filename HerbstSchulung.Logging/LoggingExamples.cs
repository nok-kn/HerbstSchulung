using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace HerbstSchulung.Logging;

/// <summary>
/// Beispiele für den Einsatz von Microsoft.Extensions.Logging in .NET.
/// </summary>
public class LoggingExamples
{
    private readonly ILogger<LoggingExamples> _logger;

    // ILogger wird typisiert injiziert. In echten Apps über DI (ServiceCollection) konfigurieren.
    public LoggingExamples(ILogger<LoggingExamples> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Registrierung des Loggings über DI. Zeigt die Verwendung von AddLogging, Filter und Providern.
    /// </summary>
    public static ServiceProvider BuildServiceProviderMitLogging()
    {
        var services = new ServiceCollection();

        // AddLogging registriert ILoggerFactory, ILogger<T> und Standard-Provider.
        services.AddLogging(builder =>
        {
            // Mindest-Level setzen. Hier: Information und höher.
            builder.SetMinimumLevel(LogLevel.Information);

            // Console-Provider hinzufügen.
            builder.AddConsole();

            // Beispiel für Filter: Kategorie- oder Provider-spezifische Filter setzen.
            builder.AddFilter("HerbstSchulung.Logging", LogLevel.Debug);
            // builder.AddFilter<ConsoleLoggerProvider>("Microsoft", LogLevel.Warning);
        });

        // Beispiel-Dienst registrieren, der ILogger<LoggingExamples> verwendet
        services.AddTransient<LoggingExamples>();

        return services.BuildServiceProvider();
    }

    /// <summary>
    /// Demonstriert die verschiedenen Logging-Level 
    /// </summary>
    public void LogLevelsDemo()
    {
        // LogLevel: Trace < Debug < Information < Warning < Error < Critical < None
        _logger.LogTrace("Trace: sehr detaillierte Diagnose-Informationen, meist nur lokal nützlich.");
        _logger.LogDebug("Debug: Debug-Informationen für Entwickler.");
        _logger.LogInformation("Information: Normaler Ablauf, wichtige Ereignisse.");
        _logger.LogWarning("Warning: Unerwartete Situation, das System läuft aber weiter.");
        _logger.LogError("Error: Fehler, eine Operation ist fehlgeschlagen.");
        _logger.LogCritical("Critical: Schwerwiegender Fehler, System evtl. nicht benutzbar.");
    }

    /// <summary>
    /// Demonstriert die Nutzung von Scopes, um zusammenhängende Ereignisse zu korrelieren.
    /// </summary>
    public void ScopesDemo()
    {
        using (_logger.BeginScope("Bestellvorgang {OrderId}", 12345))
        {
            _logger.LogInformation("Starte Bestellvorgang");

            using (_logger.BeginScope(new Dictionary<string, object>
            {
                ["KundeId"] = 987,
                ["Region"] = "DE"
            }))
            {
                _logger.LogInformation("Artikel prüfen");
                _logger.LogInformation("Zahlung ausführen");
            }

            _logger.LogInformation("Bestellvorgang abgeschlossen");
        }
    }

    /// <summary>
    /// Semantisches Logging: strukturiertes Logging mit benannten Platzhaltern. Diese erscheinen als Schlüssel/Werte in strukturierten Sinks z.B. Serilog, Seq
    /// </summary>
    public void SemantischesLogging()
    {
        var userId = 42;
        var feature = "Export";
        _logger.LogInformation("User {UserId} hat Feature {Feature} gestartet", userId, feature);

        try
        {
            WerfeBeispielFehler();
        }
        catch (Exception ex)
        {
            // Exceptions immer als erstes Argument übergeben, damit StackTrace erhalten bleibt.
            _logger.LogError(ex, "Fehler beim Ausführen von Feature {Feature} für User {UserId}", feature, userId);

            // nicht so:
            _logger.LogError("Fehler beim Ausführen von Feature");
        }
    }

    private static void WerfeBeispielFehler()
    {
        throw new InvalidOperationException("Nur ein Demo-Fehler");
    }

    /// <summary>
    /// Nutzung einer LoggerFactory ohne DI – z. B. in einfachen Konsolen-Tools oder Tests.
    /// </summary>
    public static void LoggerFactoryOhneDI()
    {
        using ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
        {
            builder
                .SetMinimumLevel(LogLevel.Debug)
                .AddConsole();
        });

        var logger = loggerFactory.CreateLogger("BeispielKategorie");
        logger.LogInformation("Hallo aus LoggerFactory ohne DI");
    }
}

// Benutze ILoggerFactory in Basisklassen, um Logger für abgeleitete Klassen zu erstellen.
public abstract class BaseClass
{
    protected ILoggerFactory LoggerFactory { get; }
    protected ILogger Logger { get; }


    protected BaseClass(ILoggerFactory loggerFactory)
    {
        LoggerFactory = loggerFactory;
        Logger = LoggerFactory.CreateLogger(GetType());
    }
}

public class DerivedClassA : BaseClass
{
    // nicht notwendig, wenn ILoggerFactory genutzt wird
    // public DerivedClassA(ILogger<DerivedClassA> logger) {}

    public DerivedClassA(ILoggerFactory loggerFactory) : base(loggerFactory)
    {
    }
}

public class DerivedClassB : BaseClass
{
    public DerivedClassB(ILoggerFactory loggerFactory) : base(loggerFactory)
    {
    }
}
