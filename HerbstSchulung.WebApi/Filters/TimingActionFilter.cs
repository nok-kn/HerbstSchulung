using Microsoft.AspNetCore.Mvc.Filters;

namespace HerbstSchulung.WebApi.Filters;

/// <summary>
/// Einfacher Action-Filter, der die Laufzeit von Aktionen misst und protokolliert.
/// </summary>
public sealed class TimingActionFilter : IActionFilter
{
    private readonly ILogger<TimingActionFilter> _logger;
    private readonly System.Diagnostics.Stopwatch _sw = new();

    public TimingActionFilter(ILogger<TimingActionFilter> logger)
    {
        _logger = logger;
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        _sw.Restart();
        _logger.LogDebug("Action {Action} gestartet", context.ActionDescriptor.DisplayName);
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        _sw.Stop();
        _logger.LogInformation("Action {Action} beendet in {Elapsed} ms", context.ActionDescriptor.DisplayName, _sw.ElapsedMilliseconds);
    }
}
