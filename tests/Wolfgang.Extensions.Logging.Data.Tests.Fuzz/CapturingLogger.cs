using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace Wolfgang.Extensions.Logging.Data.Tests.Fuzz;

/// <summary>
/// Minimal <see cref="ILogger"/> that keeps every entry's structured values so a property can
/// inspect exactly what the library handed to the logging pipeline.
/// </summary>
internal sealed class CapturingLogger : ILogger
{
    public List<IReadOnlyList<KeyValuePair<string, object?>>> Entries { get; } = new();



    public IDisposable? BeginScope<TState>(TState state)
        where TState : notnull => null;



    public bool IsEnabled(LogLevel logLevel) => true;



    public void Log<TState>
    (
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter
    )
    {
        Entries.Add(state as IReadOnlyList<KeyValuePair<string, object?>> ?? Array.Empty<KeyValuePair<string, object?>>());
    }



    public string Value(int entry, string name)
    {
        foreach (var kvp in Entries[entry])
        {
            if (string.Equals(kvp.Key, name, StringComparison.Ordinal))
            {
                return kvp.Value?.ToString() ?? string.Empty;
            }
        }

        throw new KeyNotFoundException(name);
    }
}
