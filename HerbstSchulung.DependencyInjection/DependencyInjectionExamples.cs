using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Scrutor;

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
    public static void Lebenszyklen()
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
    /// Demonstriert eine Factory-Registrierung
    /// </summary>
    public static void FactoryRegistrierung()
    {
        var services = new ServiceCollection();
        services.AddTransient<IUhrzeitProvider, SystemUhrzeitProvider>();
        services.AddSingleton<Func<string, BerichtService>>(sp => 
            berichtName => new BerichtService(sp.GetRequiredService<IUhrzeitProvider>(), berichtName));

        using var provider = services.BuildServiceProvider();
        var berichtService = provider.GetService<BerichtService>();
       
        Console.WriteLine(berichtService == null ? "BerichtService ist nicht registriert" : "BerichtService ist registriert");
        
        var factory = provider.GetRequiredService<Func<string, BerichtService>>();
        berichtService = factory("Bericht Nr 1");
        Console.WriteLine(berichtService.ErstelleBericht());
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

        services.AddSingleton<IGuidProvider>(new GuidProvider(Guid.NewGuid()));
        services.AddTransient<IUhrzeitProvider, SystemUhrzeitProvider>();
        services.AddTransient<BerichtServiceMitOptionen>();

        using var provider = services.BuildServiceProvider();
        var service = provider.GetRequiredService<BerichtServiceMitOptionen>();
        Console.WriteLine(service.ErstelleBericht());
    }
 
    /// <summary>
    /// Demonstriert Registrierung mehrerer Implementierungen desselben Interfaces und Auflösung als IEnumerable.
    /// WICHTIG: Die Auflösereihenfolge entspricht der Registrierungsreihenfolge.
    /// </summary>
    public static void MehrfachImplementierungen()
    {
        var services = new ServiceCollection();
        // Mehrere Implementierungen für dasselbe Interface registrieren
        services.AddSingleton<INotifier, EmailNotifier>();
        services.AddSingleton<INotifier, SmsNotifier>();
        // Aggregat-Dienst der alle Notifier nutzt
        services.AddSingleton<AggregatNotifier>();

        using var provider = services.BuildServiceProvider();

        // Direkte Enumeration
        var alleNotifiers = provider.GetRequiredService<IEnumerable<INotifier>>();
        Console.WriteLine("-- Direkte Enumeration über IEnumerable<INotifier> --");
        foreach (var n in alleNotifiers)
        {
            n.Senden("Test an alle");
        }

        // Nutzung über Aggregat-Service (zeigt Injection von IEnumerable<INotifier>)
        var aggregator = provider.GetRequiredService<AggregatNotifier>();
        aggregator.SendeAnAlle("Nachricht via AggregatNotifier");
    }

    /// <summary>
    /// Demonstriert automatische Service-Registrierung mit Scrutor.
    /// Scrutor scannt Assemblies und registriert Services basierend auf Namenskonventionen oder Attributen.
    /// </summary>
    public static void ScrutorRegistrierung()
    {
        var services = new ServiceCollection();
        
        // Automatische Registrierung aller Services die auf "Service" enden
        // und ein entsprechendes Interface haben (z.B. ICalculatorService -> CalculatorService)
        services.Scan(scan => scan
            .FromCallingAssembly()
            .AddClasses(classes => classes.Where(type => type.Name.EndsWith("Service")))
            .AsImplementedInterfaces()
            .WithTransientLifetime());

        // Registrierung mit spezifischen Namenskonventionen
        services.Scan(scan => scan
            .FromCallingAssembly()
            .AddClasses(classes => classes.InNamespaceOf(typeof(DependencyInjectionExamples)))
            .UsingRegistrationStrategy(RegistrationStrategy.Skip) // Überspringe bereits registrierte Services
            .AsMatchingInterface()
            .WithSingletonLifetime());

        using var provider = services.BuildServiceProvider();

        // Test der automatisch registrierten Services
        try
        {
            var calculator = provider.GetService<ICalculatorService>();
            if (calculator != null)
            {
                Console.WriteLine($"CalculatorService automatisch registriert. Testing 5 + 3 = {calculator.Add(5, 3)}");
            }
            else
            {
                Console.WriteLine("CalculatorService wurde nicht automatisch registriert");
            }

            var validator = provider.GetService<IDataValidator>();
            if (validator != null)
            {
                Console.WriteLine($"DataValidator automatisch registriert: {validator.IsValid("test@example.com")}");
            }
            else
            {
                Console.WriteLine("DataValidator wurde nicht automatisch registriert");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Fehler beim Auflösen der Services: {ex.Message}");
        }
    }

    public interface IUhrzeitProvider
    {
        DateTime AktuelleZeit();
    }

    /// <summary>
    /// Für testbaren Code ist es oft besser eine abstrahierte Zeitquelle (hier Interface) zu injizieren.
    /// </summary>
    public class SystemUhrzeitProvider : IUhrzeitProvider
    {
        public DateTime AktuelleZeit() => DateTime.UtcNow;
    }

    public class BerichtService(IUhrzeitProvider uhrzeitProvider, string berichtName)
    {
        public string ErstelleBericht()
            => $"{berichtName} {uhrzeitProvider.AktuelleZeit():O}";
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

    /// <summary>
    /// Aggregiert mehrere Notifier und sendet eine Nachricht an alle.
    /// </summary>
    public class AggregatNotifier(IEnumerable<INotifier> notifiers)
    {
        public void SendeAnAlle(string nachricht)
        {
            Console.WriteLine("-- AggregatNotifier sendet an alle --");
            foreach (var n in notifiers)
                n.Senden(nachricht);
        }
    }

    public interface ICalculatorService
    {
        int Add(int a, int b);
    }

    public class CalculatorService : ICalculatorService
    {
        public int Add(int a, int b) => a + b;
    }

    public interface IDataValidator
    {
        bool IsValid(string data);
    }

    public class DataValidator : IDataValidator
    {
        public bool IsValid(string data) => !string.IsNullOrWhiteSpace(data) && data.Contains('@');
    }
}
