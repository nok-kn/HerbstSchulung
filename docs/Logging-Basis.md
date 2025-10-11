# Logging mit Microsoft.Extensions

---

## Ziele
- Einheitliches Logging über Bibliotheken und Anwendungen hinweg (ILogger, ILoggerFactory)
- Verständliche, strukturierte Logs (semantisches Logging)
- Konfiguration über Kategorien, Level und Filter

## Kernkonzepte
- ILogger und ILogger<T>: Typisierte Logger pro Komponente/Klasse
- ILoggerFactory: Erzeugt Logger-Instanzen, konfiguriert Provider
- LogLevel: Trace, Debug, Information, Warning, Error, Critical
- Provider: Console, Debug, EventSource, Azure, Serilog, usw.
- Scopes: Korrelation von Logeinträgen (z. B. RequestId, OrderId)
- Strukturierte Werte: {Name} Platzhalter erzeugen Key/Value-Paare

## Best Practices
- LogLevel bewusst wählen: Information für Normalbetrieb, Debug/Trace nur für detaillierte Analyse
- Exceptions immer als erstes Argument loggen (preserves StackTrace)
- Keine sensiblen Daten (PII/Secrets) loggen; Maskieren wo nötig
- Einheitliche Kategorien verwenden (ILogger<T>)
- Scopes für Geschäfts-IDs und Korrelation verwenden
- LoggerMessage oder Source Generator nutzen für Hot Paths
- Filter per Kategorie/Namespace steuern (AddFilter)

## Häufige Fehler
- String-Interpolation statt strukturierter Platzhalter verwenden
- Logging in Tight-Loops ohne Level-Check (if logger.IsEnabled(...))
- Fehlende Korrelation (keine Scopes)
- Zu niedrige oder zu hohe Mindestlevel → Rauschen oder Blindflug

## Konfigurationstipps
- Produktions- vs. Entwicklungsprofile: unterschiedliche Mindestlevel
- Kategorie-Filter: z. B. Microsoft.* auf Warning setzen
- Provider-spezifische Einstellungen (Console-Format, Zeitstempel, Farben)

## Weiterführende Ressourcen
- Microsoft Docs: Logging in .NET
  https://learn.microsoft.com/dotnet/core/extensions/logging
- ILogger und LoggerFactory API Referenz
  https://learn.microsoft.com/dotnet/api/microsoft.extensions.logging
- Logging-Anbieter (Provider) Übersicht
  https://learn.microsoft.com/dotnet/core/extensions/logging-providers
- Structured Logging und Best Practices (Offizieller Blog)
  https://devblogs.microsoft.com/dotnet/net-core-logging/ 
- LoggerMessage und Source Generator
  https://learn.microsoft.com/dotnet/core/extensions/logger-message-generator
- Serilog (beliebte Lib für semantische Logging)
  https://serilog.net/
- OpenTelemetry .NET (Distributed Tracing + Logs)
  https://opentelemetry.io/docs/instrumentation/net/


