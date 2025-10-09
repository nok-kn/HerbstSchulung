namespace HerbstSchulung.Async;

/// <summary>
/// Asynchron im Konstruktor: Kein async void/kein .Result ñ stattdessen Async-Factory.
/// </summary>
public sealed class AsyncConstructorExample
{
    public bool Initialisiert { get; private set; }

    // FALSCH
    public AsyncConstructorExample()
    {
        InitAsync(); // Fire-and-Forget; Aufrufer weiﬂ nicht, wann fertig
    }
    
    // Factory liefert Task und garantiert initialisierten Zustand.
    public static async Task<AsyncConstructorExample> CreateAsync()
    {
        var repo = new AsyncConstructorExample();
        await repo.InitAsync();
        return repo;
    }

    private async Task InitAsync()
    {
        await Task.Delay(5); 
        Initialisiert = true;
    }
}