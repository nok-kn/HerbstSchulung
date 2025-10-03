namespace HerbstSchulung.Async;

/// <summary>
/// Herausforderungen/Aufgaben für Teilnehmer. 
/// </summary>
public static class AsyncChallenges
{
    // Aufgabe 1: Korrigiere/verbessere fehlerhafte/nicht ganz Optimale Implementierung
    public static async Task<string> Connect(string connectionString)
    {
        var t = new DbContex().ConnectToDatabase(connectionString);
        return await Task.FromResult(t.Result);
    }

    // Aufgabe 2: lock + async = Problem
    // Ein direkter await innerhalb eines lock ist in C# nicht erlaubt (Compilerfehler).
    // Deswegen greifen viele fälschlich zu .Result/.Wait() – genau das soll hier vermieden werden.
    // HINWEIS: Ersetze den lock durch einen asynchronen Mechanismus(z.B.SemaphoreSlim)
    public static int LockUndAsync_Blockiert_Falsch()
    {
        lock (Gate)
        {
            //.Result blockiert synchron, während der Monitor gehalten wird.
            // Das kann Deadlocks verursachen (z. B. wenn die Fortsetzung versucht, den gleichen Monitor/Synchronisationskontext zu nutzen)
            return LadeDatenAsync().Result;
        }
    }

    private static readonly object Gate = new();

    private static async Task<int> LadeDatenAsync()
    {
        await Task.Delay(50).ConfigureAwait(false);
        return 123;
    }
}

public class DbContex
{
    public Task<string> ConnectToDatabase(object stringConnectionString, CancellationToken cancellation = default)
    {
        return Task.FromResult("Connected!");
    }
}
