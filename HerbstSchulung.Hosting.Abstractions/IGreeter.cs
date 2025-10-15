namespace HerbstSchulung.Hosting.Abstractions;

/// <summary>
/// Einfache Begrüßungs-Schnittstelle, um DI und Logging zu demonstrieren.
/// </summary>
public interface IGreeter
{
    /// <summary>
    /// Gibt eine lokalisierte Begrüßung zurück.
    /// </summary>
    string Greet(string? name);

    Task<string>  GreetAsync(string name, CancellationToken cancellationToken) => Task.FromResult(Greet(name));
}
