using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace Wolfgang.Extensions.Logging.Data.EntityFramework6.Tests.Unit.TestHelpers;

internal sealed class RecordingLogger : ILogger
{
    public List<LogEntry> Entries { get; } = new List<LogEntry>();

    public LogLevel MinimumLevel { get; set; } = LogLevel.Trace;

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

    public bool IsEnabled(LogLevel logLevel) => logLevel >= MinimumLevel;

    public void Log<TState>
    (
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter
    )
    {
        var message = formatter(state, exception);
        var values = state as IReadOnlyList<KeyValuePair<string, object?>>;
        Entries.Add(new LogEntry(logLevel, message, exception, values));
    }
}


internal sealed class LogEntry
{
    public LogEntry(LogLevel level, string message, Exception? exception, IReadOnlyList<KeyValuePair<string, object?>>? values)
    {
        Level = level;
        Message = message;
        Exception = exception;
        Values = values;
    }

    public LogLevel Level { get; }

    public string Message { get; }

    public Exception? Exception { get; }

    public IReadOnlyList<KeyValuePair<string, object?>>? Values { get; }

    public object? GetValue(string name)
    {
        if (Values is null)
        {
            return null;
        }

        foreach (var kvp in Values)
        {
            if (string.Equals(kvp.Key, name, StringComparison.Ordinal))
            {
                return kvp.Value;
            }
        }

        return null;
    }
}
