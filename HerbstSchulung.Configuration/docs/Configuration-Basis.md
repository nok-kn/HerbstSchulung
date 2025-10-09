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
- Änderungen an der Konfiguration werden nicht erkannt (fehlendes Reload)


## Zusätzliche Ressourcen
- [Microsoft-Dokumentation: Configuration in .NET](https://learn.microsoft.com/de-de/dotnet/core/extensions/configuration)
- [Microsoft-Dokumentation: Options pattern in .NET](https://learn.microsoft.com/de-de/dotnet/core/extensions/options)
- [Blog: Best Practices for .NET Configuration](https://andrewlock.net/series/configuration-in-dotnet/)

