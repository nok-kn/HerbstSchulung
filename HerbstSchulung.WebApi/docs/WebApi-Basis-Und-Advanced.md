# WebAPI 

## Themenübersicht

- Kestrel als Webserver: Standard in ASP.NET Core, plattformunabhängig und performant.
- Controller-basierte API: [ApiController]-Attribut 
- Swagger/OpenAPI: Automatische API-Dokumentation, UI im Development aktiviert.
- ProblemDetails (RFC7807): Einheitliche Fehlerantworten via AddProblemDetails und UseExceptionHandler.
- Logging: HTTP-Request-Logging via AddHttpLogging/UseHttpLogging und strukturierte Logs über ILogger.
- Health Checks: /health Endpoint für Liveness/Readiness.
- Health Checks UI: /health-ui Endpoint für Healhth UI.
- HealthChecks erweiterbar mit Datenbank-, Cache- oder externen Endpoint-Checks.
- Filter: Globaler TimingActionFilter misst Laufzeiten von Aktionen.
- XML-Dokumentationskommentare werden in Swagger eingebunden (GenerateDocumentationFile).
- Exception-Mapping: z.B KeyNotFoundException -> 404 NotFound im Filter
- JSON mit System.Text.Json
- Middleware-Pipeline: UseExceptionHandler, UseStatusCodePages, UseHttpLogging, UseHttpsRedirection.
- Containerisierung: Multi-Stage Dockerfile, kleines Runtime-Image (aspnet:8.0)
- Versionierung (optional): AddApiVersioning könnte ergänzt werden.
- Rate Limiting (optional): AddRateLimiter für Schutz vor Missbrauch.
- CORS (optional): AddCors für Browser-Clients.

## Warum kein Http.sys verwenden?

- Plattformabhängigkeit: Http.sys basiert auf Windows-HTTP-Server (HTTP.SYS) und läuft nur unter Windows. Kestrel ist plattformübergreifend.
- Container-/Cloud-Szenarien: Kestrel ist der empfohlene Server für Docker und Orchestratoren (Kubernetes, Azure App Service).
- Performance und Einfachheit: Kestrel ist leichtgewichtig und für hohe Performance optimiert. Http.sys ist sinnvoll, wenn spezifische Windows-Features benötigt werden (z. B. integrierte Windows-Authentifizierung mit Kerberos/NTLM, HTTP.sys-Request-Queue), was hier nicht der Fall ist.
- Microsoft-Empfehlung: Standardmäßig Kestrel nutzen; Http.sys nur für spezielle On-Prem-Szenarien auf Windows.

## Docker

- Datei: HerbstSchulung.WebApi/Dockerfile
- Build: docker build -f HerbstSchulung.WebApi/Dockerfile -t herbstschulung-webapi . 
- Run: docker run -p 8080:8080 herbstschulung-webapi

## Zusätzliche Ressourcen

- ASP.NET Core Web API Grundlagen: https://learn.microsoft.com/aspnet/core/web-api/
- Kestrel Webserver: https://learn.microsoft.com/aspnet/core/fundamentals/servers/kestrel
- ProblemDetails (RFC7807) in ASP.NET Core: https://learn.microsoft.com/aspnet/core/web-api/handle-errors
- Health Checks: https://learn.microsoft.com/aspnet/core/host-and-deploy/health-checks
- Logging in ASP.NET Core: https://learn.microsoft.com/aspnet/core/fundamentals/logging/
- Swagger/OpenAPI (Swashbuckle): https://learn.microsoft.com/aspnet/core/tutorials/web-api-help-pages-using-swagger
- Filters in ASP.NET Core: https://learn.microsoft.com/aspnet/core/mvc/controllers/filters
- Docker mit .NET: https://learn.microsoft.com/dotnet/core/docker/
- API Versioning: https://github.com/dotnet/aspnet-api-versioning
- Rate Limiting: https://learn.microsoft.com/aspnet/core/performance/rate-limit
