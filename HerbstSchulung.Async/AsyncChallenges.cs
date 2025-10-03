namespace HerbstSchulung.Async;

/// <summary>
/// Herausforderungen/Aufgaben für Teilnehmer. Jede Aufgabe enthält:
/// - eine fehlerhafte Implementierung
/// - Hinweise zur Lösung
/// </summary>
public static class AsyncChallenges
{
    // Aufgabe 1 (Einsteiger): Fehlende await-Anweisung
    // Ziel: await hinzufügen und Fehler propagieren lassen
    public static async Task<string> LadeTextAsync(Uri uri)
    {
        // FEHLER: Der Task wird gestartet, aber nicht awaited -> mögliche Race Conditions/Fehler gehen verloren
        var t = new HttpClient().GetStringAsync(uri);
        return await Task.FromResult(t.Result);
    }

    // Aufgabe 2: Deadlock/Blockierung vermeiden
    // Ziel: Keine .Result/Wait() verwenden und ConfigureAwait sinnvoll einsetzen (Bibliothekskontext)
    public static string LadeTextSynchron_Falsch(Uri uri)
    {
        // FEHLER: .Result blockiert den Thread
        return new HttpClient().GetStringAsync(uri).Result;
    }

    // Aufgabe 3 (Fortgeschritten): ValueTask korrekt verwenden
    // Ziel: API-Formulierung und nur einfaches Await, synchroner Schnellpfad + asynchroner Pfad
    public static ValueTask<int> BerechneAsync(bool synchron)
    {
        if (synchron)
        {
            // ok: synchroner Schnellpfad
            return new ValueTask<int>(7);
        }
        // FEHLER: ValueTask mit Task.Run ohne ConfigureAwait und Mehrfachawait-Gefahr in Aufrufercode
        return new ValueTask<int>(Task.Run(async () => { await Task.Delay(5); return 7; }));
    }

    // Aufgabe 4 (Fortgeschritten): IAsyncDisposable korrekt implementieren
    // Ziel: Ressourcen richtig freigeben und DisposeAsync korrekt awaiten
    public sealed class AsyncStream : IAsyncDisposable
    {
        private readonly MemoryStream _buffer = new();
        private bool _disposed;

        public async Task WriteAsync(byte[] data, CancellationToken ct = default)
        {
            // FEHLER: Keine Cancellation-Abfrage, ConfigureAwait fehlt (Bibliothekscode), Dispose-Zustand wird ignoriert
            await Task.Delay(1, ct);
            await _buffer.WriteAsync(data, ct);
        }

        public ValueTask DisposeAsync()
        {
            // FEHLER: nicht asynchron, keine Idempotenz, Ressourcen nicht korrekt freigegeben
            _buffer.Dispose();
            return ValueTask.CompletedTask;
        }
    }

    // Aufgabe 5 (Bonus): Fehlerbehandlung und Cancellation in Pipelines
    // Ziel: Exceptions korrekt weitergeben, Cancellation honorieren, ConfigureAwait in Libs
    public static async Task<int> PipelineAsync(Func<Task<int>> step1, Func<int, Task<int>> step2, CancellationToken ct)
    {
        // FEHLER: Cancellation wird ignoriert, Exceptions werden geschluckt
        try
        {
            var a = await step1();
            var b = await step2(a);
            return a + b;
        }
        catch
        {
            return -1;
        }
    }
}
