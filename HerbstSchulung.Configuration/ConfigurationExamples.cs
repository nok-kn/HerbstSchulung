using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace HerbstSchulung.Configuration
{
    /// <summary>
    /// Beispiele für die Verwendung von Microsoft.Extensions.Configuration und Options.
    /// </summary>
    public class ConfigurationExamples
    {
        /// <summary>
        /// Beispiel für das Registrieren und Einlesen von Konfiguration.
        /// </summary>
        public void KonfigurationEinlesen()
        {
            // Erstellen einer Konfiguration aus appsettings.json
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true)
                .Build();

            // Zugriff auf einen Wert mit Indexer
            var wert = configuration["BeispielEinstellung:Wert"]; 
            
            Console.WriteLine($"MeineEinstellung:Wert = {wert}");
            
            // oder
            wert = configuration.GetSection("BeispielEinstellung")["Wert"];
            
            Console.WriteLine($"MeineEinstellung:Wert = {wert}");
        }

        /// <summary>
        /// Beispiel für die Registrierung Options in DI
        /// </summary>
        public void Options()
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true)
                .Build();

            var services = new ServiceCollection();
            services.AddSingleton<MeineService>();

            // Binding configuration => C# Klasse
            services.Configure<MeineOptionen>(configuration.GetSection(nameof(MeineOptionen)));
            
            var provider = services.BuildServiceProvider();
            
            var service = provider.GetRequiredService<MeineService>();
            Console.WriteLine($"Name: {service.Name}");
        }

        /// <summary>
        /// Demonstriert die Unterschiede zwischen IOptions, IOptionsSnapshot und IOptionsMonitor
        ///
        /// - IOptions:            Für Optionen die statisch bleiben. Der Wert wird beim ersten Zugriff(nach Build) hergestellt und ändert sich zur Laufzeit nicht.
        ///
        /// - IOptionsSnapshot:    Scoped. Jeder neue Scope erzeugt eine neue, erneut gebundene Instanz.
        ///                        Änderungen an der Konfiguration zwischen Scopes werden so sichtbar.
        ///                        Innerhalb eines Scopes bleibt der Wert unverändert.
        /// 
        /// - IOptionsMonitor:     Beobachtet Änderungen(Reload) und aktualisiert den Wert sofort.
        ///                        Zusätzlich können OnChange-Callbacks registriert werden,  um auf Änderungen zu reagieren.
        ///                        Geeignet für dynamische Laufzeit-Anpassungen ohne neuen Scope.
        /// 
        /// </summary>
        public void OptionenVarianten()
        {
            var startDaten = new Dictionary<string, string?>
            {
                ["MeineOptionen:Name"] = "Alice"
            };

            var configuration = new ConfigurationBuilder()
                // Wir verwenden eine In-Memory-Konfiguration, damit wir die Werte zur Laufzeit ändern können
                .AddInMemoryCollection(startDaten)
                .Build(); // IConfigurationRoot

            var services = new ServiceCollection();

            services.Configure<MeineOptionen>(configuration.GetSection(nameof(MeineOptionen)));

            // Services, die verschiedene Varianten injizieren
            services.AddTransient<MeineService>();                  // nutzt IOptions (Singleton-Snapshot beim Container-Bau)
            services.AddTransient<MeineSnapshotService>();          // nutzt IOptionsSnapshot (pro Scope neu)
            services.AddSingleton<MeineMonitorCallbackCollector>();
            services.AddSingleton<MeineMonitorService>();           // nutzt IOptionsMonitor (aktualisiert + OnChange)

            var provider = services.BuildServiceProvider();

            using (var scope1 = provider.CreateScope())
            {
                var optionService1 = scope1.ServiceProvider.GetRequiredService<MeineService>();
                var snapshotService1 = scope1.ServiceProvider.GetRequiredService<MeineSnapshotService>();
                var monitorService1 = scope1.ServiceProvider.GetRequiredService<MeineMonitorService>();

                Console.WriteLine($"IOptions      : {optionService1.Name}");
                Console.WriteLine($"IOptionsSnapshot (Scope1): {snapshotService1.Name}");
                Console.WriteLine($"IOptionsMonitor: {monitorService1.NameAktuell}");
            }

            // Änderung der Konfiguration zur Laufzeit
            configuration["MeineOptionen:Name"] = "Bob"; // Triggert Reload für IOptionsMonitor + neue Snapshots
            Console.WriteLine();
            Console.WriteLine("Nach Konfigurationsänderung (Name=Bob)");

            using (var scope2 = provider.CreateScope())
            {
                var optionService = scope2.ServiceProvider.GetRequiredService<MeineService>();
                var snapshotService2 = scope2.ServiceProvider.GetRequiredService<MeineSnapshotService>();
                var monitorService2 = scope2.ServiceProvider.GetRequiredService<MeineMonitorService>();

                Console.WriteLine($"IOptions      (unverändert): {optionService.Name}");
                Console.WriteLine($"IOptionsSnapshot (Scope2)  : {snapshotService2.Name}");
                Console.WriteLine($"IOptionsMonitor (aktuell)  : {monitorService2.NameAktuell}");
            }

            var collector = provider.GetRequiredService<MeineMonitorCallbackCollector>();
            Console.WriteLine();
            Console.WriteLine("Monitor OnChange Callback Historie:");
            foreach (var eintrag in collector.Aenderungen)
            {
                Console.WriteLine(eintrag);
            }

        }
    }

    /// <summary>
    /// Beispiel-Optionsklasse für die Bindung und Validierung.
    /// </summary>
    public class MeineOptionen
    {
        /// <summary>
        /// Name-Eigenschaft, wird aus der Konfiguration gebunden.
        /// </summary>
        public string Name { get; set; } = string.Empty;
    }

    public class MeineService
    {
        private readonly IOptions<MeineOptionen> _options;

        public MeineService(IOptions<MeineOptionen> options)
        {
            _options = options;
        }

        public string Name => _options.Value.Name;
    }

    public class MeineSnapshotService
    {
        private readonly IOptionsSnapshot<MeineOptionen> _optionsSnapshot;

        public MeineSnapshotService(IOptionsSnapshot<MeineOptionen> optionsSnapshot)
        {
            _optionsSnapshot = optionsSnapshot;
        }

        public string Name => _optionsSnapshot.Value.Name;
    }

    public class MeineMonitorService : IDisposable
    {
        private readonly IOptionsMonitor<MeineOptionen> _monitor;
        
        private readonly IDisposable? _onChangeDisposable;

        public MeineMonitorService(IOptionsMonitor<MeineOptionen> monitor, MeineMonitorCallbackCollector collector)
        {
            _monitor = monitor;
            _onChangeDisposable = _monitor.OnChange(o => collector.Aenderungen.Add($"OnChange -> Neuer Name: {o.Name}"));
        }

        public string NameAktuell => _monitor.CurrentValue.Name;

        // wenn MeineMonitorService als Transient registriert ist, sollte OnChange Handler in Dispose entfernt werden
        public void Dispose()
        {
            _onChangeDisposable?.Dispose();
        }
    }

    public class MeineMonitorCallbackCollector
    {
        public List<string> Aenderungen { get; } = new();
    }
}
