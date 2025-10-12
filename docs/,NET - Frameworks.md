# Unterschied: .NET Framework 4.x.x vs. .NET Core 8/9/10

| Thema                        | .NET Framework 4.x.x                             | .NET 8 / 9 / 10 (modernes .NET)                 |
|------------------------------|--------------------------------------------------|-------------------------------------------------|
| **Plattform**                | Nur Windows                                      | Cross-Plattform (Windows, Linux, macOS)         |
| **Runtime**                  | CLR (Common Language Runtime)                    | .NET Runtime                                    |
| **Depoyment**        | Monolithisch, keine Side-by-Side Installation, GAC | Side-by-Side, Self-contained Installation, AOT        |
| **WebAPI-Technologie**       | ASP.NET (System.Web)                             | ASP.NET Core (modular, leichtgewichtig)         |
| **Biblotheken**     |          Standard und 3-part Nuget      |                 | Standard, NuGet-Ökosystem, Microsoft.Extensions
| **Konfigurationssystem**     | Web.config / App.config (XML)                    | `appsettings.json` (JSON, flexibel)             |
| **NuGet-Verwaltung**         | `packages.config` (separat)                      | `PackageReference` direkt im `.csproj`, Pinning |
| **Support & Updates**        | Nur Sicherheitsupdates                           | Aktive Entwicklung & Feature-Releases           |
| **Open Source**              | Nein                                             | Ja (komplett auf GitHub)                        |
| **Build-System**             | Klassisches MSBuild                              | Modernes MSBuild, inkl. Hot Reload, Source Gen. |
| **Performance**              | Schwergewichtig                                  | Deutlich besser                                 |
| **Microservices / Cloud**    | Nicht ideal geeignet (nur Windows)               | Optimiert für Docker, Kubernetes, Cloud-native  |

