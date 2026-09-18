using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Wolfgang.Extensions.Logging.Data.EntityFramework6.Tests.Unit.TestHelpers;

namespace Wolfgang.Extensions.Logging.Data.EntityFramework6.Tests.Unit;

/// <summary>
/// Pins the behaviour of the test double every logging assertion in this project relies on.
/// </summary>
public class RecordingLoggerTests
{
    [Fact]
    public void IsEnabled_when_level_is_below_MinimumLevel_returns_false()
    {
        var logger = new RecordingLogger { MinimumLevel = LogLevel.Warning };

        Assert.False(logger.IsEnabled(LogLevel.Information));
        Assert.True(logger.IsEnabled(LogLevel.Warning));
    }



    [Fact]
    public void BeginScope_returns_null()
    {
        var logger = new RecordingLogger();

        Assert.Null(logger.BeginScope("scope"));
    }



    [Fact]
    public void Log_records_level_message_exception_and_structured_values()
    {
        var logger = new RecordingLogger();
        var exception = new InvalidOperationException("boom");
        var state = new List<KeyValuePair<string, object?>>
        {
            new KeyValuePair<string, object?>("Name", "value"),
        };

        logger.Log(LogLevel.Error, default, state, exception, (_, _) => "formatted");

        var entry = Assert.Single(logger.Entries);
        Assert.Equal(LogLevel.Error, entry.Level);
        Assert.Equal("formatted", entry.Message);
        Assert.Same(exception, entry.Exception);
        Assert.Equal("value", entry.GetValue("Name"));
        Assert.Null(entry.GetValue("Missing"));
    }



    [Fact]
    public void Log_when_state_is_not_a_structured_value_list_records_null_values()
    {
        var logger = new RecordingLogger();

        logger.Log(LogLevel.Information, default, "plain state", null, (s, _) => s);

        var entry = Assert.Single(logger.Entries);
        Assert.Null(entry.Values);
        Assert.Null(entry.GetValue("anything"));
    }
}
