# Dependency Injection (DI) Grundlagen in .NET

## Was ist Dependency Injection?
Dependency Injection (DI) ist ein Muster zur Entkopplung von Komponenten. Abh�ngigkeiten (z. B. Services) werden nicht direkt innerhalb einer Klasse erzeugt, sondern von au�en bereitgestellt. Das erh�ht Testbarkeit, Austauschbarkeit und Wartbarkeit.

## Vorteile
- Testbarkeit: Leichtes Mocking von Interfaces
- Austauschbarkeit: Implementierungen k�nnen zur Laufzeit / Konfiguration gewechselt werden
- Single Responsibility: Klassen fokussieren sich auf ihre Aufgabe, nicht auf Objektaufbau
- Wartbarkeit & Erweiterbarkeit: Neue Implementierungen ohne invasive �nderungen

## Kernkonzepte im .NET DI Container (Microsoft.Extensions.DependencyInjection)
1. Registrierung: Zuweisung von Service-Typ (Interface) zu Implementierung
2. Aufl�sung (Resolve): Container erstellt Instanzen und injiziert Abh�ngigkeiten (Konstruktor bevorzugt)
3. Lebenszyklen (Lifetimes): Bestimmen, wie lange Instanzen leben

## Lebenszyklen
- Transient: Neue Instanz bei jedem Resolve
- Scoped: Eine Instanz pro Scope (z. B. Web-Request). In Console/Tests manuell �ber `CreateScope()`.
- Singleton: Eine Instanz f�r die gesamte Lebensdauer des Root-Providers

## Best Practices
- Verwende Konstruktor-Injektion (klar, explizit, testbar)
- Keine Service-Locator / `IServiceProvider`-Abh�ngigkeiten in normalen Klassen
- Vermeide zirkul�re Abh�ngigkeiten (Hinweis auf schlechtes Design)
- Halte Services stateless oder verstehe die Konsequenzen von State
- Nutze Interfaces nur bei Bedarf (Austauschbarkeit / Tests)
- Nutze Options Pattern f�r Konfiguration (`IOptions<T>`, `IOptionsSnapshot<T>`)

## Factory / Delegierte Registrierungen
```csharp
services.AddSingleton<IMeinService>(sp => new MeinService(DateTime.UtcNow));
```
Nutze dies f�r Feinsteuerung beim Aufbau (z. B. Zugriff auf andere Services oder Konfiguration).

## Keyed / Named Services (.NET 8)
```csharp
services.AddKeyedSingleton<INotifier, EmailNotifier>("email");
var email = provider.GetRequiredKeyedService<INotifier>("email");
```
Erm�glicht mehrere Implementierungen desselben Interfaces differenziert abzurufen.

## H�ufige Fehler
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
Problem: Versteckte Abh�ngigkeiten, schwieriges Testen.

## Weiterf�hrende Themen
- Options Pattern / Konfiguration
- Hosted Services / BackgroundService
- Generische Typen (z. B. `IRepository<T>`)
- Dekoratoren (manuell oder via Wrapper Registrierungen)
- Pipelines / Middleware

## Zus�tzliche Ressourcen
- Microsoft Docs: https://learn.microsoft.com/dotnet/core/extensions/dependency-injection
- Lebenszyklen: https://learn.microsoft.com/dotnet/core/extensions/dependency-injection#service-lifetimes
- Options Pattern: https://learn.microsoft.com/dotnet/core/extensions/options
- Keyed Services (.NET 8): https://learn.microsoft.com/dotnet/core/extensions/dependency-injection#keyed-services
- Blog: https://andrewlock.net (Viele tiefgehende Artikel zu DI in .NET)
- GitHub Beispiele: https://github.com/dotnet/runtime/tree/main/src/libraries/Microsoft.Extensions.DependencyInjection
