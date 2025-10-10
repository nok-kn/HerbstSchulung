using HerbstSchulung.Hosting.Abstractions;
using Microsoft.Extensions.Logging;

namespace HerbstSchulung.Hosting.Services;

/// <summary>
/// Interne Implementierung von IGreeter.
/// </summary>
internal sealed class Greeter(ILogger<Greeter> logger, IClock clock) : IGreeter
{
    public string Greet(string? name)
    {
        // Beispiel-Logeintrag auf Information
        logger.LogInformation("Erzeuge Begrüßung für {Name} um {Time}", name ?? "<null>", clock.UtcNow);
        var who = string.IsNullOrWhiteSpace(name) ? "Entwickler" : name.Trim();
        return $"Hallo {who}! Zeit (UTC): {clock.UtcNow:O}";
    }
}
