# Herbstschulung

## Kurzbeschreibung
- Trainings- und Beispiel-Repository für moderne .NET 8 Themen
- Fokus: Konfiguration, Dependency Injection, Hosting/Worker, Async/await, Nullable Reference Types, Records
- Die Beispiele sind kompakt gehalten 
- Einige Projekte enthalten zusätzliche Dokumentation im jeweiligen docs-Ordner

## Projekte im Überblick
- HerbstSchulung.Nullable
  - Beispiele zu Nullable Reference Types und Records (z. B. Fahrzeug, RecordExamples)
- - HerbstSchulung.Async
  - Async/await-Beispiele, z. B. Anti-Pattern „Async im Konstruktor“ (AsyncConstructorExample)
  - Begleitende Unit-Tests im Projekt HerbstSchulung.Async.Tests
- HerbstSchulung.Configuration
  - Beispiele für Microsoft.Extensions.Configuration und Options-Pattern
  - IOptions, IOptionsSnapshot, IOptionsMonitor und Validierung (DataAnnotations, benutzerdefiniert)
  - appsettings.json sowie In-Memory-Konfiguration
  - Dokumentation: ./HerbstSchulung.Configuration/docs/Configuration-Basis.md
- HerbstSchulung.DependencyInjection
  - Beispiele und Übungen zu Microsoft.Extensions.DependencyInjection
  - Lebensdauern (Transient/Scoped/Singleton), ServiceCollection-Erweiterungen
  - Dateien: DependencyInjectionExamples.cs, DependencyInjectionChallenges.cs, AssertServiceCollectionExtensions.cs
- HerbstSchulung.Hosting
  - Generischer Host, BackgroundService/Worker (TimeLoggingWorker)
  - Integration von Services und Konfiguration
  - Dokumentation: ./HerbstSchulung.Hosting/docs/Hosting-Basis.md
- HerbstSchulung.Hosting.Abstractions
  - Abstraktionen/Contracts für Services 
- HerbstSchulung.Hosting.Services
  - Service-Implementierungen  und DI Registrierung

## Voraussetzungen
- .NET SDK 8
- Visual Studio 2022 oder eine andere IDE


