using HerbstSchulung.Hosting.Abstractions;

namespace HerbstSchulung.Hosting.Services;

/// <summary>
/// Interne Implementierung von IClock auf Basis von Systemzeit.
/// </summary>
internal sealed class SystemClock : IClock
{
    public DateTime UtcNow => DateTime.UtcNow;
}
