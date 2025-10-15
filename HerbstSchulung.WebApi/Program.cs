using HerbstSchulung.Hosting.Services;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using HerbstSchulung.WebApi.Filters;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

// Dienste in DI registrieren konfigurieren
builder.Services
    .AddControllers(options =>
    {
        // Globale Filter hinzufügen (z. B. für Header/Logging etc.)
        options.Filters.Add(new ServiceFilterAttribute(typeof(TimingActionFilter)));
        options.Filters.Add(new ServiceFilterAttribute(typeof(ExceptionMappingFilter)));
    })
    .AddJsonOptions(o =>
    {
        // Newtonsoft.Json geht langsam in Rente, man verwendet jetzt System.Text.Json ...

        // Konsistente JSON-Optionen: camelCase, Indentierung in Development
        o.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        o.JsonSerializerOptions.WriteIndented = builder.Environment.IsDevelopment();
    });

// Filter als Service registrieren
builder.Services.AddScoped<TimingActionFilter>();
builder.Services.AddScoped<ExceptionMappingFilter>();

// Abstraktionen/Services aus eigener Bibliothek registrieren
builder.Services.AddMyBuisnessLogicServices(builder.Configuration);

// Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    // Einbindung von XML-Kommentaren
    var xmlName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name + ".xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlName);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
    }
});

// ProblemDetails gemäß RFC7807
builder.Services.AddProblemDetails();

// HTTP-Request-Logging (Header, Body-Größen etc.)
builder.Services.AddHttpLogging(logging =>
{
    logging.LoggingFields = HttpLoggingFields.RequestPropertiesAndHeaders | HttpLoggingFields.ResponsePropertiesAndHeaders;
});

// HealthChecks
builder.Services.AddHealthChecks()
    .AddCheck("Self", () => HealthCheckResult.Healthy())
    .AddSqlServer(builder.Configuration.GetConnectionString("Default")!, name: "Database");

// HealthChecks UI
builder.Services
    .AddHealthChecksUI(options =>
    {
        // UI polls this endpoint which aggregates "self" and "Database"
        options.AddHealthCheckEndpoint("API Health", "/health");
    })
    .AddInMemoryStorage();

// Jetzt die App bauen
var app = builder.Build();

// Entwicklungs-spezifische Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// HTTP-Request-Logging aktivieren
app.UseHttpLogging();

// Exception-Handler und ProblemDetails
app.UseExceptionHandler();
app.UseStatusCodePages();
app.UseHttpsRedirection();

// Routing und Endpunkte
app.MapControllers();

// HealthCheck Endpoint 
app.MapHealthChecks("/health", new HealthCheckOptions
{
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

// HealthChecks UI endpoint
app.MapHealthChecksUI(options => options.UIPath = "/health-ui");
 
// Startseite (Mini-API) mit Links zu Swagger und HealthChecks UI
app.MapGet("/", () =>
{
    const string html = """
                        <!doctype html>
                        <html lang="en">
                        <head>
                          <meta charset="utf-8" />
                          <meta name="viewport" content="width=device-width, initial-scale=1" />
                          <title>HerbstSchulung API</title>
                          <style>
                            body{font-family: system-ui, -apple-system, Segoe UI, Roboto, Helvetica, Arial, sans-serif; margin:2rem;}
                            a{display:block; margin:.5rem 0; font-size:1.1rem;}
                          </style>
                        </head>
                        <body>
                          <h1>HerbstSchulung API</h1>
                          <ul>
                            <li><a href="/swagger">Swagger UI</a></li>
                            <li><a href="/health-ui">Health Checks UI</a></li>
                          </ul>
                        </body>
                        </html>
                        """;
    return Results.Content(html, "text/html; charset=utf-8");
}).ExcludeFromDescription(); // nicht in Swagger anzeigen


app.Run();

