using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace HerbstSchulung.EntityFramework.Tests;

/// <summary>
/// Extension methods f�r das Hinzuf�gen von xUnit Test Output Logging.
/// </summary>
public static class XunitLoggerExtensions
{
    /// <summary>
    /// F�gt einen xUnit Test Output Logger zum Logging-Builder hinzu.
    /// </summary>
    /// <param name="builder">Der ILoggingBuilder.</param>
    /// <param name="testOutputHelper">Der xUnit ITestOutputHelper.</param>
    /// <returns>Der ILoggingBuilder f�r Method-Chaining.</returns>
    public static ILoggingBuilder AddXunitTestOutput(this ILoggingBuilder builder, ITestOutputHelper testOutputHelper)
    {
        return builder.AddProvider(new XunitLoggerProvider(testOutputHelper));
    }
}

/// <summary>
/// Logger Provider f�r xUnit Test Output.
/// </summary>
internal sealed class XunitLoggerProvider : ILoggerProvider
{
    private readonly ITestOutputHelper _testOutputHelper;

    public XunitLoggerProvider(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    public ILogger CreateLogger(string categoryName)
    {
        return new XunitLogger(_testOutputHelper, categoryName);
    }

    public void Dispose()
    {
        // Nichts zu freigeben
    }
}

/// <summary>
/// Logger Implementierung, die in xUnit Test Output schreibt.
/// </summary>
internal sealed class XunitLogger : ILogger
{
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly string _categoryName;

    public XunitLogger(ITestOutputHelper testOutputHelper, string categoryName)
    {
        _testOutputHelper = testOutputHelper;
        _categoryName = categoryName;
    }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        return null;
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return logLevel != LogLevel.None;
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel))
        {
            return;
        }

        try
        {
            var message = formatter(state, exception);
            var logEntry = $"[{logLevel}] {_categoryName}: {message}";
            
            if (exception != null)
            {
                logEntry += Environment.NewLine + exception;
            }

            _testOutputHelper.WriteLine(logEntry);
        }
        catch
        {
            // ITestOutputHelper kann au�erhalb des Test-Kontexts Exceptions werfen
            // Ignorieren, damit Tests nicht fehlschlagen
        }
    }
}
