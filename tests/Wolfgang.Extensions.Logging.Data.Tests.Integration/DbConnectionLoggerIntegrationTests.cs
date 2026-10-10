using System.Data;
using System.Data.Common;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;

namespace Wolfgang.Extensions.Logging.Data.Tests.Integration;

/// <summary>
/// End-to-end integration tests that exercise <c>LogDbConnection</c> with:
///   - a real <see cref="DbConnection"/> implementation (Microsoft.Data.Sqlite),
///   - a real <see cref="LoggerFactory"/> with a structured-capturing provider.
///
/// Where the unit tests use hand-rolled fakes for both, these tests prove the
/// extension method works against the production .NET logging pipeline and a
/// production ADO.NET provider — so a regression in either contract surfaces
/// here without needing the unit-test fakes to track it.
/// </summary>
public class DbConnectionLoggerIntegrationTests
{
    [Fact]
    public void LogDbConnection_against_open_sqlite_connection_emits_a_structured_entry()
    {
        using var provider = new CapturingLoggerProvider();
        using var factory = LoggerFactory.Create(builder =>
        {
            builder.SetMinimumLevel(LogLevel.Trace);
            builder.AddProvider(provider);
        });
        var logger = factory.CreateLogger<DbConnectionLoggerIntegrationTests>();

        using var connection = new SqliteConnection("Data Source=:memory:");
        connection.Open();

        logger.LogDbConnection(connection);

        var entry = Assert.Single(provider.Entries);
        Assert.Equal(LogLevel.Information, entry.Level);
        Assert.Contains(nameof(ConnectionState.Open), entry.Message, System.StringComparison.Ordinal);

        // Real connection — values come from Microsoft.Data.Sqlite, not a fake.
        // SqliteConnection.DataSource is the database file path, which is the
        // empty string for an in-memory database (":memory:" is the data source
        // *keyword*, not the reported DataSource value) — so assert the named
        // slot is present rather than a specific value.
        Assert.NotNull(entry.GetValue("DataSource"));   // empty string for :memory:, but the slot is populated
        Assert.Equal(ConnectionState.Open, entry.GetValue("State"));
        Assert.NotNull(entry.GetValue("ConnectionType"));   // typeof(SqliteConnection).FullName
        Assert.NotNull(entry.GetValue("ServerVersion"));     // SQLite reports its lib version once Open
    }



    [Fact]
    public void LogDbConnection_redacts_password_through_real_sqlite_connection_string()
    {
        using var provider = new CapturingLoggerProvider();
        using var factory = LoggerFactory.Create(builder =>
        {
            builder.SetMinimumLevel(LogLevel.Trace);
            builder.AddProvider(provider);
        });
        var logger = factory.CreateLogger<DbConnectionLoggerIntegrationTests>();

        // SqliteConnectionStringBuilder accepts a Password keyword (it maps to
        // SQLCipher-style encryption) at construction time. The native
        // 'e_sqlite3' library rejects it only on Open(), so the connection is
        // deliberately NOT opened here — the redaction path reads
        // SqliteConnection.ConnectionString, which works regardless of state.
        // This proves the redaction works against the real Microsoft.Data.Sqlite
        // connection-string normalization rather than the hand-rolled
        // FakeDbConnection the unit tests use.
        using var connection = new SqliteConnection("Data Source=:memory:;Password=topsecret");

        logger.LogDbConnection(connection);

        var entry = Assert.Single(provider.Entries);
        var logged = (string)entry.GetValue("ConnectionString")!;
        Assert.DoesNotContain("topsecret", logged, System.StringComparison.OrdinalIgnoreCase);
        Assert.Contains(":memory:", logged, System.StringComparison.OrdinalIgnoreCase);
    }



    [Fact]
    public void LogDbConnection_against_closed_sqlite_connection_emits_null_server_version()
    {
        using var provider = new CapturingLoggerProvider();
        using var factory = LoggerFactory.Create(builder => builder.AddProvider(provider));
        var logger = factory.CreateLogger<DbConnectionLoggerIntegrationTests>();

        using var connection = new SqliteConnection("Data Source=:memory:");
        // intentionally not opened

        logger.LogDbConnection(connection);

        var entry = Assert.Single(provider.Entries);
        Assert.Null(entry.GetValue("ServerVersion"));
        Assert.Equal(ConnectionState.Closed, entry.GetValue("State"));
    }



