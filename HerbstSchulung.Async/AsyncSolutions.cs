using System.Diagnostics;

namespace HerbstSchulung.Async;

/// <summary>
/// Beispiel-Lösungen und Erklärungen zu den Herausforderungen.
/// </summary>
public static class AsyncSolutions
{
    // Lösung 1: await hinzufügen und HttpClient korrekt verwalten
    public static async Task<string> LadeTextAsync_Loesung(Uri uri)
    {
        // In echten Anwendungen: HttpClient wiederverwenden/injizieren
        using var http = new HttpClient();
        // In Bibliotheken: ConfigureAwait(false) nutzen
        var text = await http.GetStringAsync(uri).ConfigureAwait(false);
        return text;
    }

    // Lösung 2: Durchgängig asynchron + ConfigureAwait(false)
    public static async Task<string> LadeTextSynchron_Loesung(Uri uri)
    {
        using var http = new HttpClient();
        return await http.GetStringAsync(uri).ConfigureAwait(false);
    }

    // Lösung 3: ValueTask korrekt nutzen – nur für synchronen Schnellpfad
    public static ValueTask<int> BerechneAsync_Loesung(bool synchron)
    {
        if (synchron)
        {
            return new ValueTask<int>(7);
        }
        // Für asynchrone Pfade einfach Task nutzen (keine Mehrfachawait-Gefahr)
        return new ValueTask<int>(BerechneAsyncImpl());

        static async Task<int> BerechneAsyncImpl()
        {
            await Task.Delay(5).ConfigureAwait(false);
            return 7;
        }
    }

    // Lösung 4: IAsyncDisposable korrekt implementieren
    public sealed class AsyncStream : IAsyncDisposable
    {
        private readonly MemoryStream _buffer = new();
        private bool _disposed;

        public async Task WriteAsync(byte[] data, CancellationToken ct = default)
        {
            ObjectDisposedException.ThrowIf(_disposed, nameof(AsyncStream));
            ct.ThrowIfCancellationRequested();
            await _buffer.WriteAsync(data, ct).ConfigureAwait(false);
        }

        public async ValueTask DisposeAsync()
        {
            if (_disposed) return;
            _disposed = true;
            // Beispiel: Flush simulieren
            await Task.Yield();
            _buffer.Dispose();
        }
    }

    // Lösung 5: Fehlerbehandlung + Cancellation korrekt
    public static async Task<int> PipelineAsync_Loesung(Func<CancellationToken, Task<int>> step1, Func<int, CancellationToken, Task<int>> step2, CancellationToken ct)
    {
        // Cancellation früh prüfen
        ct.ThrowIfCancellationRequested();

        var a = await step1(ct).ConfigureAwait(false);
        ct.ThrowIfCancellationRequested();
        var b = await step2(a, ct).ConfigureAwait(false);
        return a + b;
    }
}
