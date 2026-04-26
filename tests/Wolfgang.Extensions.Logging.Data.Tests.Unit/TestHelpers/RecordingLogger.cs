using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace Wolfgang.Extensions.Logging.Data.Tests.Unit.TestHelpers;

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
        Entries.Add(new LogEntry(logLevel, eventId, message, exception, values));
    }
}
