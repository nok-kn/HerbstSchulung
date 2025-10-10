using HerbstSchulung.Hosting.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HerbstSchulung.Hosting.Services;

/// <summary>
/// Erweiterungsmethoden f�r DI-Registrierungen dieses Moduls.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registriert Beispiel-Services dieser Bibliothek.
    /// </summary>
    /// <remarks>
    /// - Implementierungen sind "internal", nur die Interfaces sind �ffentlich.
    /// - Lebensdauern sind bewusst einfach gehalten (Singleton f�r Uhr, Transient f�r Greeter).
    /// </remarks>
    public static IServiceCollection AddMyBuisnessLogicServices(this IServiceCollection services, IConfiguration? configuration = null)
    {
        // Zugriff auf Konfiguration w�re hier m�glich (z. B. Optionen binden)
        // var section = configuration?.GetSection("Greeter");

        services
            .AddSingleton<IClock, SystemClock>()
            .AddTransient<IGreeter, Greeter>();

        return services;
    }
}
