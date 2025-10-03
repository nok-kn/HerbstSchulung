namespace HerbstSchulung.Async
{
    /// <summary>
    /// Demonstriert Synchronisationskontext (z. B. WPF) und die Rolle des Dispatchers.
    /// Hinweis: Diese Klasse simuliert WPF-Verhalten; in echten WPF-Apps nutzt man Application.Current.Dispatcher.
    /// </summary>
    public class DispatcherProblematik
    {
        public async Task WPF_HatSynchronisationkontext()
        {
            // In WPF existiert ein Synchronisationskontext (UI-Thread). await kehrt standardmäßig dorthin zurück
            await LadeDaten();

            // Hier bist du wieder im UI-Thread
            MeineWPFAnwendung.StatusText = "Aktualisiert im UI Thread!";
        }

        public async Task WozuIsDispatcher()
        {
            // CPU-Arbeit im Hintergrund-Thread; danach UI-Update sicher über Dispatcher
            await Task.Run(() =>
            {
                // Hier bist du NICHT mehr im UI-Thread
                // UI-Zugriff ohne Dispatcher würde crashen (in WPF)
                Dispatcher.Invoke(() => { MeineWPFAnwendung.StatusText = "Vom Hintergrundthread aktualisiert!"; });
            });
            
            // oder ...
            // In Bibliotheken oder zur Vermeidung unnötiger Kontextwechsel: ConfigureAwait(false)
            // Dadurch kehrt die Fortsetzung NICHT automatisch zum UI-Thread zurück.
            await LadeDaten().ConfigureAwait(false);

            Dispatcher.Invoke(() => { MeineWPFAnwendung.StatusText = "Per Dispatcher nach ConfigureAwait(false) aktualisiert!"; });
        }

        private async Task LadeDaten()
        {
            await Task.Delay(1000);
        }
    }

    /// <summary>
    /// Im echten WPF würde man den UI-Thread-Dispatcher verwenden. Hier nur Simulation.
    /// </summary>
    internal static class Dispatcher
    {
        public static bool IsOnUiThread { get; set; }

        /// <summary>
        /// Simuliertes Invoke 
        /// </summary>
        public static void Invoke(Action action)
        {
            if (IsOnUiThread)
            {
                action();
            }
            else
            {
                // In echter WPF: zum UI-Thread marshallen (Dispatcher.Invoke/BeginInvoke)
                // Demo: direkte Ausführung als Platzhalter
                action();
            }
        }
    }

    internal static class MeineWPFAnwendung
    {
        public static object? StatusText;
    }
}
