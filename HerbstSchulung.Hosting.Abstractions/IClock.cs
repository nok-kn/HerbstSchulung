namespace HerbstSchulung.Hosting.Abstractions;

/// <summary>
/// Abstraktion für Zeit-Funktionalität, um Testbarkeit und DI zu zeigen.
/// </summary>
public interface IClock
{
    /// <summary>
    /// Liefert die aktuelle Uhrzeit (UTC).
    /// </summary>
    DateTime UtcNow { get; }
}
