# Herbstschulung

@2025 Krzysztof Nowak - http://www.nowak-it.com

## Kurzbeschreibung
- Trainings- und Beispiel-Repository für moderne .NET 8 - 10 Themen
- Die Beispiele sind kompakt gehalten 
- Einige Projekte enthalten zusätzliche Dokumentation im jeweiligen docs-Ordner

## Projekte im Überblick

### [HerbstSchulung.CSharpFeatures](HerbstSchulung.CSharpFeatures)
- Beispiele zu Nullable Reference Types, Records, Default Interface Methods, Pattern Matching, Top-level Statements, File-scoped Namespaces, Target-typed new, Init-only Properties, etc. 
- [Dokumentation: C# Features](HerbstSchulung.CSharpFeatures/docs/csharp-features.md)

### [HerbstSchulung.Async](HerbstSchulung.Async)
- Async/await-Beispiele und Anti-Patterns
- Begleitende Unit-Tests im Projekt [HerbstSchulung.Async.Tests](Tests/HerbstSchulung.Async.Tests)
- [Dokumentation: Async Einführung](HerbstSchulung.Async/docs/Async-Einfuehrung.md)

### [HerbstSchulung.Configuration](HerbstSchulung.Configuration)
- Beispiele für Microsoft.Extensions.Configuration und Options Pattern
- IOptions, IOptionsSnapshot, IOptionsMonitor und Validierung (DataAnnotations, benutzerdefiniert)
- appsettings.json sowie In-Memory-Konfiguration
- [Dokumentation: Configuration Basis](HerbstSchulung.Configuration/docs/Configuration-Basis.md)

### [HerbstSchulung.DependencyInjection](HerbstSchulung.DependencyInjection)
- Beispiele und Übungen zu Microsoft.Extensions.DependencyInjection
- Lebensdauern (Transient/Scoped/Singleton), ServiceCollection-Erweiterungen
- [Dokumentation: DependencyInjection Basis](HerbstSchulung.DependencyInjection/docs/DependencyInjection-Basis.md)
- [Dokumentation: Unity vs Microsoft DI](HerbstSchulung.DependencyInjection/docs/Unity-vs-Microsoft-DI.md)

### [HerbstSchulung.Logging](HerbstSchulung.Logging)
- Beispiele zu Microsoft.Extensions.Logging, Log-Level, Kategorien, strukturierte Logs
- [Dokumentation: Logging Basis](HerbstSchulung.Logging/docs/Logging-Basis.md)

### [HerbstSchulung.Hosting](HerbstSchulung.Hosting)
- Generischer Host, BackgroundService/Worker (TimeLoggingWorker)
- Integration von Services und Konfiguration
- [Dokumentation: Hosting Basis](HerbstSchulung.Hosting/docs/Hosting-Basis.md)

### [HerbstSchulung.Hosting.Abstractions](HerbstSchulung.Hosting.Abstractions)
- Abstraktionen/Contracts für Besipiel-Services

### [HerbstSchulung.Hosting.Services](HerbstSchulung.Hosting.Services)
- Service-Implementierungen und DI Registrierung

### [HerbstSchulung.EntityFramework](HerbstSchulung.EntityFramework)
- EF Core Beispiele und Patterns
- Entity Triggers, TPH/TPC/TPT, Value Objects, Bulk Operations
- Integration Tests mit Respawn  
- Tests: [HerbstSchulung.EntityFramework.Tests](Tests/HerbstSchulung.EntityFramework.Tests)
- [Dokumentation: EF Basis](HerbstSchulung.EntityFramework/docs/EF-Basis.md)
- [Dokumentation: EF Async](HerbstSchulung.EntityFramework/docs/EF-Async.md)
- [Dokumentation: EF Inheritance](HerbstSchulung.EntityFramework/docs/EF-Inheritance.md)
- [Dokumentation: EF Cascade Delete](HerbstSchulung.EntityFramework/docs/EF-Cascade-Delete.md)
- [Dokumentation: EF Split Queries](HerbstSchulung.EntityFramework/docs/EF-SplitQueries.md)

### [HerbstSchulung.WebApi](HerbstSchulung.WebApi)
- ASP.NET Core Web API mit Swagger, Health Checks, ProblemDetails
- Docker Support 
- [Dokumentation: WebApi Basis und Advanced](HerbstSchulung.WebApi/docs/WebApi-Basis-Und-Advanced.md)

### [HerbstSchulung.RestClients](HerbstSchulung.RestClients)
- HTTP-Client mit Refit automatisch erstellen

### [HerbstSchulung.UnitTesting](Tests/HerbstSchulung.UnitTesting)
- Unit Testing mit xUnit, FluentAssertions, Moq
- AAA-Pattern, Theory/MemberData, TDD-Beispiele
- DI Registrierungen Testing
- [Dokumentation: Unit Testing Grundlagen](Tests/HerbstSchulung.UnitTesting/docs/Unit-Testing-Grundlagen.md)
- [Dokumentation: xUnit vs MS Test2](Tests/HerbstSchulung.UnitTesting/docs/Vergleich-xUnit-MS-Test2.md)

## Allgemeine Dokumentation

- [.NET Framework vs. .NET 8-10](docs/.NET%20-%20Frameworks.md)
- [NuGet Best Practices](docs/Nuget-Best-Practices.md)
- [Common NuGet Pakete - Diskussion](docs/Common-Nuget-Pakete.md)
- [EditorConfig Guide](docs/Editorconfig-Guide.md)
- [Quiz](docs/Quiz.md) - 18 Fragen mit Lösungen

## Voraussetzungen
- .NET SDK 8
- Visual Studio 2022 oder eine andere IDE


