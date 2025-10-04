using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace HerbstSchulung.DependencyInjection;

public static class DependencyInjectionExamples
{
    /// <summary>
    /// Demonstriert eine sehr einfache Registrierung und Auflösung eines Dienstes.
    /// </summary>
    public static void EinfacheRegistrierung()
    {
        var services = new ServiceCollection();
        services.AddTransient<IUhrzeitProvider, SystemUhrzeitProvider>();

        using var provider = services.BuildServiceProvider();
        var uhr = provider.GetRequiredService<IUhrzeitProvider>();
        Console.WriteLine($"Aktuelle Zeit (UTC): {uhr.AktuelleZeit():O}");
    }
    

    /// <summary>
    /// Zeigt Unterschiede der Lifetimes (Transient, Scoped, Singleton) anhand von zufällig generierten GUIDs.
    /// Erwartung: Transient -> immer neu, Scoped -> pro Scope einmal, Singleton -> immer gleich.
    /// </summary>
    public static void DemoLebenszyklen()
    {
        var services = new ServiceCollection();
        services.AddTransient<OperationTransient>();
        services.AddScoped<OperationScoped>();
        services.AddSingleton<OperationSingleton>();

        using var provider = services.BuildServiceProvider();

        // Scope 1
        using (var scope1 = provider.CreateScope())
        {
            var t1 = scope1.ServiceProvider.GetRequiredService<OperationTransient>();
            var t2 = scope1.ServiceProvider.GetRequiredService<OperationTransient>();
            var s1 = scope1.ServiceProvider.GetRequiredService<OperationScoped>();
            var s2 = scope1.ServiceProvider.GetRequiredService<OperationScoped>();
            var si1 = scope1.ServiceProvider.GetRequiredService<OperationSingleton>();
            var si2 = scope1.ServiceProvider.GetRequiredService<OperationSingleton>();

            Console.WriteLine("-- Scope 1 --");
            Console.WriteLine($"Transient 1: {t1.Id}");
            Console.WriteLine($"Transient 2: {t2.Id} (sollte != Transient 1 sein)");
            Console.WriteLine($"Scoped 1:    {s1.Id}");
            Console.WriteLine($"Scoped 2:    {s2.Id} (sollte == Scoped 1 sein)");
            Console.WriteLine($"Singleton 1: {si1.Id}");
            Console.WriteLine($"Singleton 2: {si2.Id} (sollte == Singleton 1 sein)");
        }

        // Scope 2
        using (var scope2 = provider.CreateScope())
        {
            var s3 = scope2.ServiceProvider.GetRequiredService<OperationScoped>();
            Console.WriteLine("-- Scope 2 --");
            Console.WriteLine($"Scoped 3:    {s3.Id} (sollte != Scoped 1 sein)");
        }

        // Singleton Vergleich außerhalb Scopes
        var singletonOutside = provider.GetRequiredService<OperationSingleton>();
        Console.WriteLine($"Singleton (Root): {singletonOutside.Id} (identisch zu den Singleton-IDs in Scope 1)");
    }
  
    /// <summary>
    /// Demonstriert eine Factory-Registrierung (delegierte Registrierung) für ein Interface.
    /// </summary>
    public static void FactoryRegistrierung()
    {
        var services = new ServiceCollection();
        // Factory vergibt beim Aufbau einen neuen Guid.
        services.AddSingleton<IGuidProvider>(new GuidProvider(Guid.NewGuid()));
        services.AddTransient<IUhrzeitProvider, SystemUhrzeitProvider>();
        services.AddTransient<BerichtService>();

        using var provider = services.BuildServiceProvider();
        var service1 = provider.GetRequiredService<BerichtService>();
        var service2 = provider.GetRequiredService<BerichtService>();

        // Da GuidProvider Singleton (durch AddSingleton) ist, sollten beide Berichte denselben Guid tragen.
        Console.WriteLine(service1.ErstelleBericht("Factory Beispiel 1"));
        Console.WriteLine(service2.ErstelleBericht("Factory Beispiel 2"));
    }
    
