namespace HerbstSchulung.DependencyInjection;

using static HerbstSchulung.DependencyInjection.DependencyInjectionExamples; // F�r IGuidProvider
using Microsoft.Extensions.DependencyInjection; // F�r GetService<T>

/// <summary>
/// Sammlung von �bungsaufgaben (Challenges) rund um Dependency Injection.
/// Die Ideen: H�ufige Fehler, Anti-Pattern oder unvollst�ndige Implementierungen sollen von den Teilnehmern verbessert werden.
/// Jede Challenge mit Kommentar "AUFGABE" und optional "HINWEIS" versehen.
/// </summary>
public static class DependencyInjectionChallenges
{
    // AUFGABE 1:
    // Problem: Manuelle Instanziierung statt Nutzung des Containers erschwert Testbarkeit.
    // Aufgabe: Refaktorieren Sie die Klasse, so dass alle Abh�ngigkeiten via Konstruktor injiziert werden
    // und registrieren Sie diese korrekt (Interface + Implementierung). Entfernen Sie direkte new()-Aufrufe.
    public class ReportGenerator
    {
        // HINWEIS: Diese Klasse erzeugt ihre Abh�ngigkeiten selbst - Anti-Pattern (Service Locator / Control Freak)
        private readonly TimeService _timeService = new();
        private readonly GuidService _guidService = new();

        public string Generate(string title)
        {
            // HINWEIS: Schlecht testbar (Zeit + Guid variieren), sollte �ber Interfaces abstrahiert werden
            return $"[{_timeService.UtcNow():O}] ({_guidService.NewGuid()}) {title}";
        }
    }

    public class TimeService
    {
        public DateTime UtcNow() => DateTime.UtcNow; // AUFGABE: Interface extrahieren
    }

    public class GuidService
    {
        public Guid NewGuid() => Guid.NewGuid(); // AUFGABE: Interface extrahieren
    }

    // AUFGABE 2:
    // Problem: Falscher Lifetime gew�hlt -> teurer Dienst wird als Transient registriert und mehrfach aufgebaut.
    // Aufgabe: Entscheiden Sie anhand der Kommentare welchen Lifetime Sie w�hlen w�rden.
    public interface ITeurerCache
    {
        // Stellt teure Daten bereit
        IReadOnlyDictionary<string, string> Daten { get; }
    }

    // Annahme: Aufbau sehr teuer (z.B. Dateizugriff oder Netzwerk-Lookup) -> sollte nicht bei jedem Resolve neu erstellt werden.
    public class TeurerCache : ITeurerCache, IDisposable
    {
        public IReadOnlyDictionary<string, string> Daten { get; }

        public TeurerCache()
        {
            // Simulierter teurer Aufbau
            Daten = new Dictionary<string, string>
            {
                ["A"] = "Alpha",
                ["B"] = "Beta"
            };
        }

        public void Dispose()
        {
            // Ressourcenbereinigung falls n�tig
        }
    }

    // AUFGABE 3:
    // Problem: Capturing eines Scoped Service in einem Singleton f�hrt zu fehlerhaftem Verhalten / m�glichen Speicherlecks.
    // Aufgabe: Erkl�ren, warum das schlecht ist und wie man es korrigiert (Factory, Lazy, IServiceProvider.CreateScope, oder Umgestaltung).
    public interface IRequestContext
    {
        Guid RequestId { get; }
    }

    public class RequestContext : IRequestContext
    {
        public Guid RequestId { get; } = Guid.NewGuid();
    }

    // FALSCHES BEISPIEL
    public class SingletonLogger
    {
        // HINWEIS: Das hier w�re falsch, wenn IRequestContext Scoped registriert ist.
        private readonly IRequestContext _context; // AUFGABE: Wie l�sen?
        public SingletonLogger(IRequestContext context) => _context = context;

        public void Log(string nachricht)
            => Console.WriteLine($"[{_context.RequestId}] {nachricht}");
    }

    // AUFGABE 4:
    // Problem: Zirkul�re Abh�ngigkeit (w�rde zur Laufzeit eine Exception ausl�sen).
    // Aufgabe: Entfernen oder �ber Events / Mediator / Callback / IMessageBus aufl�sen.
    public interface IA
    {
        void DoA();
    }

    public interface IB
    {
        void DoB();
    }

    public class A : IA
    {
        private readonly IB _b; // AUFGABE: Pr�fen ob diese direkte Abh�ngigkeit n�tig ist
        public A(IB b) => _b = b;
        public void DoA() => _b.DoB();
    }

    public class B : IB
    {
        private readonly IA _a; // AUFGABE: Zirkul�r
        public B(IA a) => _a = a;
        public void DoB() => _a.DoA();
    }

    // AUFGABE 5:
    // Problem: Service Locator Anti-Pattern durch direkten Zugriff auf IServiceProvider in Methoden.
    // Aufgabe: Umgestalten auf reine Konstruktorinjektion.
    public class BadServiceLocatorUsage
    {
        private readonly IServiceProvider _provider;
        public BadServiceLocatorUsage(IServiceProvider provider) => _provider = provider; // AUFGABE: Entfernen

        public void Handle()
        {
            // HINWEIS: Anti-Pattern (versteckte Abh�ngigkeit)
            var guidProvider = _provider.GetService<IGuidProvider>(); // Beispiel: verdeckte Abh�ngigkeit
            _ = guidProvider; // unterdr�ckt Warnung
        }
    }
}
