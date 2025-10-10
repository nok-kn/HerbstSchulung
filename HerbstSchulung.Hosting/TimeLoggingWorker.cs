using HerbstSchulung.Hosting.Abstractions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace HerbstSchulung.Hosting;

/// <summary>
/// Ein einfacher BackgroundService, der periodisch die Zeit protokolliert.
/// </summary>
public sealed class TimeLoggingWorker(ILogger<TimeLoggingWorker> logger, IClock clock) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Einfache Endlosschleife mit Abbruchunterstützung
        while (!stoppingToken.IsCancellationRequested)
        {
            logger.LogDebug("Aktuelle Zeit (UTC): {UtcNow}", clock.UtcNow);
            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        }
    }
}
