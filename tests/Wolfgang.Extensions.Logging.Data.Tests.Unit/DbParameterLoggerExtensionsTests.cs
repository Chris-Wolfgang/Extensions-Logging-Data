using System;
using System.Data;
using Microsoft.Extensions.Logging;
using Wolfgang.Extensions.Logging.Data.Tests.Unit.TestHelpers;

namespace Wolfgang.Extensions.Logging.Data.Tests.Unit;

public class DbParameterLoggerExtensionsTests
{
    private static FakeDbParameter CreateParameter()
    {
        return new FakeDbParameter
        {
            ParameterName = "@id",
            DbType = DbType.Int32,
            Direction = ParameterDirection.Input,
            Size = 4,
            Precision = 10,
            Scale = 0,
            IsNullable = false,
            Value = 42
        };
    }



    [Fact]
    public void LogDbParameter_when_logger_is_null_throws_ArgumentNullException()
    {
        ILogger logger = null!;
        var parameter = CreateParameter();

        Assert.Throws<ArgumentNullException>(() => logger.LogDbParameter(parameter));
    }



    [Fact]
    public void LogDbParameter_with_level_when_logger_is_null_throws_ArgumentNullException()
    {
        ILogger logger = null!;
        var parameter = CreateParameter();

        Assert.Throws<ArgumentNullException>(() => logger.LogDbParameter(parameter, LogLevel.Warning));
    }



    [Fact]
    public void LogDbParameter_with_redact_when_logger_is_null_throws_ArgumentNullException()
    {
        ILogger logger = null!;
        var parameter = CreateParameter();

        Assert.Throws<ArgumentNullException>(() => logger.LogDbParameter(parameter, redactValue: true));
    }



    [Fact]
    public void LogDbParameter_with_redact_and_level_when_logger_is_null_throws_ArgumentNullException()
    {
        ILogger logger = null!;
        var parameter = CreateParameter();

        Assert.Throws<ArgumentNullException>(() => logger.LogDbParameter(parameter, redactValue: true, LogLevel.Warning));
    }



    [Fact]
    public void LogDbParameter_when_parameter_is_null_throws_ArgumentNullException()
    {
        var logger = new RecordingLogger();

        Assert.Throws<ArgumentNullException>(() => logger.LogDbParameter(parameter: null!));
    }



    [Fact]
    public void LogDbParameter_with_level_when_parameter_is_null_throws_ArgumentNullException()
    {
        var logger = new RecordingLogger();

        Assert.Throws<ArgumentNullException>(() => logger.LogDbParameter(null!, LogLevel.Warning));
    }



    [Fact]
    public void LogDbParameter_when_log_level_is_disabled_writes_nothing()
    {
        var logger = new RecordingLogger { MinimumLevel = LogLevel.Warning };
        var parameter = CreateParameter();

        logger.LogDbParameter(parameter, LogLevel.Information);

        Assert.Empty(logger.Entries);
    }



    [Fact]
    public void LogDbParameter_default_overload_logs_at_information()
    {
        var logger = new RecordingLogger();
        var parameter = CreateParameter();

        logger.LogDbParameter(parameter);

        var entry = Assert.Single(logger.Entries);
        Assert.Equal(LogLevel.Information, entry.Level);
    }



    [Theory]
    [InlineData(LogLevel.Trace)]
    [InlineData(LogLevel.Debug)]
    [InlineData(LogLevel.Information)]
    [InlineData(LogLevel.Warning)]
    [InlineData(LogLevel.Error)]
    [InlineData(LogLevel.Critical)]
    public void LogDbParameter_logs_at_specified_level(LogLevel level)
    {
        var logger = new RecordingLogger();
        var parameter = CreateParameter();

        logger.LogDbParameter(parameter, level);

        var entry = Assert.Single(logger.Entries);
        Assert.Equal(level, entry.Level);
    }



