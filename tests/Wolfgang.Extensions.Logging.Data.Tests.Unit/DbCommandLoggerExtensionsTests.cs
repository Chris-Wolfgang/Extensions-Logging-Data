using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.Extensions.Logging;
using Wolfgang.Extensions.Logging.Data.Tests.Unit.TestHelpers;

namespace Wolfgang.Extensions.Logging.Data.Tests.Unit;

public class DbCommandLoggerExtensionsTests
{
    private static FakeDbCommand CreateCommand(string commandText = "SELECT 1", CommandType type = CommandType.Text, int timeout = 30)
    {
        return new FakeDbCommand
        {
            CommandText = commandText,
            CommandType = type,
            CommandTimeout = timeout
        };
    }



    private static FakeDbParameter AddParameter(FakeDbCommand command, string name, object? value, DbType dbType = DbType.String)
    {
        var p = new FakeDbParameter
        {
            ParameterName = name,
            Value = value,
            DbType = dbType
        };
        command.Parameters.Add(p);
        return p;
    }



    [Fact]
    public void LogDbCommand_when_logger_is_null_throws_ArgumentNullException()
    {
        ILogger logger = null!;
        var command = CreateCommand();

        Assert.Throws<ArgumentNullException>(() => logger.LogDbCommand(command));
    }



    [Fact]
    public void LogDbCommand_with_level_when_logger_is_null_throws_ArgumentNullException()
    {
        ILogger logger = null!;
        var command = CreateCommand();

        Assert.Throws<ArgumentNullException>(() => logger.LogDbCommand(command, LogLevel.Warning));
    }



    [Fact]
    public void LogDbCommand_with_excluded_when_logger_is_null_throws_ArgumentNullException()
    {
        ILogger logger = null!;
        var command = CreateCommand();

        Assert.Throws<ArgumentNullException>(() => logger.LogDbCommand(command, new[] { "@p" }));
    }



    [Fact]
    public void LogDbCommand_with_excluded_and_level_when_logger_is_null_throws_ArgumentNullException()
    {
        ILogger logger = null!;
        var command = CreateCommand();

        Assert.Throws<ArgumentNullException>(() => logger.LogDbCommand(command, new[] { "@p" }, LogLevel.Warning));
    }



    [Fact]
    public void LogDbCommand_when_command_is_null_throws_ArgumentNullException()
    {
        var logger = new RecordingLogger();

        Assert.Throws<ArgumentNullException>(() => logger.LogDbCommand(command: null!));
    }



    [Fact]
    public void LogDbCommand_with_level_when_command_is_null_throws_ArgumentNullException()
    {
        var logger = new RecordingLogger();

        Assert.Throws<ArgumentNullException>(() => logger.LogDbCommand(null!, LogLevel.Warning));
    }



    [Fact]
    public void LogDbCommand_when_log_level_is_disabled_writes_nothing()
    {
        var logger = new RecordingLogger { MinimumLevel = LogLevel.Warning };
        var command = CreateCommand();

        logger.LogDbCommand(command, LogLevel.Information);

        Assert.Empty(logger.Entries);
    }



