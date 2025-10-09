# Einführung in Microsoft.Extensions.Configuration

Die Microsoft.Extensions.Configuration Bibliothek ist ein zentrales Element der modernen .NET-Entwicklung. Sie ermöglicht das flexible Einlesen und Verwalten von Konfigurationen aus verschiedenen Quellen wie JSON-Dateien, Umgebungsvariablen oder Benutzergeheimnissen.

## Grundlagen
- **Konfigurationsquellen**: Typischerweise werden Einstellungen aus `appsettings.json`, Umgebungsvariablen oder anderen Quellen geladen.
- **Hierarchische Struktur**: Einstellungen können verschachtelt und gruppiert werden.
- **Bindung an Klassen**: Mit `Options` können Konfigurationswerte direkt auf C#-Objekte abgebildet werden.
- **Validierung**: Optionen können beim Start validiert werden, um Fehler frühzeitig zu erkennen.

## Häufige Fehlerquellen
- Tippfehler in Schlüsselnamen
- Fehlende oder falsche Bindung von Options-Klassen
- Keine Validierung der Konfiguration
- Keine Dokumentation der Konfiguration
- Änderungen an der Konfiguration werden nicht erkannt (fehlendes Reload)

## Optionen-Varianten: IOptions, IOptionsSnapshot, IOptionsMonitor

| Merkmal | IOptions | IOptionsSnapshot | IOptionsMonitor |
| --- | --- | --- | --- |
| Wertänderung zur Laufzeit | Nein, Wert bleibt stabil nach dem ersten Zugriff | Nur beim neuen Scope (z. B. pro Web-Request) | Ja, liefert immer den aktuellen Wert |
| Reaktion auf Änderungen (ohne neuen Scope) | Nein | Nein | Ja, über Change-Token |
| OnChange-Callbacks | Nein | Nein | Ja, `OnChange(Action<T>)` |
| Typische Verwendung | Statische/selten geänderte Einstellungen, v. a. in Singleton-Services | Request-/Scope-bezogene Einstellungen (ASP.NET Core) | Laufzeit-dynamische Settings, Feature-Flags, Throttling, etc. |
| DI-Lebensdauer-Empfehlung | In Singleton-/Scoped-/Transient-Services verwendbar; sinnvoll v. a. in Singleton | In Scoped-/Transient-Services | Häufig in Singleton-Services |
| Performance/Overhead | Sehr gering | Gering bis moderat (pro Scope neu gebunden) | Gering; plus Event-Handling für Änderungen |

Hinweise:
- Alle drei Varianten verwenden dieselbe Bindung/Registrierung, z. B. `services.Configure<MyOptions>(configuration.GetSection("MyOptions"));`.
- Quellen wie `AddJsonFile("appsettings.json", reloadOnChange: true)` liefern Change-Tokens automatisch. `AddInMemoryCollection` liefert beim Setzen kein Change-Token; in diesem Fall muss explizit `((IConfigurationRoot)configuration).Reload()` aufgerufen werden, damit `IOptionsMonitor` seine Änderungen bemerkt.

## Zusätzliche Ressourcen
- [Microsoft-Dokumentation: Configuration in .NET](https://learn.microsoft.com/de-de/dotnet/core/extensions/configuration)
- [Microsoft-Dokumentation: Options pattern in .NET](https://learn.microsoft.com/de-de/dotnet/core/extensions/options)

