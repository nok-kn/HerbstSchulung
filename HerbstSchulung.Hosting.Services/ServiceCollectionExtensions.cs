using HerbstSchulung.Hosting.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using HerbstSchulung.Hosting.Abstractions.Deskriptor;
using HerbstSchulung.Hosting.Services.Deskriptor;

namespace HerbstSchulung.Hosting.Services;

/// <summary>
/// Erweiterungsmethoden für DI-Registrierungen dieses Moduls.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registriert Beispiel-Services dieser Bibliothek.
    /// </summary>
    /// <remarks>
    /// - Implementierungen sind "internal", nur die Interfaces sind öffentlich.
    /// - Lebensdauern sind bewusst einfach gehalten (Singleton für Uhr, Transient für Greeter).
    /// </remarks>
    public static IServiceCollection AddMyBuisnessLogicServices(this IServiceCollection services, IConfiguration? configuration = null)
    {
        // Zugriff auf Konfiguration wäre hier möglich (z. B. Optionen binden)
        // var section = configuration?.GetSection("Greeter");

        services
            .AddSingleton<IClock, SystemClock>()
            .AddTransient<IGreeter, Greeter>()
            .AddSingleton<IDeskriptorService, InMemoryDeskriptorService>();

        return services;
    }
}
