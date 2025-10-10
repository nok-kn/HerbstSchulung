namespace HerbstSchulung.Hosting.Abstractions;

/// <summary>
/// Abstraktion f�r Zeit-Funktionalit�t, um Testbarkeit und DI zu zeigen.
/// </summary>
public interface IClock
{
    /// <summary>
    /// Liefert die aktuelle Uhrzeit (UTC).
    /// </summary>
    DateTime UtcNow { get; }
}
