using System.Data;
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
        var provider = new CapturingLoggerProvider();
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
        var provider = new CapturingLoggerProvider();
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
        var provider = new CapturingLoggerProvider();
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
        var provider = new CapturingLoggerProvider();
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

        public IDisposable BeginScope<TState>(TState state) where TState : notnull => NullScope.Instance;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, System.Exception? exception, System.Func<TState, System.Exception?, string> formatter)
        {
            var values = state as IReadOnlyList<KeyValuePair<string, object?>>;
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
    private readonly IReadOnlyList<KeyValuePair<string, object?>>? _values;

    public CapturedEntry(LogLevel level, string message, IReadOnlyList<KeyValuePair<string, object?>>? values)
    {
        Level = level;
        Message = message;
        _values = values;
    }

    public LogLevel Level { get; }

    public string Message { get; }

    public object? GetValue(string name)
    {
        if (_values is null)
        {
            return null;
        }

        foreach (var kv in _values)
        {
            if (string.Equals(kv.Key, name, System.StringComparison.Ordinal))
            {
                return kv.Value;
            }
        }

        return null;
    }
}
