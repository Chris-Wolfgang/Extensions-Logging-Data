using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace Wolfgang.Extensions.Logging.Data.Tests.Unit.TestHelpers;

internal sealed class LogEntry
{
    public LogEntry
    (
        LogLevel level,
        EventId eventId,
        string message,
        Exception? exception,
        IReadOnlyList<KeyValuePair<string, object?>>? values
    )
    {
        Level = level;
        EventId = eventId;
        Message = message;
        Exception = exception;
        Values = values;
    }

    public LogLevel Level { get; }

    public EventId EventId { get; }

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