    [Fact]
    public void LogDbConnection_respects_pipeline_minimum_level()
    {
        using var provider = new CapturingLoggerProvider();
        using var factory = LoggerFactory.Create(builder =>
        {
            builder.SetMinimumLevel(LogLevel.Warning);
            builder.AddProvider(provider);
        });
        var logger = factory.CreateLogger<DbConnectionLoggerIntegrationTests>();

        using var connection = new SqliteConnection("Data Source=:memory:");
        connection.Open();

        // Default overload logs at Information; with min=Warning, nothing
        // should reach the provider.
        logger.LogDbConnection(connection);
        Assert.Empty(provider.Entries);

        // Explicit Warning should pass.
        logger.LogDbConnection(connection, LogLevel.Warning);
        Assert.Single(provider.Entries);
    }



    [Fact]
    public void LogDbConnection_inside_a_caller_scope_emits_one_entry_through_the_pipeline()
    {
        using var provider = new CapturingLoggerProvider();
        using var factory = LoggerFactory.Create(builder => builder.AddProvider(provider));
        var logger = factory.CreateLogger<DbConnectionLoggerIntegrationTests>();

        using var connection = new SqliteConnection("Data Source=:memory:");
        connection.Open();

        using (logger.BeginScope("import {BatchId}", 42))
        {
            logger.LogDbConnection(connection);
        }

        Assert.Equal("import 42", Assert.Single(provider.Scopes));
        Assert.Equal(ConnectionState.Open, Assert.Single(provider.Entries).GetValue("State"));
    }
}


/// <summary>
/// Minimal <see cref="ILoggerProvider"/> that captures every log entry as a
/// structured payload. Keeps the integration tests free of any concrete sink
/// (Console / Debug / Serilog / etc.) so they only assert what the library
/// produces, not what a sink renders.
/// </summary>
internal sealed class CapturingLoggerProvider : ILoggerProvider
{
    public List<CapturedEntry> Entries { get; } = new();

    public List<string?> Scopes { get; } = new();

    public ILogger CreateLogger(string categoryName) => new CapturingLogger(this);

    public void Dispose()
    {
    }

    private sealed class CapturingLogger : ILogger
    {
        private readonly CapturingLoggerProvider _provider;

        public CapturingLogger(CapturingLoggerProvider provider)
        {
            _provider = provider;
        }

        public IDisposable BeginScope<TState>(TState state) where TState : notnull
        {
            _provider.Scopes.Add(state.ToString());
            return NullScope.Instance;
        }

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, System.Exception? exception, System.Func<TState, System.Exception?, string> formatter)
        {
            // The library logs through LoggerMessage, whose state is always the
            // structured key/value list; anything else is a contract change.
            var values = (IReadOnlyList<KeyValuePair<string, object?>>)state!;
            _provider.Entries.Add(new CapturedEntry(logLevel, formatter(state, exception), values));
        }
    }

    private sealed class NullScope : System.IDisposable
    {
        public static readonly NullScope Instance = new();

        public void Dispose()
        {
        }
    }
}


internal sealed class CapturedEntry
{
    private readonly IReadOnlyList<KeyValuePair<string, object?>> _values;

    public CapturedEntry(LogLevel level, string message, IReadOnlyList<KeyValuePair<string, object?>> values)
    {
        Level = level;
        Message = message;
        _values = values;
    }

    public LogLevel Level { get; }

    public string Message { get; }

    /// <summary>
    /// Returns the value of the named structured slot. Throws when the slot is
    /// absent, so a misspelt or dropped slot fails the test instead of reading
    /// as <see langword="null"/>.
    /// </summary>
    public object? GetValue(string name) =>
        _values
            .Single(kv => string.Equals(kv.Key, name, System.StringComparison.Ordinal))
            .Value;
}
