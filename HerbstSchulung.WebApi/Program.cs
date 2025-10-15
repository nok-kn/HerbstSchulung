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
    // API-Controller inklusive automatische 400-Validierung
    .AddControllers(options =>
    {
        // Globale Filter hinzufügen (z. B. für Header/Logging etc.)
        options.Filters.Add(new ProducesResponseTypeAttribute(typeof(ProblemDetails), StatusCodes.Status500InternalServerError));
        options.Filters.Add(new ProducesResponseTypeAttribute(typeof(ProblemDetails), StatusCodes.Status400BadRequest));
        options.Filters.Add(new ServiceFilterAttribute(typeof(TimingActionFilter)));
    })
    .AddJsonOptions(o =>
    {
        // Newtonsoft.Json geht langsam in Rente
        // wir verwenden System.Text.Json

        // Konsistente JSON-Optionen: camelCase, Indentierung in Development
        o.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        o.JsonSerializerOptions.WriteIndented = builder.Environment.IsDevelopment();
    });

// Filter als Service registrieren
builder.Services.AddScoped<TimingActionFilter>();

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

app.Run();

