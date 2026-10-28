using System;
using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Wolfgang.Extensions.Logging.Data.Benchmarks;

/// <summary>
/// Compares the two parameter-supplying styles of <see cref="DbCommandLoggerExtensions"/>:
///
///   - the explicit dictionary path (no reflection), and
///   - the Dapper-style anonymous-object path (reflection, cached per type).
///
/// Each is measured in two regimes:
///   - enabled logger (full render of the parameter set), and
///   - disabled logger (IsEnabled short-circuit — the dominant production case).
///
/// The anonymous-object path's per-type accessor cache means the reflection
/// cost is paid once per shape; the steady-state cost measured here should be
/// close to the dictionary path, and MemoryDiagnoser surfaces the extra
/// allocation the reflected dictionary introduces.
/// </summary>
[MemoryDiagnoser]
public class DbCommandLoggerExtensionsBenchmarks
{
    private static readonly ILogger EnabledLogger = new EnabledNoOpLogger();
    private static readonly ILogger DisabledLogger = NullLogger.Instance;
    private const string CommandText = "SELECT * FROM Users WHERE Id = @id AND Status = @status";

    private static readonly IReadOnlyDictionary<string, object?> DictParameters =
        new Dictionary<string, object?> { ["id"] = 42, ["status"] = "active", ["password"] = "hunter2" };

    private static readonly string[] Excluded = { "password" };

    // A live command for the LogDbCommand path (the one the EF6 interceptor calls for every
    // command). SqliteCommand needs no open connection to hold text and parameters.
    private static readonly SqliteCommand LiveCommand = CreateLiveCommand();



    [Benchmark(Baseline = true)]
    public void Dictionary_EnabledLogger()
    {
        EnabledLogger.LogCommandText(CommandText, DictParameters, Excluded);
    }



    [Benchmark]
    public void Dictionary_EnabledLogger_NoExclusions()
    {
        // The common case: the 2- and 3-argument overloads pass an empty exclusion list.
        EnabledLogger.LogCommandText(CommandText, DictParameters);
    }



    [Benchmark]
    public void LogDbCommand_EnabledLogger()
    {
        EnabledLogger.LogDbCommand(LiveCommand, Excluded);
    }



    [Benchmark]
    public void LogDbCommand_EnabledLogger_NoExclusions()
    {
        EnabledLogger.LogDbCommand(LiveCommand);
    }



    [Benchmark]
    public void AnonymousObject_EnabledLogger()
    {
        EnabledLogger.LogCommandText(CommandText, new { id = 42, status = "active", password = "hunter2" }, Excluded);
    }



    [Benchmark]
    public void Dictionary_DisabledLogger_FastPath()
    {
        DisabledLogger.LogCommandText(CommandText, DictParameters, Excluded);
    }



    [Benchmark]
    public void AnonymousObject_DisabledLogger_FastPath()
    {
        // The anonymous-object overload reflects into a dictionary BEFORE the
        // IsEnabled check (the reflection happens in the public overload, the
        // short-circuit in the core overload), so this measures the unavoidable
        // reflect-even-when-disabled cost — useful for deciding whether to gate
        // the anonymous overload behind an explicit IsEnabled at the call site.
        DisabledLogger.LogCommandText(CommandText, new { id = 42, status = "active", password = "hunter2" }, Excluded);
    }



    private static SqliteCommand CreateLiveCommand()
    {
        var command = new SqliteCommand(CommandText);
        command.Parameters.AddWithValue("@id", 42);
        command.Parameters.AddWithValue("@status", "active");
        command.Parameters.AddWithValue("@password", "hunter2");
        return command;
    }



    /// <summary>
    /// Logger that reports enabled and runs the formatter (so the structured
    /// render cost is included) but discards the payload.
    /// </summary>
    private sealed class EnabledNoOpLogger : ILogger
    {
        public IDisposable BeginScope<TState>(TState state) where TState : notnull => NullScope.Instance;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            _ = formatter(state, exception);
        }

        private sealed class NullScope : IDisposable
        {
            public static readonly NullScope Instance = new();

            public void Dispose()
            {
            }
        }
    }
}
