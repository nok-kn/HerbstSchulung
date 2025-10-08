# Dependency Injection (DI) Grundlagen in .NET

## Was ist Dependency Injection?
Dependency Injection (DI) ist ein Muster zur Entkopplung von Komponenten. Abhängigkeiten (z. B. Services) werden nicht direkt innerhalb einer Klasse erzeugt, sondern von außen bereitgestellt. Das erhöht Testbarkeit, Austauschbarkeit und Wartbarkeit.

## Vorteile
- Testbarkeit: Leichtes Mocking von Interfaces
- Austauschbarkeit: Implementierungen können zur Laufzeit / Konfiguration gewechselt werden
- Single Responsibility: Klassen fokussieren sich auf ihre Aufgabe, nicht auf Objektaufbau
- Wartbarkeit & Erweiterbarkeit: Neue Implementierungen ohne invasive Änderungen

### Warum DI Tests verbessert
- **Isolation**: Services können durch Mocks/Stubs ersetzt werden
- **Kontrolle**: Testbare Abhängigkeiten (Zeit, Random, HTTP-Calls)
- **Nachvollziehbarkeit**: Explizite Abhängigkeiten im Konstruktor

## Kernkonzepte im .NET DI Container (Microsoft.Extensions.DependencyInjection)
1. Registrierung: Zuweisung von Service-Typ (Interface) zu Implementierung
2. Auflösung (Resolve): Container erstellt Instanzen und injiziert Abhängigkeiten (Konstruktor bevorzugt)
3. Lebenszyklen (Lifetimes): Bestimmen, wie lange Instanzen leben

## Lebenszyklen
- Transient: Neue Instanz bei jedem Resolve
- Scoped: Eine Instanz pro Scope (z. B. Web-Request). In Console/Tests manuell über `CreateScope()`.
- Singleton: Eine Instanz für die gesamte Lebensdauer des Root-Providers

## Best Practices
- Verwende Konstruktor-Injektion (klar, explizit, testbar). Vermeide Property Injection (manchmal notwendig z.B Blazor bevor .NET 9))
- Keine Service-Locator / `IServiceProvider`-Abhängigkeiten in normalen Klassen
- Vermeide zirkuläre Abhängigkeiten (Hinweis auf schlechtes Design)
- Halte Services stateless oder verstehe die Konsequenzen von State
- Statt static Klassen, nutze DI mit Singleton Registrierung
- Nutze Interfaces nur bei Bedarf (Austauschbarkeit / Tests)
- Nutze Options Pattern für Konfiguration (`IOptions<T>`, `IOptionsSnapshot<T>`)
- Am besten alle Abhängigkeiten explizit registrieren (kein automatisches Scannen nach allen möglichen Klassen)


## Häufige Fehler
- Falscher Lifetime (z. B. teure Dienste als Transient)
- Capturing eines Scoped Service in Singleton-Konstruktor
- Verwendung von `BuildServiceProvider()` mehrfach (Root-Container sollte einmal gebaut werden)
- Manuelle `new`-Erzeugungen statt Injektion


## Zusätzliche Ressourcen
- Microsoft Docs: https://learn.microsoft.com/dotnet/core/extensions/dependency-injection
- Lebenszyklen: https://learn.microsoft.com/dotnet/core/extensions/dependency-injection#service-lifetimes
- Options Pattern: https://learn.microsoft.com/dotnet/core/extensions/options
- Keyed Services (.NET 8): https://learn.microsoft.com/dotnet/core/extensions/dependency-injection#keyed-services
- Testing with DI: https://learn.microsoft.com/aspnet/core/fundamentals/dependency-injection#testing-with-dependency-injection
- Buch "Dependency Injection Principles, Practices, and Patterns"  Mark Seemann, Steven van Deursen
- Scrutor (Assembly Scanning, Dekoratoren): https://github.com/khellang/Scrutor

