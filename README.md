# Herbstschulung

## Kurzbeschreibung
- Trainings- und Beispiel-Repository für moderne .NET 8 Themen
- Fokus: Konfiguration, Dependency Injection, Hosting/Worker, Async/await, Nullable Reference Types, Records
- Die Beispiele sind kompakt gehalten 
- Einige Projekte enthalten zusätzliche Dokumentation im jeweiligen docs-Ordner

## Projekte im Überblick
- HerbstSchulung.CSharpFeatures
  - Beispiele zu Nullable Reference Types und Records (z. B. Fahrzeug, RecordExamples)
- HerbstSchulung.Async
  - Async/await-Beispiele und Anti-Patter
  - Begleitende Unit-Tests im Projekt HerbstSchulung.Async.Tests
- HerbstSchulung.Configuration
  - Beispiele f�r Microsoft.Extensions.Configuration und Options Pattern
  - IOptions, IOptionsSnapshot, IOptionsMonitor und Validierung (DataAnnotations, benutzerdefiniert)
  - appsettings.json sowie In-Memory-Konfiguration
- HerbstSchulung.DependencyInjection
  - Beispiele und Übungen zu Microsoft.Extensions.DependencyInjection
  - Lebensdauern (Transient/Scoped/Singleton), ServiceCollection-Erweiterungen
- HerbstSchulung.Hosting
  - Generischer Host, BackgroundService/Worker (TimeLoggingWorker)
  - Integration von Services und Konfiguration
- HerbstSchulung.Hosting.Abstractions
  - Abstraktionen/Contracts f�r Services 
- HerbstSchulung.Hosting.Services
  - Service-Implementierungen  und DI Registrierung

## Voraussetzungen
- .NET SDK 8
- Visual Studio 2022 oder eine andere IDE


