namespace HerbstSchulung.DependencyInjection;

using Microsoft.Extensions.DependencyInjection; 

public static class DependencyInjectionChallenges
{
    // Aufgabe 1: 
    // Problem: Manuelle Instanziierung statt Nutzung des Containers erschwert Testbarkeit
    // Diese Klasse erzeugt ihre Abhängigkeiten selbst. 
    // Wie würdest Du die Klasse umbauen, damit sie testbar wird?
    public class ReportGenerator
    {
        private readonly RaportService _timeService = new();

        public string Generate(string title)
        {
            return _timeService.GetReportText();
        }
    }

    public class RaportService
    {
        public string GetReportText() => "Report";
    }

    // Aufgabe 2: 
    // Problem: Service Locator Anti-Pattern durch direkten Zugriff auf IServiceProvider in Methoden.
    // Wie kann man die versteckte Abhängigkeit entfernen?
    public class BadServiceLocatorUsage
    {
        private readonly IServiceProvider _provider;

        public BadServiceLocatorUsage(IServiceProvider provider) => _provider = provider; 

        public Guid Handle()
        {
            // HINWEIS: Anti-Pattern 
            var guidProvider = _provider.GetRequiredService<DependencyInjectionExamples.IGuidProvider>();
            return guidProvider.AktuellerGuid;
        }
    }

    // Aufgabe 3: 
    // Verwende in deinem Projekt um DI zu testen


}
