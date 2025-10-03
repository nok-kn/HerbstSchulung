using System.Diagnostics;

namespace HerbstSchulung.Async;

/// <summary>
/// Korrekte Nutzung von async void für Event-Handler. 
/// Testbarkeit: Die eigentliche Logik liegt in einer internen Task-Methode, die separat getestet werden kann.
/// </summary>
public static class AsyncEventHandlerExample
{
    // FALSCH: Event-Handler mit Logik, die Exceptions ungefangen lässt
    public static async void Button_Click_Fehler(object? sender, EventArgs e)
    {
        await Task.Delay(10);
        throw new InvalidOperationException("Fehler im Click-Handler");
    }

    // KORREKT: async void als Event-Handler, aber die Logik in eine Task-Methode auslagern.
    // So kann die Logik getestet werden, indem die Impl direkt aufgerufen/awaited wird.
    public static async void Button_Click_Korrekt(object? sender, EventArgs e)
    {
        try
        {
            await ButtonClickImplAsync();
        }
        catch (Exception ex)
        {
            // Beispiel: Logging/Fehlerdialog etc. – hier nur Trace
            Trace.WriteLine($"Fehler im Event-Handler: {ex.Message}");
        }
    }

    public static async Task ButtonClickImplAsync()
    {
        await Task.Delay(5);
        // eigentliche Logik
    }
}