    [Fact]
    public void LogDbCommand_default_overload_logs_at_information()
    {
        var logger = new RecordingLogger();
        var command = CreateCommand();

        logger.LogDbCommand(command);

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
    public void LogDbCommand_logs_at_specified_level(LogLevel level)
    {
        var logger = new RecordingLogger();
        var command = CreateCommand();

        logger.LogDbCommand(command, level);

        var entry = Assert.Single(logger.Entries);
        Assert.Equal(level, entry.Level);
    }



    [Fact]
    public void LogDbCommand_includes_command_text_type_and_timeout()
    {
        var logger = new RecordingLogger();
        var command = CreateCommand("EXEC GetUser @id", CommandType.StoredProcedure, 60);

        logger.LogDbCommand(command);

        var entry = Assert.Single(logger.Entries);
        Assert.Equal("EXEC GetUser @id", entry.GetValue("CommandText"));
        Assert.Equal(CommandType.StoredProcedure, entry.GetValue("CommandType"));
        Assert.Equal(60, entry.GetValue("CommandTimeout"));
    }



    [Fact]
    public void LogDbCommand_with_empty_parameter_collection_logs_empty_list()
    {
        var logger = new RecordingLogger();
        var command = CreateCommand();

        logger.LogDbCommand(command);

        var entry = Assert.Single(logger.Entries);
        var snapshots = (IReadOnlyList<LoggedDbParameter>)entry.GetValue("Parameters")!;
        Assert.Empty(snapshots);
    }



    [Fact]
    public void LogDbCommand_includes_all_parameters_in_log_entry()
    {
        var logger = new RecordingLogger();
        var command = CreateCommand();
        AddParameter(command, "@id", 42, DbType.Int32);
        AddParameter(command, "@name", "alice", DbType.String);
        AddParameter(command, "@active", value: true, DbType.Boolean);

        logger.LogDbCommand(command);

        var entry = Assert.Single(logger.Entries);
        var snapshots = (IReadOnlyList<LoggedDbParameter>)entry.GetValue("Parameters")!;
        Assert.Equal(3, snapshots.Count);
        Assert.Equal("@id", snapshots[0].Name);
        Assert.Equal(42, snapshots[0].Value);
        Assert.Equal("@name", snapshots[1].Name);
        Assert.Equal("alice", snapshots[1].Value);
        Assert.Equal("@active", snapshots[2].Name);
        Assert.Equal(true, snapshots[2].Value);
    }



    [Fact]
    public void LogDbCommand_snapshot_captures_full_metadata_for_each_parameter()
    {
        var logger = new RecordingLogger();
        var command = CreateCommand();
        var p = new FakeDbParameter
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
        command.Parameters.Add(p);

        logger.LogDbCommand(command);

        var entry = Assert.Single(logger.Entries);
        var snap = ((IReadOnlyList<LoggedDbParameter>)entry.GetValue("Parameters")!).Single();
        Assert.Equal("@total", snap.Name);
        Assert.Equal(DbType.Decimal, snap.DbType);
        Assert.Equal(ParameterDirection.InputOutput, snap.Direction);
        Assert.Equal(9, snap.Size);
        Assert.Equal(18, snap.Precision);
        Assert.Equal(4, snap.Scale);
        Assert.True(snap.IsNullable);
        Assert.Equal(12.34m, snap.Value);
    }



    [Fact]
    public void LogDbCommand_redacts_excluded_parameter_value_only()
    {
        var logger = new RecordingLogger();
        var command = CreateCommand();
        AddParameter(command, "@user", "alice");
        AddParameter(command, "@password", "secret");

        logger.LogDbCommand(command, new[] { "@password" });

        var entry = Assert.Single(logger.Entries);
        var snaps = (IReadOnlyList<LoggedDbParameter>)entry.GetValue("Parameters")!;
        Assert.Equal("alice", snaps[0].Value);
        Assert.Equal("[REDACTED]", snaps[1].Value);
        Assert.Equal("@password", snaps[1].Name);
        Assert.Equal(DbType.String, snaps[1].DbType);
    }



    [Theory]
    [InlineData("@password")]
    [InlineData("password")]
    [InlineData("PASSWORD")]
    [InlineData("PassWord")]
    [InlineData(":password")]
    [InlineData("?password")]
    public void LogDbCommand_excluded_name_matches_case_insensitively_and_ignores_prefix(string excludedName)
    {
        var logger = new RecordingLogger();
        var command = CreateCommand();
        AddParameter(command, "@password", "secret");

        logger.LogDbCommand(command, new[] { excludedName });

        var entry = Assert.Single(logger.Entries);
        var snap = ((IReadOnlyList<LoggedDbParameter>)entry.GetValue("Parameters")!).Single();
        Assert.Equal("[REDACTED]", snap.Value);
    }



    [Theory]
    [InlineData("@password")]
    [InlineData(":password")]
    [InlineData("?password")]
    [InlineData("password")]
    public void LogDbCommand_redacts_parameter_regardless_of_actual_prefix(string actualName)
    {
        var logger = new RecordingLogger();
        var command = CreateCommand();
        AddParameter(command, actualName, "secret");

        logger.LogDbCommand(command, new[] { "password" });

        var entry = Assert.Single(logger.Entries);
        var snap = ((IReadOnlyList<LoggedDbParameter>)entry.GetValue("Parameters")!).Single();
        Assert.Equal("[REDACTED]", snap.Value);
    }



    [Fact]
    public void LogDbCommand_with_null_excluded_does_not_redact()
    {
        var logger = new RecordingLogger();
        var command = CreateCommand();
        AddParameter(command, "@password", "secret");

        logger.LogDbCommand(command, excludedParameterNames: null);

        var entry = Assert.Single(logger.Entries);
        var snap = ((IReadOnlyList<LoggedDbParameter>)entry.GetValue("Parameters")!).Single();
        Assert.Equal("secret", snap.Value);
    }



    [Fact]
    public void LogDbCommand_with_excluded_and_level_uses_both()
    {
        var logger = new RecordingLogger();
        var command = CreateCommand();
        AddParameter(command, "@p", "v");

        logger.LogDbCommand(command, new[] { "p" }, LogLevel.Warning);

        var entry = Assert.Single(logger.Entries);
        Assert.Equal(LogLevel.Warning, entry.Level);
        var snap = ((IReadOnlyList<LoggedDbParameter>)entry.GetValue("Parameters")!).Single();
        Assert.Equal("[REDACTED]", snap.Value);
    }



    // ---- Internal helper tests ----

    [Fact]
    public void StripPrefix_strips_at_sign() => Assert.Equal("name", DbCommandLoggerExtensions.StripPrefix("@name"));



    [Fact]
    public void StripPrefix_strips_colon() => Assert.Equal("name", DbCommandLoggerExtensions.StripPrefix(":name"));



    [Fact]
    public void StripPrefix_strips_question_mark() => Assert.Equal("name", DbCommandLoggerExtensions.StripPrefix("?name"));



    [Fact]
    public void StripPrefix_leaves_other_first_characters_alone() => Assert.Equal("name", DbCommandLoggerExtensions.StripPrefix("name"));



    [Fact]
    public void StripPrefix_with_empty_returns_empty() => Assert.Equal(string.Empty, DbCommandLoggerExtensions.StripPrefix(string.Empty));



    [Fact]
    public void StripPrefix_with_only_prefix_returns_empty() => Assert.Equal(string.Empty, DbCommandLoggerExtensions.StripPrefix("@"));



    [Fact]
    public void NormalizeExcluded_with_null_returns_empty_set()
    {
        var set = DbCommandLoggerExtensions.NormalizeExcluded(excludedParameterNames: null);
        Assert.Empty(set);
    }



    [Fact]
    public void NormalizeExcluded_strips_prefix_and_lowercases_for_match()
    {
        var set = DbCommandLoggerExtensions.NormalizeExcluded(new[] { "@Password", ":SECRET" });
        Assert.Contains("password", set);
        Assert.Contains("secret", set);
        Assert.True(set.Contains("PASSWORD"));
        Assert.True(set.Contains("Secret"));
    }



    [Fact]
    public void NormalizeExcluded_skips_null_and_empty_entries()
    {
        var set = DbCommandLoggerExtensions.NormalizeExcluded(new[] { null!, string.Empty, "real" });
        Assert.Single(set);
        Assert.Contains("real", set);
    }



    [Fact]
    public void ShouldRedact_with_empty_excluded_returns_false()
    {
        var set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        Assert.False(DbCommandLoggerExtensions.ShouldRedact("@anything", set));
    }



    [Fact]
    public void ShouldRedact_with_null_parameter_name_returns_false_when_no_empty_excluded()
    {
        var set = DbCommandLoggerExtensions.NormalizeExcluded(new[] { "real" });
        Assert.False(DbCommandLoggerExtensions.ShouldRedact(parameterName: null, set));
    }
}
