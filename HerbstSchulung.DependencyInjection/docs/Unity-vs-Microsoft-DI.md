# Vergleich: Unity Container vs. Microsoft.Extensions.DependencyInjection (MS.DI)

## �berblick
| Aspekt | Unity | Microsoft DI |
|--------|-------|--------------|
| Schwerpunkt | Funktionsreicher IoC Container | Einfacher, minimalistischer Default-Container |
| Performance | Solide, teilweise langsamer bei komplexen Szenarien | Fokus auf Performance & niedrigen Overhead |
| Erweiterbarkeit | Umfangreich (Interception, Child Container, Policies) | Basis-Features, Erweiterung �ber Drittanbieter oder eigene Logik |
| Registrierung von Open Generics | Ja | Ja |
| Namens-/Key-Unterst�tzung | Ja (Named Registrations) | Seit .NET 8: Keyed Services |
| Lifetime Management | Sehr flexibel | Transient, Scoped, Singleton (ohne Hierarchie) |
| Interception / AOP | Unterst�tzt | Nicht nativ (Workaround: Dekoratoren / Proxy libs) |
| Child Container | Ja | Nein (nur Scopes) |
| Konfiguration | Fluent API | Fluent Methoden-Erweiterungen |
| Lernkurve | H�her (mehr Features) | Gering (bewusste Einfachheit) |

## Wann Unity?
- Bedarf an Interception / AOP out-of-the-box
- Notwendig: Child Container Hierarchien
- Bereits bestehende Investition / Legacy Code nutzt Unity intensiv

## Wann Microsoft DI?
- Moderne .NET Apps (ASP.NET Core, Worker Services) als Standard
- Fokus auf Klarheit, Einfachheit und Integration ins �kosystem
- Performance-kritische Anwendungen ohne komplexe Container-Features

## Typische Unterschiede
1. Feature-Umfang: Unity bietet mehr eingebaute M�glichkeiten, MS.DI setzt auf Minimalismus.
2. Erweiterung: Bei MS.DI l�sbar �ber zus�tzliche Libraries (Scrutor f�r Scanning/Dekoratoren).
3. Interception: In MS.DI nicht vorgesehen -> Alternativen: Source Generator, Decorator Pattern, DynamicProxy (Castle).
4. Child Container vs. Scope: MS.DI verwendet Scopes als leichtgewichtige Alternative zu Container-Hierarchien.

## Beispiel: Keyed / Named Services
Unity:
```csharp
container.RegisterType<INotifier, EmailNotifier>("email");
var email = container.Resolve<INotifier>("email");
```
Microsoft DI (.NET 8):
```csharp
services.AddKeyedSingleton<INotifier, EmailNotifier>("email");
var email = provider.GetRequiredKeyedService<INotifier>("email");
```

## Migrationshinweise von Unity zu MS.DI
- Pr�fen welche Unity-spezifischen Features (Interception, Child Container) genutzt werden
- Dekoratoren statt Interception einsetzen
- Scopes statt Child Container
- Eigene Factories / delegierte Registrierungen nutzen
- F�r komplexe Szenarien ggf. spezialisierten Container (Autofac / Lamar) evaluieren

## Zus�tzliche Ressourcen
- MS Docs DI: https://learn.microsoft.com/dotnet/core/extensions/dependency-injection
- Scrutor (Assembly Scanning, Dekoratoren): https://github.com/khellang/Scrutor
- Unity Container GitHub: https://github.com/unitycontainer/unity
- Performance Vergleich (Blog von Steven / Simple Injector): https://blog.ploeh.dk (Allgemeine DI Diskussionen)