    [Fact]
    public void LogDbParameter_includes_all_metadata_fields()
    {
        var logger = new RecordingLogger();
        var parameter = new FakeDbParameter
        {
            ParameterName = "@total",
            DbType = DbType.Decimal,
            Direction = ParameterDirection.InputOutput,
            Size = 9,
            Precision = 18,
            Scale = 4,
            IsNullable = true,
            Value = 12.34m
        };

        logger.LogDbParameter(parameter);

        var entry = Assert.Single(logger.Entries);
        Assert.Equal("@total", entry.GetValue("ParameterName"));
        Assert.Equal(DbType.Decimal, entry.GetValue("DbType"));
        Assert.Equal(ParameterDirection.InputOutput, entry.GetValue("Direction"));
        Assert.Equal(9, entry.GetValue("Size"));
        Assert.Equal((byte)18, entry.GetValue("Precision"));
        Assert.Equal((byte)4, entry.GetValue("Scale"));
        Assert.Equal(true, entry.GetValue("IsNullable"));
        Assert.Equal(12.34m, entry.GetValue("Value"));
    }



    [Fact]
    public void LogDbParameter_default_overload_does_not_redact_value()
    {
        var logger = new RecordingLogger();
        var parameter = CreateParameter();

        logger.LogDbParameter(parameter);

        var entry = Assert.Single(logger.Entries);
        Assert.Equal(42, entry.GetValue("Value"));
    }



    [Fact]
    public void LogDbParameter_with_level_only_does_not_redact_value()
    {
        var logger = new RecordingLogger();
        var parameter = CreateParameter();

        logger.LogDbParameter(parameter, LogLevel.Warning);

        var entry = Assert.Single(logger.Entries);
        Assert.Equal(42, entry.GetValue("Value"));
    }



    [Fact]
    public void LogDbParameter_with_redact_true_replaces_value_with_redacted_marker()
    {
        var logger = new RecordingLogger();
        var parameter = CreateParameter();

        logger.LogDbParameter(parameter, redactValue: true);

        var entry = Assert.Single(logger.Entries);
        Assert.Equal("[REDACTED]", entry.GetValue("Value"));
    }



    [Fact]
    public void LogDbParameter_with_redact_false_keeps_value()
    {
        var logger = new RecordingLogger();
        var parameter = CreateParameter();

        logger.LogDbParameter(parameter, redactValue: false);

        var entry = Assert.Single(logger.Entries);
        Assert.Equal(42, entry.GetValue("Value"));
    }



    [Fact]
    public void LogDbParameter_with_redact_true_keeps_metadata()
    {
        var logger = new RecordingLogger();
        var parameter = CreateParameter();

        logger.LogDbParameter(parameter, redactValue: true);

        var entry = Assert.Single(logger.Entries);
        Assert.Equal("@id", entry.GetValue("ParameterName"));
        Assert.Equal(DbType.Int32, entry.GetValue("DbType"));
        Assert.Equal(ParameterDirection.Input, entry.GetValue("Direction"));
        Assert.Equal(4, entry.GetValue("Size"));
        Assert.Equal((byte)10, entry.GetValue("Precision"));
        Assert.Equal((byte)0, entry.GetValue("Scale"));
        Assert.Equal(false, entry.GetValue("IsNullable"));
    }



    [Fact]
    public void LogDbParameter_with_redact_and_level_uses_both()
    {
        var logger = new RecordingLogger();
        var parameter = CreateParameter();

        logger.LogDbParameter(parameter, redactValue: true, LogLevel.Warning);

        var entry = Assert.Single(logger.Entries);
        Assert.Equal(LogLevel.Warning, entry.Level);
        Assert.Equal("[REDACTED]", entry.GetValue("Value"));
    }



    [Fact]
    public void LogDbParameter_with_null_value_logs_null()
    {
        var logger = new RecordingLogger();
        var parameter = new FakeDbParameter
        {
            ParameterName = "@nullable",
            Value = null
        };

        logger.LogDbParameter(parameter);

        var entry = Assert.Single(logger.Entries);
        Assert.Null(entry.GetValue("Value"));
    }
}
