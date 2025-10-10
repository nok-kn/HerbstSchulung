using System.Runtime.Versioning;
using HerbstSchulung.Hosting.Abstractions;
using HerbstSchulung.Hosting.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace HerbstSchulung.Hosting;

/// <summary>
/// Enthält kompakte Beispiele für Hosting in unterschiedlichen Anwendungstypen:
/// </summary>
public static class HostingExamples
{
    /// <summary>
    /// Beispiel für einen "Generic Host" in einer Konsolenanwendung.
    /// </summary>
    /// <remarks>
    /// Verwendung (z. B. in Program.cs einer Console-App):
    ///   var host = HostingExamples.CreateConsoleHost(args);
    ///   await host.RunAsync();
    /// </remarks>
    public static IHost CreateConsoleHost(string[]? args = null)
    {
        var builder = Host.CreateApplicationBuilder(args);

        AddConfiguration(builder.Configuration, builder.Environment, args);

        ConfigureLogging(builder.Logging, builder.Environment);

        builder.Services.AddMyBuisnessLogicServices(builder.Configuration);

        builder.Services.AddHostedService<TimeLoggingWorker>();

        return builder.Build();
    }

    /// <summary>
    /// Beispiel für eine Web-Anwendung (Minimal API) mit WebApplication.
    /// </summary>
    /// <remarks>
    /// Verwendung:
    ///   var app = HostingExamples.CreateWebApplication(args);
    ///   app.Run();
    /// </remarks>
    public static WebApplication CreateWebApplication(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        AddConfiguration(builder.Configuration, builder.Environment, args);

        ConfigureLogging(builder.Logging, builder.Environment);

        builder.Services.AddMyBuisnessLogicServices(builder.Configuration);

        var app = builder.Build();

        if (IsDev(builder.Environment))
        {
            // Developer-spezifische Einstellungen (z. B. Swagger, Detailed Errors)
            app.Logger.LogInformation("Starte im DEV-Modus");
        }
        else if (IsTest(builder.Environment))
        {
            app.Logger.LogInformation("Starte im TEST-Modus");
        }
        else
        {
            // Prod: z. B. HSTS, Exception-Handler usw.
            app.Logger.LogInformation("Starte im PROD-Modus");
            app.UseHsts();
        }

        // Beispiel Minimal API
        app.MapGet("/greet/{name?}", (IGreeter greeter, string? name) =>
        {
            // Einfache DI-Nutzung im Handler
            var text = greeter.Greet(name);
            return Results.Ok(text);
        });

        app.MapGet("/time", (IClock clock) => Results.Ok(new { utc = clock.UtcNow }));

        return app;
    }

    /// <summary>
    /// Beispiel für Integration des Generic Host in eine WPF-Anwendung.
    /// </summary>
    /// <remarks>
    /// Typische Verwendung in einer WPF-App (App.xaml.cs):
    ///   public partial class App : Application
    ///   {
    ///       private IHost? _host;
    ///       protected override void OnStartup(StartupEventArgs e)
    ///       {
    ///           base.OnStartup(e);
    ///           _host = HostingExamples.CreateWpfHost();
    ///           _host.Start();
    ///           // Optional: Hauptfenster über DI auflösen
    ///           // var mainWindow = _host.Services.GetRequiredService<MainWindow>();
    ///           // mainWindow.Show();
    ///       }
    ///       protected override void OnExit(ExitEventArgs e)
    ///       {
    ///           _host?.Dispose();
    ///           base.OnExit(e);
    ///       }
    ///   }
    /// </remarks>
    [SupportedOSPlatform("windows")] // WPF ist Windows-spezifisch
    public static IHost CreateWpfHost(string[]? args = null)
    {
        var builder = Host.CreateApplicationBuilder(args);

        // Konfiguration wie bei Console/Web
        AddConfiguration(builder.Configuration, builder.Environment, args);
        ConfigureLogging(builder.Logging, builder.Environment);

        // Services registrieren (inkl. WPF-spezifischer Typen, falls vorhanden)
        builder.Services.AddMyBuisnessLogicServices(builder.Configuration);

        // Beispiel: Registrierung eines ViewModels oder Windows wäre hier möglich
        // builder.Services.AddTransient<MainWindow>();
        // builder.Services.AddTransient<MainViewModel>();

        return builder.Build();
    }

    /// <summary>
    /// Fügt übliche Konfigurationsquellen hinzu
    /// </summary>
    private static void AddConfiguration(ConfigurationManager configuration, IHostEnvironment env, string[]? args)
    {
        configuration
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables();

        if (args is { Length: > 0 })
        {
            configuration.AddCommandLine(args);
        }
    }

    /// <summary>
    /// Richtet Logging so ein, dass es in DEV ausführlicher und in PROD restriktiver ist
    /// Das ist nur ein Beispiel, in echten Anwendungen kommen oft die Logging Einstllungen von apssettings.json
    /// </summary>
    private static void ConfigureLogging(ILoggingBuilder logging, IHostEnvironment env)
    {
        logging.ClearProviders();
        logging.AddSimpleConsole(options =>
        {
            options.SingleLine = true;
            options.TimestampFormat = "HH:mm:ss.fff ";
        });
        logging.AddDebug(); 

        if (IsDev(env))
        {
            logging.SetMinimumLevel(LogLevel.Debug);
        }
        else if (IsTest(env))
        {
            logging.SetMinimumLevel(LogLevel.Information);
        }
        else // Prod
        {
            logging.SetMinimumLevel(LogLevel.Warning);
        }
    }

    /// <summary>
    /// Akzeptiert sowohl Standard- als auch Kurz-Namen für Umgebungen
    /// </summary>
    private static bool IsDev(IHostEnvironment env)
        => env.IsDevelopment() || env.IsEnvironment("Dev") || env.IsEnvironment("Development");

    private static bool IsTest(IHostEnvironment env)
        => env.IsStaging() || env.IsEnvironment("Test") || env.IsEnvironment("Testing") || env.IsEnvironment("QA");

    private static bool IsProd(IHostEnvironment env)
        => env.IsProduction() || env.IsEnvironment("Prod") || env.IsEnvironment("Production");
}
