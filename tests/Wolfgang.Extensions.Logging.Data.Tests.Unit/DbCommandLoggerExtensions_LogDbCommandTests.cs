using System;
using System.Data;
using System.Data.Common;
using Microsoft.Extensions.Logging;
using Wolfgang.Extensions.Logging.Data.Tests.Unit.TestHelpers;

namespace Wolfgang.Extensions.Logging.Data.Tests.Unit;

public class DbCommandLoggerExtensions_LogDbCommandTests
{
    private static FakeDbCommand SampleCommand()
    {
        var command = new FakeDbCommand { CommandText = "SELECT * FROM Users WHERE Id = @id" };
        command.AddParameter("@id", 1);
        command.AddParameter("@password", "hunter2");
        return command;
    }



    [Fact]
    public void LogDbCommand_when_logger_is_null_throws_ArgumentNullException()
    {
        ILogger logger = null!;

        Assert.Throws<ArgumentNullException>(() => logger.LogDbCommand(SampleCommand()));
    }



    [Fact]
    public void LogDbCommand_when_command_is_null_throws_ArgumentNullException()
    {
        var logger = new RecordingLogger();

        Assert.Throws<ArgumentNullException>(() => logger.LogDbCommand((DbCommand)null!));
    }



    [Fact]
    public void LogDbCommand_when_excludedParameterNames_is_null_throws_ArgumentNullException()
    {
        var logger = new RecordingLogger();

        Assert.Throws<ArgumentNullException>(() => logger.LogDbCommand(SampleCommand(), (System.Collections.Generic.IEnumerable<string>)null!));
    }



    [Fact]
    public void LogDbCommand_when_log_level_is_disabled_writes_nothing()
    {
        var logger = new RecordingLogger { MinimumLevel = LogLevel.Warning };

        logger.LogDbCommand(SampleCommand(), LogLevel.Information);

        Assert.Empty(logger.Entries);
    }



    [Fact]
    public void LogDbCommand_default_overload_logs_at_information()
    {
        var logger = new RecordingLogger();

        logger.LogDbCommand(SampleCommand());

        Assert.Equal(LogLevel.Information, Assert.Single(logger.Entries).Level);
    }



    [Theory]
    [InlineData(LogLevel.Trace)]
    [InlineData(LogLevel.Debug)]
    [InlineData(LogLevel.Warning)]
    [InlineData(LogLevel.Error)]
    public void LogDbCommand_logs_at_specified_level(LogLevel level)
    {
        var logger = new RecordingLogger();

        logger.LogDbCommand(SampleCommand(), level);

        Assert.Equal(level, Assert.Single(logger.Entries).Level);
    }



    [Fact]
    public void LogDbCommand_logs_command_text_and_parameters_from_the_live_command()
    {
        var logger = new RecordingLogger();

        logger.LogDbCommand(SampleCommand());

        var entry = Assert.Single(logger.Entries);
        Assert.Equal("SELECT * FROM Users WHERE Id = @id", entry.GetValue("CommandText"));
        var rendered = (string)entry.GetValue("Parameters")!;
        Assert.Contains("@id=1", rendered, StringComparison.Ordinal);
    }



    [Fact]
    public void LogDbCommand_redacts_excluded_parameter_value()
    {
        var logger = new RecordingLogger();

        logger.LogDbCommand(SampleCommand(), new[] { "password" });

        var rendered = (string)Assert.Single(logger.Entries).GetValue("Parameters")!;
        Assert.DoesNotContain("hunter2", rendered, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("@password=***", rendered, StringComparison.Ordinal);
        Assert.Contains("@id=1", rendered, StringComparison.Ordinal);
    }



    [Fact]
    public void LogDbCommand_with_no_parameters_renders_empty_set()
    {
        var logger = new RecordingLogger();
        var command = new FakeDbCommand { CommandText = "SELECT 1" };

        logger.LogDbCommand(command);

        Assert.Equal("{}", Assert.Single(logger.Entries).GetValue("Parameters"));
    }



    [Fact]
    public void LogDbCommand_normalizes_DBNull_parameter_value_to_null()
    {
        var logger = new RecordingLogger();
        var command = new FakeDbCommand { CommandText = "UPDATE ..." };
        command.AddParameter("@note", DBNull.Value);

        logger.LogDbCommand(command);

        var rendered = (string)Assert.Single(logger.Entries).GetValue("Parameters")!;
        Assert.Contains("@note=null", rendered, StringComparison.Ordinal);
    }



    [Fact]
    public void LogDbCommand_with_null_command_text_logs_empty_string()
    {
        var logger = new RecordingLogger();
        var command = new FakeDbCommand { CommandText = null! };

        logger.LogDbCommand(command);

        Assert.Equal(string.Empty, Assert.Single(logger.Entries).GetValue("CommandText"));
    }
}