    /// <summary>
    /// Demonstriert Keyed Services (.NET 8). Registriert zwei Notifier Implementierungen mit Schlüsseln und ruft beide ab.
    /// </summary>
    public static void KeyedServices()
    {
#if NET8_0_OR_GREATER
        var services = new ServiceCollection();
        services.AddKeyedSingleton<INotifier, EmailNotifier>("email");
        services.AddKeyedSingleton<INotifier, SmsNotifier>("sms");

        using var provider = services.BuildServiceProvider();
        var email = provider.GetRequiredKeyedService<INotifier>("email");
        var sms = provider.GetRequiredKeyedService<INotifier>("sms");

        email.Senden("Willkommen via E-Mail");
        sms.Senden("Willkommen via SMS");
#else
        Console.WriteLine("Keyed Services sind erst ab .NET 8 verfügbar.");
#endif
    }
    
    /// <summary>
    /// Demonstriert das Options Pattern in Kombination mit DI (vereinfachtes Beispiel ohne echte Konfiguration).
    /// </summary>
    public static void Optionen()
    {
        var services = new ServiceCollection();
        services.AddOptions<BerichtOptions>()
                .Configure(o => o.StandardTitel = "Standard-Berichtstitel");

        services.AddSingleton<IGuidProvider>(sp => new GuidProvider(Guid.NewGuid()));
        services.AddTransient<IUhrzeitProvider, SystemUhrzeitProvider>();
        services.AddTransient<BerichtServiceMitOptionen>();

        using var provider = services.BuildServiceProvider();
        var service = provider.GetRequiredService<BerichtServiceMitOptionen>();
        Console.WriteLine(service.ErstelleBericht());
    }
 
    public interface IUhrzeitProvider
    {
        DateTime AktuelleZeit();
    }

    /// <summary>
    /// Konkrete Implementierung, die direkt DateTime.UtcNow nutzt.
    /// Achtung: Für testbaren Code oft besser eine abstrahierte Zeitquelle (hier Interface) zu injizieren.
    /// </summary>
    public class SystemUhrzeitProvider : IUhrzeitProvider
    {
        public DateTime AktuelleZeit() => DateTime.UtcNow;
    }

    /// <summary>
    /// Demonstration einer Klasse mit mehreren Abhängigkeiten.
    /// </summary>
    public class BerichtService(IUhrzeitProvider uhrzeitProvider, IGuidProvider guidProvider)
    {
        public string ErstelleBericht(string titel)
            => $"[{uhrzeitProvider.AktuelleZeit():O}] ({guidProvider.AktuellerGuid}) {titel}";
    }

    /// <summary>
    /// Variante mit Optionen (Options Pattern) zur Demonstration.
    /// </summary>
    public class BerichtServiceMitOptionen(
        IUhrzeitProvider uhrzeitProvider,
        IGuidProvider guidProvider,
        IOptions<BerichtOptions> options)
    {
        private readonly BerichtOptions _options = options.Value;

        public string ErstelleBericht()
            => $"[{uhrzeitProvider.AktuelleZeit():O}] ({guidProvider.AktuellerGuid}) {_options.StandardTitel}";
    }

    public class BerichtOptions
    {
        public string StandardTitel { get; set; } = string.Empty;
    }

    public interface IGuidProvider
    {
        Guid AktuellerGuid { get; }
    }

    /// <summary>
    /// Implementation mit wählbarem Lifetime-Effekt: Transient => jedes Mal neuer Guid.
    /// Singleton => immer derselbe.
    /// </summary>
    public class GuidProvider : IGuidProvider
    {
        public GuidProvider(Guid guid) => AktuellerGuid = guid;
        public Guid AktuellerGuid { get; }
    }

    /// <summary>
    /// Hilfsklassen zur Visualisierung verschiedener Lifetimes.
    /// </summary>
    public sealed class OperationTransient { public Guid Id { get; } = Guid.NewGuid(); }
    public sealed class OperationScoped { public Guid Id { get; } = Guid.NewGuid(); }
    public sealed class OperationSingleton { public Guid Id { get; } = Guid.NewGuid(); }

    // Notifier Beispiele für Keyed Services
    public interface INotifier
    {
        void Senden(string nachricht);
    }

    public class EmailNotifier : INotifier
    {
        public void Senden(string nachricht) => Console.WriteLine($"[EMAIL] {nachricht}");
    }

    public class SmsNotifier : INotifier
    {
        public void Senden(string nachricht) => Console.WriteLine($"[SMS] {nachricht}");
    }
}
