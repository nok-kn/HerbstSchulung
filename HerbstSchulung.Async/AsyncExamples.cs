namespace HerbstSchulung.Async;

/// <summary>
/// Falsche (Anti-)Beispiele f�r asynchrone Programmierung.
/// Jedes Beispiel enth�lt eine kurze Erkl�rung, warum es problematisch ist, und ggf. eine korrigierte Variante.
/// </summary>
public static class AsyncExamples
{
    private static readonly HttpClient Http = new();

    // Beispiel 1: async void vermeiden
    // Problem: Ausnahmen propagieren nicht zum Aufrufer -> Absturz/Unbeobachtete Exceptions m�glich.
    // Au�erdem kann der Aufrufer nicht warten (await) oder Fehler abfangen.
    public static async void DoWorkAsyncVoid_Fehler()
    {
        await Task.Delay(10);
        throw new InvalidOperationException("Fehler: async void Methode wirft Ausnahme, die nicht beobachtbar ist.");
    }

    // Korrektur: Immer Task zur�ckgeben, damit der Aufrufer await nutzen kann.
    public static async Task DoWorkAsync_Korrekt()
    {
        await Task.Delay(10);
        // Ausnahme ist nun beobachtbar und kann vom Aufrufer mittels try/catch behandelt werden.
        throw new InvalidOperationException("Ausnahme ist beobachtbar.");
    }
    
    private static void Empfehlung()
    {
        // Einmalig beim App-Start registrieren
        TaskScheduler.UnobservedTaskException += (sender, e) =>
        {
            foreach (var ex in e.Exception.InnerExceptions)
            {
                Console.WriteLine($"Unbeobachtete Exception: {ex.Message}");
                // Logging, Telemetrie, etc.
            }

            // Verhindert App-Crash (optional)
            e.SetObserved();
        };
    }

    // Beispiel 2: Fehlendes await -> Fire-and-Forget ohne Absicht
    // Problem: Der Fehler im inneren Task geht verloren, und die Methode signalisiert f�lschlich Fertigstellung.
    public static Task FehlendesAwait()
    {
        InnereArbeitAsync(); // ignorierter Task
        return Task.CompletedTask; // suggeriert: alles fertig
    }

    private static async Task InnereArbeitAsync()
    {
        await Task.Delay(10);
        throw new Exception("Fehler in InnereArbeitAsync");
    }

    // Korrektur: await verwenden, damit Fehler propagieren und Aufrufer warten kann
    public static async Task FehlendesAwait_Korrekt()
    {
        await InnereArbeitAsync();
    }

    // Beispiel 3: Blockierung des Threads via .Result / .Wait()
    // Problem: Deadlocks und Threadpool-Blockierung m�glich, besonders in Synchronisationskontexten (WPF/ASP.NET alt)
    public static string HttpResult_Blockiert_Fehler(Uri uri)
    {
        // Achtung: .Result blockiert synchron und kann Deadlocks verursachen
        return Http.GetStringAsync(uri).Result;
    }

    // Korrektur: durchg�ngig asynchron arbeiten
    public static async Task<string> HttpResult_Asynchron_Korrekt(Uri uri)
    {
        return await Http.GetStringAsync(uri);
    }

    // Beispiel 4: Bibliothekscode ohne ConfigureAwait(false)
    // Problem: In Bibliotheken sollten �blicherweise keine Synchronisationskontexte eingefangen werden, um Deadlocks zu vermeiden.
    public static async Task<string> LibraryCall_ohne_ConfigureAwait_Fehler(Uri uri)
    {
        var s = await Http.GetStringAsync(uri); 
        return s;
    }

    // Korrektur: In Bibliotheken ConfigureAwait(false) verwenden
    public static async Task<string> LibraryCall_mit_ConfigureAwait_Korrekt(Uri uri)
    {
        var s = await Http.GetStringAsync(uri).ConfigureAwait(false);
        return s;
    }

    // Beispiel 5: IAsyncDisposable falsch verwendet (DisposeAsync nicht awaited)
    public static async Task AsyncDisposable_Fehler()
    {
        var res = new RessourcenContainer();
        try
        {
            await res.SchreibeAsync("test");
        }
        finally
        {
            // Fehler: DisposeAsync nicht awaited 
            res.DisposeAsync(); // NICHT MACHEN
        }
    }

    // Korrektur: DisposeAsync immer awaiten oder await using verwenden
    public static async Task AsyncDisposable_Korrekt()
    {
        await using var res = new RessourcenContainer();
        await res.SchreibeAsync("test");
        // hier sorgt await using daf�r, dass DisposeAsync korrekt awaited wird
    }

}

/// <summary>
/// Beispielressource, die asynchron freigegeben werden muss.
/// </summary>
public sealed class RessourcenContainer : IAsyncDisposable
{
    private bool _disposed;

    public async Task SchreibeAsync(string text)
    {
        if (_disposed) throw new ObjectDisposedException(nameof(RessourcenContainer));
        await Task.Delay(1).ConfigureAwait(false);
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed) return;
        _disposed = true;
        // simuliert asynchrone Freigabe
        await Task.Delay(1).ConfigureAwait(false);
    }
}