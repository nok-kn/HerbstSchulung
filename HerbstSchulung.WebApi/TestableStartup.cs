using HealthChecks.UI.Client;
using HerbstSchulung.Hosting.Services;
using HerbstSchulung.WebApi.Filters;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace HerbstSchulung.WebApi;

/// <summary>
/// Ein Versuch, die Logik aus dem Program besser zu strukturieren und testbar zu machen.
/// </summary>
public class TestableStartup(IConfiguration configuration, IHostEnvironment environment)
{
    private readonly IConfiguration _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    private readonly IHostEnvironment _environment = environment ?? throw new ArgumentNullException(nameof(environment));

    public IServiceCollection ConfigureServices(IServiceCollection services)
    {
        try
        {
            AddControllersWithFilters(services);
            AddFiltersAsServices(services);
            AddBusinessServices(services);
            AddSwagger(services);
            AddProblemDetails(services);
            AddHttpLogging(services);
            AddHealthChecks(services);
            AddHealthChecksUI(services);
            return services;
        }
        catch (Exception e)
        {
            // Startup-Fehler hier manuell loggen (da Logger noch nicht konfiguriert ist)
            Console.WriteLine(e);
            File.WriteAllText("startup_errors.log", e.ToString());
            throw;
        }
    }

    public void ConfigureMiddleware(WebApplication app)
    {
        UseDevelopmentTools(app);
        app.UseHttpLogging();
        app.UseExceptionHandler();
        app.UseStatusCodePages();
        app.UseHttpsRedirection();

        app.MapControllers();
        MapHealthChecks(app);
        MapHealthChecksUI(app);
        MapRootEndpoint(app);
    }

    // Service registrations
    internal void AddControllersWithFilters(IServiceCollection services)
    {
        services
            .AddControllers(options =>
            {
                options.Filters.Add(new ServiceFilterAttribute(typeof(TimingActionFilter)));
                options.Filters.Add(new ServiceFilterAttribute(typeof(ExceptionMappingFilter)));
            })
            .AddJsonOptions(o =>
            {
                o.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
                o.JsonSerializerOptions.WriteIndented = _environment.IsDevelopment();
            });
    }

    internal void AddFiltersAsServices(IServiceCollection services)
    {
        services.AddScoped<TimingActionFilter>();
        services.AddScoped<ExceptionMappingFilter>();
    }

    internal void AddBusinessServices(IServiceCollection services)
    {
        services.AddMyBuisnessLogicServices(_configuration);
    }

    internal void AddSwagger(IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            var xmlName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name + ".xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlName);
            if (File.Exists(xmlPath))
            {
                c.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
            }
        });
    }

    internal void AddProblemDetails(IServiceCollection services)
    {
        services.AddProblemDetails();
    }

    internal void AddHttpLogging(IServiceCollection services)
    {
        services.AddHttpLogging(logging =>
        {
            logging.LoggingFields = HttpLoggingFields.RequestPropertiesAndHeaders | HttpLoggingFields.ResponsePropertiesAndHeaders;
        });
    }

    internal void AddHealthChecks(IServiceCollection services)
    {
        services.AddHealthChecks()
            .AddCheck("Self", () => HealthCheckResult.Healthy())
            .AddSqlServer(_configuration.GetConnectionString("Default")!, name: "Database");
    }

    internal void AddHealthChecksUI(IServiceCollection services)
    {
        services
            .AddHealthChecksUI(options =>
            {
                options.AddHealthCheckEndpoint("API Health", "/health");
            })
            .AddInMemoryStorage();
    }

    // Middleware / pipeline
    internal void UseDevelopmentTools(WebApplication app)
    {
        if (_environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
    }

    internal void MapHealthChecks(WebApplication app)
    {
        app.MapHealthChecks("/health", new HealthCheckOptions
        {
            Predicate = _ => true,
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });
    }

    internal void MapHealthChecksUI(WebApplication app)
    {
        app.MapHealthChecksUI(options => options.UIPath = "/health-ui");
    }

    internal void MapRootEndpoint(WebApplication app)
    {
        app.MapGet("/", () => Results.Content(IndexHtml, "text/html; charset=utf-8")).ExcludeFromDescription();
    }

    private const string IndexHtml = """
<!doctype html>
<html lang=\"en\">
<head>
  <meta charset=\"utf-8\" />
  <meta name=\"viewport\" content=\"width=device-width, initial-scale=1\" />
  <title>HerbstSchulung API</title>
  <style>
    body{font-family: system-ui, -apple-system, Segoe UI, Roboto, Helvetica, Arial, sans-serif; margin:2rem;}
    a{display:block; margin:.5rem 0; font-size:1.1rem;}
  </style>
</head>
<body>
  <h1>HerbstSchulung API</h1>
  <ul>
    <li><a href=\"/swagger\">Swagger UI</a></li>
    <li><a href=\"/health-ui\">Health Checks UI</a></li>
  </ul>
</body>
</html>
""";
}
