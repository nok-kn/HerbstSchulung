# Hosting (Microsoft.Extensions.Hosting) – Basis

## Ziele 
- Einheitliches Bootstrap für Console, Web und Desktop (WPF, WinForms).
- Zentralisierte Infrastruktur: Dependency Injection, Konfiguration, Logging, Lebenszyklus.
- Saubere Trennung von App-Code und Infrastruktur.

## Kernkonzepte
- Host: Verwaltet Lebenszyklus, DI-Container, Logging, Konfiguration.
- Generic Host: `Host.CreateApplicationBuilder(...)` für nicht-web Apps.
- WebApplication Host: `WebApplication.CreateBuilder(...)` für Minimal APIs/ASP.NET Core.
- IHostedService/BackgroundService: Ausführende Komponenten, die mit dem Host starten und stoppen.
- IHostEnvironment/IWebHostEnvironment: Informationen zur Umgebung (z. B. Development, Staging, Production).

## Konfiguration
Typische Quellen (Merge-Reihenfolge):
1. appsettings.json
2. appsettings.{Environment}.json
3. Umgebungsvariablen
4. Kommandozeilenargumente

Tipp: DOTNET_ENVIRONMENT oder ASPNETCORE_ENVIRONMENT auf „Dev“, „Test“ oder „Prod“ setzen.

## Logging
- Provider: Console, Debug, EventSource, Serilog etc.
- Mindestlevel umgebungsspezifisch wählen (z. B. Debug in Dev, Warning in Prod).
- Filter für Namespaces einsetzen, um Noise zu reduzieren.

## Umgebungen (Dev/Test/Prod)
- Standard: Development, Staging, Production
- Kurzformen möglich: „Dev“, „Test“, „Prod“ (per `IsEnvironment("...")`)
- Umgebung beeinflusst:
  - Log-Level
  - Middleware (z. B. Developer-Tools nur in Dev)
  - Konfigurationsdateien (appsettings.{Env}.json)

## Häufige Fallstricke
- Logging doppelt konfigurieren (sowohl in Bibliothek als auch im Host): Logging zentral im Host konfigurieren.
- Fehlende Reloads: `reloadOnChange: true` für appsettings aktivieren.
- Umgebungsnamen inkonsistent: Einheitliche Benennung und Abgleich via `IsEnvironment(...)`.
- Vergessene `await host.RunAsync()` bzw. `app.Run()`: Die App läuft sonst nicht.

## Zusätzliche Ressourcen
- Microsoft Docs – Generic Host: https://learn.microsoft.com/aspnet/core/fundamentals/host/generic-host
- Microsoft Docs – Dependency Injection: https://learn.microsoft.com/aspnet/core/fundamentals/dependency-injection
- Microsoft Docs – Logging: https://learn.microsoft.com/aspnet/core/fundamentals/logging
- Microsoft Docs – Konfiguration: https://learn.microsoft.com/aspnet/core/fundamentals/configuration
- Microsoft Docs – Minimal APIs: https://learn.microsoft.com/aspnet/core/fundamentals/minimal-apis
