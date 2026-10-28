using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Wolfgang.Extensions.Logging.Data.Tests.Fuzz;

/// <summary>
/// Pins the behaviour of the fuzz project's test doubles, including the members the
/// properties never reach, so the doubles cannot drift silently.
/// </summary>
public class TestDoublesTests
{
    [Fact]
    public void FakeDbConnection_when_used_is_closed_and_rejects_every_operation()
    {
        using var connection = new FakeDbConnection();

        connection.ConnectionString = null!;

        Assert.Equal(string.Empty, connection.ConnectionString);
        Assert.Equal("fuzz", connection.Database);
        Assert.Equal("fuzz-host", connection.DataSource);
        Assert.Equal("1.0", connection.ServerVersion);
        Assert.Equal(ConnectionState.Closed, connection.State);
        Assert.Throws<NotSupportedException>(() => connection.ChangeDatabase("other"));
        Assert.Throws<NotSupportedException>(() => connection.Close());
        Assert.Throws<NotSupportedException>(() => connection.Open());
        Assert.Throws<NotSupportedException>(() => connection.CreateCommand());
        Assert.Throws<NotSupportedException>(() => connection.BeginTransaction());
    }



    [Fact]
    public void CapturingLogger_when_logging_records_structured_values_and_has_no_scope()
    {
        var logger = new CapturingLogger();
        var values = new List<KeyValuePair<string, object?>>
        {
            new("Name", "value"),
            new("Null", null),
        };

        logger.Log(LogLevel.Information, default, values, null, (_, _) => string.Empty);
        logger.Log(LogLevel.Information, default, "not structured", null, (_, _) => string.Empty);

        Assert.Null(logger.BeginScope("scope"));
        Assert.True(logger.IsEnabled(LogLevel.Trace));
        Assert.Equal("value", logger.Value(0, "Name"));
        Assert.Equal(string.Empty, logger.Value(0, "Null"));
        Assert.Empty(logger.Entries[1]);
        Assert.Throws<KeyNotFoundException>(() => logger.Value(0, "Missing"));
    }
}
