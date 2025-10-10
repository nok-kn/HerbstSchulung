namespace HerbstSchulung.Hosting.Abstractions;

/// <summary>
/// Einfache Begr��ungs-Schnittstelle, um DI und Logging zu demonstrieren.
/// </summary>
public interface IGreeter
{
    /// <summary>
    /// Gibt eine lokalisierte Begr��ung zur�ck.
    /// </summary>
    string Greet(string? name);
}
