using System;
using System.Data;
using System.Data.Common;
using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Wolfgang.Extensions.Logging.Data.Benchmarks;

/// <summary>
/// Microbenchmarks for the public extension methods. Two scenarios are
/// measured:
///
/// 1. The IsEnabled fast-path — the logger returns false on IsEnabled, so
///    LogDbConnection should short-circuit before doing the connection-
///    string redaction work. This is the dominant production cost (almost
///    every call site is gated by a log level that's not enabled).
///
/// 2. The full work path — the logger returns true on IsEnabled, so the
///    redaction + Log call run. This is what hits when the log entry
///    actually gets written.
///
/// MemoryDiagnoser is enabled so any future refactor that introduces
/// allocation surfaces in the gh-pages benchmark chart immediately.
/// </summary>
[MemoryDiagnoser]
public class DbConnectionLoggerExtensionsBenchmarks
{
    private static readonly ILogger DisabledLogger = NullLogger.Instance;
    private static readonly ILogger EnabledLogger = new EnabledMemoryLogger();
    private static readonly DbConnection SampleConnection = new SampleDbConnection(
        connectionString: "Server=tcp:sample.example.com,1433;Database=widgets;User ID=app;Password=hunter2;TrustServerCertificate=true;");



    [Benchmark(Baseline = true)]
    public void LogDbConnection_DisabledLogger_FastPath()
    {
        DisabledLogger.LogDbConnection(SampleConnection);
    }



    [Benchmark]
    public void LogDbConnection_EnabledLogger_FullWork()
    {
        EnabledLogger.LogDbConnection(SampleConnection);
    }



    [Benchmark]
    public void LogDbConnection_EnabledLogger_ExplicitDebugLevel()
    {
        EnabledLogger.LogDbConnection(SampleConnection, LogLevel.Debug);
    }



    /// <summary>
    /// Logger that returns true for every IsEnabled call but discards the log
    /// payload. Lets the benchmark measure the redaction + Log invocation
    /// without coupling to any concrete sink implementation.
    /// </summary>
    private sealed class EnabledMemoryLogger : ILogger
    {
        public IDisposable BeginScope<TState>(TState state) where TState : notnull => NullScope.Instance;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            // Force the formatter to run so the benchmark also captures the
            // structured-message render cost rather than only the dispatch.
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



    /// <summary>
    /// Minimal DbConnection used only as a benchmark input. Implements the
    /// handful of properties LogDbConnection reads (ConnectionString,
    /// Database, DataSource, State, ConnectionTimeout) with canned values
    /// representative of a typical SQL connection. ServerVersion throws
    /// InvalidOperationException so the TryGetServerVersion catch branch is
    /// exercised at least sometimes.
    /// </summary>
    private sealed class SampleDbConnection : DbConnection
    {
        public SampleDbConnection(string connectionString)
        {
            ConnectionString = connectionString;
        }

        [System.Diagnostics.CodeAnalysis.AllowNull]
        public override string ConnectionString { get; set; }

        public override string Database => "widgets";

        public override string DataSource => "tcp:sample.example.com,1433";

        public override string ServerVersion => throw new InvalidOperationException("Connection not open.");

        public override ConnectionState State => ConnectionState.Closed;

        public override void ChangeDatabase(string databaseName)
        {
        }

        public override void Close()
        {
        }

        public override void Open()
        {
        }

        protected override DbCommand CreateDbCommand() => throw new NotSupportedException();

        protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel) => throw new NotSupportedException();
    }
}
