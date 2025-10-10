namespace HerbstSchulung.Async;

/// <summary>
/// Herausforderungen/Aufgaben für Teilnehmer. 
/// </summary>
public class AsyncChallenges
{
    //------------------------------------------------------------------------------------------------------------------
    // Aufgabe 1: Korrigiere/verbessere fehlerhafte/nicht ganz optimale Implementierung
    public async Task<string> Connect(string connectionString)
    {
        var t = new DbContex().ConnectToDatabase(connectionString);
        return await Task.FromResult(t.Result);
    }

    private class DbContex : IDisposable
    {
        public Task<string> ConnectToDatabase(object stringConnectionString, CancellationToken cancellation = default)
        {
            return Task.FromResult("Connected!");
        }

        public void Dispose()
        {
            // ...
        }
    }

    //------------------------------------------------------------------------------------------------------------------
    // Aufgabe 2: Korrigiere/verbessere fehlerhafte/nicht ganz optimale Implementierung
    public async Task<string> TikTok()
    {
        await Task.Factory.StartNew(async () => await DownloadExcitingVideosFromTikTok());
        return "I'm ready";
    }

    private Task DownloadExcitingVideosFromTikTok()
    {
        return Task.Delay(10000);
    }



    //------------------------------------------------------------------------------------------------------------------
    // Aufgabe 3: Korrigiere/verbessere fehlerhafte/nicht ganz optimale Implementierung
    public async Task FindCatsInVideos()
    {
        var videos = new List<Video> { new(), new() };
        int catCount = 0;
        foreach (var v in videos)
        {
            catCount += await v.ContainsCat() ? 1 : 0;
        }
        Console.WriteLine($"Found {catCount} cat videos");
    }

    private class Video
    {
        public Task<bool> ContainsCat() => Task.FromResult(true);
    }
    
    //------------------------------------------------------------------------------------------------------------------
    // Aufgabe 4: lock + async = Problem
    // Ein direkter await innerhalb eines lock ist in C# nicht erlaubt (Compilerfehler).
    // Deswegen greifen viele fälschlich zu .Result/.Wait() – genau das soll hier vermieden werden.
    // HINWEIS: Ersetze den lock durch einen asynchronen Mechanismus(z.B.SemaphoreSlim)
    public int LockUndAsync()
    {
        lock (Gate)
        {
            //.Result blockiert synchron, während der Monitor gehalten wird.
            // Das kann Deadlocks verursachen (z. B. wenn die Fortsetzung versucht, den gleichen Monitor/Synchronisationskontext zu nutzen)
            return LadeDatenAsync().Result;
        }
    }

    private readonly object Gate = new();

    private async Task<int> LadeDatenAsync()
    {
        await Task.Delay(50);
        return 123;
    }

    //------------------------------------------------------------------------------------------------------------------
    // Aufgabe 5: 
    // Benutze CancellationToken, um eine langlaufende asynchrone Operation ProcessData nach 500ms vorzeitig abzubrechen.
    // Hinweis: CancellationTokenSource.CancelAfter kann genutzt werden, um den Abbruch nach einer bestimmten Zeit zu initiieren.
    public async Task WieKannManEineAsyncOperationAbbrechen()
    {
        var result = await ProcessData();
        Console.WriteLine(result);
    }
    
    private async Task<string> ProcessData(CancellationToken cancellationToken = default)
    {
        var data = new List<string>();
        string calculatedData = string.Empty;
        for (var i = 0; i < 1000; i++)
        {
            calculatedData = await CalculateData(calculatedData, i);
            data.Add(calculatedData);
        }
        return string.Join(",", data);
    }

    private Task<string> CalculateData(string previousData, int index, CancellationToken cancellationToken = default)
    {
        // Simuliere eine asynchrone Berechnung
        Task.Delay(100, cancellationToken);
        return Task.FromResult($"{previousData} Data{index}");
    }

}

