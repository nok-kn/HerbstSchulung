# Hosting (Microsoft.Extensions.Hosting) � Basis

## Ziele 
- Einheitliches Bootstrap f�r Console, Web und Desktop (WPF, WinForms).
- Zentralisierte Infrastruktur: Dependency Injection, Konfiguration, Logging, Lebenszyklus.
- Saubere Trennung von App-Code und Infrastruktur.

## Kernkonzepte
- Host: Verwaltet Lebenszyklus, DI-Container, Logging, Konfiguration.
- Generic Host: `Host.CreateApplicationBuilder(...)` f�r nicht-web Apps.
- WebApplication Host: `WebApplication.CreateBuilder(...)` f�r Minimal APIs/ASP.NET Core.
- IHostedService/BackgroundService: Ausf�hrende Komponenten, die mit dem Host starten und stoppen.
- IHostEnvironment/IWebHostEnvironment: Informationen zur Umgebung (z. B. Development, Staging, Production).

## Konfiguration
Typische Quellen (Merge-Reihenfolge):
1. appsettings.json
2. appsettings.{Environment}.json
3. Umgebungsvariablen
4. Kommandozeilenargumente

Tipp: DOTNET_ENVIRONMENT oder ASPNETCORE_ENVIRONMENT auf �Dev�, �Test� oder �Prod� setzen.

## Logging
- Provider: Console, Debug, EventSource, Serilog etc.
- Mindestlevel umgebungsspezifisch w�hlen (z. B. Debug in Dev, Warning in Prod).
- Filter f�r Namespaces einsetzen, um Noise zu reduzieren.

## Umgebungen (Dev/Test/Prod)
- Standard: Development, Staging, Production
- Kurzformen m�glich: �Dev�, �Test�, �Prod� (per `IsEnvironment("...")`)
- Umgebung beeinflusst:
  - Log-Level
  - Middleware (z. B. Developer-Tools nur in Dev)
  - Konfigurationsdateien (appsettings.{Env}.json)

## H�ufige Fallstricke
- Logging doppelt konfigurieren (sowohl in Bibliothek als auch im Host): Logging zentral im Host konfigurieren.
- Fehlende Reloads: `reloadOnChange: true` f�r appsettings aktivieren.
- Umgebungsnamen inkonsistent: Einheitliche Benennung und Abgleich via `IsEnvironment(...)`.
- Vergessene `await host.RunAsync()` bzw. `app.Run()`: Die App l�uft sonst nicht.

## Zus�tzliche Ressourcen
- Microsoft Docs � Generic Host: https://learn.microsoft.com/aspnet/core/fundamentals/host/generic-host
- Microsoft Docs � Dependency Injection: https://learn.microsoft.com/aspnet/core/fundamentals/dependency-injection
- Microsoft Docs � Logging: https://learn.microsoft.com/aspnet/core/fundamentals/logging
- Microsoft Docs � Konfiguration: https://learn.microsoft.com/aspnet/core/fundamentals/configuration
- Microsoft Docs � Minimal APIs: https://learn.microsoft.com/aspnet/core/fundamentals/minimal-apis
