using System.Diagnostics;

namespace HerbstSchulung.Async;

/// <summary>
/// Asynchron in Property-Settern: Kein async void! 
/// Korrektes Muster: Eine explizite Async-Methode anbieten, die awaited werden kann.
/// Testbarkeit: Methode liefert Task und hat beobachtbare Zustände.
/// </summary>
public class AsyncPropertySetterExample
{
    private async Task LadeDatenAsync(string value)
    {
        // Simulation I/O
        await Task.Delay(5).ConfigureAwait(false);
    }

    private string _name = string.Empty;

    // FALSCHES Muster 
    public string Name
    {
        get => _name;
        set
        {
            if (value != _name)
            {
                _name = value;
                _ = LadeDatenAsync(value); // Fire-and-Forget im Setter
            }
        }
    }
    
    // Korrektur 1: Explizite Async-Methode statt Setter

    private string? _name1;

    public string? Name1 => _name1;
    
    public async Task SetName1Async(string value)
    {
        _name1 = value;
        await LadeDatenAsync(value);
    }

    // Korrektur 2: Testbare und "safe" async void, aber (!): 
    //	- Aufrufer können die Fertigstellung nicht awaiten
    //  - es kann zu Race-Conditions kommen, wenn der Setter schnell hintereinander aufgerufen wird.
    private string? _name2;

    public string? Name2
    {
        get => _name2;
        set
        {
            if (value != _name2)
            {
                _name2 = value;
                OnName2Changed();
            }
        }
    }

    private async void OnName2Changed()
    {
        try
        {
            await OnName2ChangedAsync();
        }
        catch (Exception e)
        {
            Trace.Write(e);
        }
    }

    internal async Task OnName2ChangedAsync()
    {
        await LadeDatenAsync(Name2);
    }


}