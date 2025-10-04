# Dependency Injection (DI) Grundlagen in .NET

## Was ist Dependency Injection?
Dependency Injection (DI) ist ein Muster zur Entkopplung von Komponenten. Abhängigkeiten (z. B. Services) werden nicht direkt innerhalb einer Klasse erzeugt, sondern von außen bereitgestellt. Das erhöht Testbarkeit, Austauschbarkeit und Wartbarkeit.

## Vorteile
- Testbarkeit: Leichtes Mocking von Interfaces
- Austauschbarkeit: Implementierungen können zur Laufzeit / Konfiguration gewechselt werden
- Single Responsibility: Klassen fokussieren sich auf ihre Aufgabe, nicht auf Objektaufbau
- Wartbarkeit & Erweiterbarkeit: Neue Implementierungen ohne invasive Änderungen

## Kernkonzepte im .NET DI Container (Microsoft.Extensions.DependencyInjection)
1. Registrierung: Zuweisung von Service-Typ (Interface) zu Implementierung
2. Auflösung (Resolve): Container erstellt Instanzen und injiziert Abhängigkeiten (Konstruktor bevorzugt)
3. Lebenszyklen (Lifetimes): Bestimmen, wie lange Instanzen leben

## Lebenszyklen
- Transient: Neue Instanz bei jedem Resolve
- Scoped: Eine Instanz pro Scope (z. B. Web-Request). In Console/Tests manuell über `CreateScope()`.
- Singleton: Eine Instanz für die gesamte Lebensdauer des Root-Providers

## Best Practices
- Verwende Konstruktor-Injektion (klar, explizit, testbar)
- Keine Service-Locator / `IServiceProvider`-Abhängigkeiten in normalen Klassen
- Vermeide zirkuläre Abhängigkeiten (Hinweis auf schlechtes Design)
- Halte Services stateless oder verstehe die Konsequenzen von State
- Nutze Interfaces nur bei Bedarf (Austauschbarkeit / Tests)
- Nutze Options Pattern für Konfiguration (`IOptions<T>`, `IOptionsSnapshot<T>`)

## Factory / Delegierte Registrierungen
```csharp
services.AddSingleton<IMeinService>(sp => new MeinService(DateTime.UtcNow));
```
Nutze dies für Feinsteuerung beim Aufbau (z. B. Zugriff auf andere Services oder Konfiguration).

## Keyed / Named Services (.NET 8)
```csharp
services.AddKeyedSingleton<INotifier, EmailNotifier>("email");
var email = provider.GetRequiredKeyedService<INotifier>("email");
```
Ermöglicht mehrere Implementierungen desselben Interfaces differenziert abzurufen.

## Häufige Fehler
- Falscher Lifetime (z. B. teure Dienste als Transient)
- Capturing eines Scoped Service in Singleton-Konstruktor
- Verwendung von `BuildServiceProvider()` mehrfach (Root-Container sollte einmal gebaut werden)
- Manuelle `new`-Erzeugungen statt Injektion

## Anti-Pattern Service Locator
```csharp
public class Bad
{
    private readonly IServiceProvider _sp;
    public Bad(IServiceProvider sp) => _sp = sp;
    public void Handle() => _sp.GetService(typeof(IMyService));
}
```
Problem: Versteckte Abhängigkeiten, schwieriges Testen.

## Weiterführende Themen
- Options Pattern / Konfiguration
- Hosted Services / BackgroundService
- Generische Typen (z. B. `IRepository<T>`)
- Dekoratoren (manuell oder via Wrapper Registrierungen)
- Pipelines / Middleware

## Zusätzliche Ressourcen
- Microsoft Docs: https://learn.microsoft.com/dotnet/core/extensions/dependency-injection
- Lebenszyklen: https://learn.microsoft.com/dotnet/core/extensions/dependency-injection#service-lifetimes
- Options Pattern: https://learn.microsoft.com/dotnet/core/extensions/options
- Keyed Services (.NET 8): https://learn.microsoft.com/dotnet/core/extensions/dependency-injection#keyed-services
- Blog: https://andrewlock.net (Viele tiefgehende Artikel zu DI in .NET)
- GitHub Beispiele: https://github.com/dotnet/runtime/tree/main/src/libraries/Microsoft.Extensions.DependencyInjection
