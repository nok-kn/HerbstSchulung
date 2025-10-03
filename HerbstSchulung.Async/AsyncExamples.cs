using System.Diagnostics;

namespace HerbstSchulung.Async;

/// <summary>
/// Falsche (Anti-)Beispiele für asynchrone Programmierung.
/// Jedes Beispiel enthält eine kurze Erklärung, warum es problematisch ist, und ggf. eine korrigierte Variante.
/// </summary>
public static class AsyncExamples
{
    private static readonly HttpClient Http = new();

    // Beispiel 1: async void vermeiden
    // Problem: Ausnahmen propagieren nicht zum Aufrufer -> Absturz/Unbeobachtete Exceptions möglich.
    // Außerdem kann der Aufrufer nicht warten (await) oder Fehler abfangen.
    public static async void DoWorkAsyncVoid_Fehler()
    {
        await Task.Delay(10);
        throw new InvalidOperationException("Fehler: async void Methode wirft Ausnahme, die nicht beobachtbar ist.");
    }

    // Korrektur: Immer Task zurückgeben, damit der Aufrufer await nutzen kann.
    public static async Task DoWorkAsync_Korrekt()
    {
        await Task.Delay(10);
        // Ausnahme ist nun beobachtbar und kann vom Aufrufer mittels try/catch behandelt werden.
        throw new InvalidOperationException("Ausnahme ist beobachtbar.");
    }

    // Beispiel 2: Fehlendes await -> Fire-and-Forget ohne Absicht
    // Problem: Der Fehler im inneren Task geht verloren, und die Methode signalisiert fälschlich Fertigstellung.
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
    // Problem: Deadlocks und Threadpool-Blockierung möglich, besonders in Synchronisationskontexten (WPF/ASP.NET alt)
    public static string HttpResult_Blockiert_Fehler(Uri uri)
    {
        // Achtung: .Result blockiert synchron und kann Deadlocks verursachen
        return Http.GetStringAsync(uri).Result;
    }

    // Korrektur: durchgängig asynchron arbeiten
    public static async Task<string> HttpResult_Asynchron_Korrekt(Uri uri)
    {
        return await Http.GetStringAsync(uri);
    }

    // Beispiel 4: Bibliothekscode ohne ConfigureAwait(false)
    // Problem: In Bibliotheken sollten üblicherweise keine Synchronisationskontexte eingefangen werden, um Deadlocks zu vermeiden.
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

    // Beispiel 5: ValueTask falsch eingesetzt
    // Problem 1: ValueTask ist kein Ersatz für Task und sollte nur mit Bedacht genutzt werden (Overhead/Fehlerpotenzial).
    // Problem 2: Ein ValueTask darf nur einmal awaited werden. Mehrfaches Await führt zu Fehlern.
    public static ValueTask<int> LiefereZahl_ValueTask_Fehlerhaft(bool synchron)
    {
        if (synchron)
        {
            // Hier ist ValueTask sinnvoll (synchrones Ergebnis). Aber Vorsicht: Aufrufer darf nur einmal awaiten.
            return new ValueTask<int>(42);
        }
        else
        {
            // Hier wird unnötig ValueTask statt Task verwendet. Besser: Task bei rein asynchronem Pfad.
            return new ValueTask<int>(Task.Run(async () => { await Task.Delay(5); return 21 + 21; }));
        }
    }

    // Korrektur: API klar halten. Wenn häufig synchron: ValueTask, sonst Task. Niemals mehrfach awaiten.
    public static async Task<int> LiefereZahl_Task_Korrekt()
    {
        await Task.Yield();
        return 42;
    }

    // Beispiel 6: IAsyncDisposable falsch verwendet (DisposeAsync nicht awaited)
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
        // hier sorgt await using dafür, dass DisposeAsync korrekt awaited wird
    }

    // Beispiel 7: Fehlerbehandlung in async void (Ausnahmen gehen verloren)
    public static async void FehlerBehandlung_async_void_Fehler()
    {
        try
        {
            await Task.Delay(5);
            throw new ApplicationException("Boom");
        }
        catch
        {
            // swallow
        }
    }

    public static async Task FehlerBehandlung_Task_Korrekt()
    {
        try
        {
            await Task.Delay(5);
            throw new ApplicationException("Boom");
        }
        catch (Exception ex)
        {
            // sinnvoll: Logging/Wrap/Weiterwerfen
            Trace.WriteLine($"Fehler korrekt behandelt: {ex.Message}");
            throw;
        }
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