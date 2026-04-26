using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using Microsoft.Extensions.Logging;
using Wolfgang.Extensions.Logging.Data.Tests.Unit.TestHelpers;

namespace Wolfgang.Extensions.Logging.Data.Tests.Unit;

public class DbParameterCollectionLoggerExtensionsTests
{
    private static FakeDbParameterCollection CreateCollection()
    {
        return new FakeDbParameterCollection();
    }



    private static FakeDbParameter Add(DbParameterCollection collection, string name, object? value, DbType dbType = DbType.String)
    {
        var p = new FakeDbParameter
        {
            ParameterName = name,
            Value = value,
            DbType = dbType
        };
        collection.Add(p);
        return p;
    }



    [Fact]
    public void LogDbParameterCollection_when_logger_is_null_throws_ArgumentNullException()
    {
        ILogger logger = null!;
        var parameters = CreateCollection();

        Assert.Throws<ArgumentNullException>(() => logger.LogDbParameterCollection(parameters));
    }



    [Fact]
    public void LogDbParameterCollection_with_level_when_logger_is_null_throws_ArgumentNullException()
    {
        ILogger logger = null!;
        var parameters = CreateCollection();

        Assert.Throws<ArgumentNullException>(() => logger.LogDbParameterCollection(parameters, LogLevel.Warning));
    }



    [Fact]
    public void LogDbParameterCollection_with_excluded_when_logger_is_null_throws_ArgumentNullException()
    {
        ILogger logger = null!;
        var parameters = CreateCollection();

        Assert.Throws<ArgumentNullException>(() => logger.LogDbParameterCollection(parameters, new[] { "@p" }));
    }



    [Fact]
    public void LogDbParameterCollection_with_excluded_and_level_when_logger_is_null_throws_ArgumentNullException()
    {
        ILogger logger = null!;
        var parameters = CreateCollection();

        Assert.Throws<ArgumentNullException>(() => logger.LogDbParameterCollection(parameters, new[] { "@p" }, LogLevel.Warning));
    }



    [Fact]
    public void LogDbParameterCollection_when_parameters_is_null_throws_ArgumentNullException()
    {
        var logger = new RecordingLogger();

        Assert.Throws<ArgumentNullException>(() => logger.LogDbParameterCollection(parameters: null!));
    }



    [Fact]
    public void LogDbParameterCollection_with_level_when_parameters_is_null_throws_ArgumentNullException()
    {
        var logger = new RecordingLogger();

        Assert.Throws<ArgumentNullException>(() => logger.LogDbParameterCollection(null!, LogLevel.Warning));
    }



    [Fact]
    public void LogDbParameterCollection_when_log_level_is_disabled_writes_nothing()
    {
        var logger = new RecordingLogger { MinimumLevel = LogLevel.Warning };
        var parameters = CreateCollection();

        logger.LogDbParameterCollection(parameters, LogLevel.Information);

        Assert.Empty(logger.Entries);
    }



    [Fact]
    public void LogDbParameterCollection_default_overload_logs_at_information()
    {
        var logger = new RecordingLogger();
        var parameters = CreateCollection();

        logger.LogDbParameterCollection(parameters);

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
    public void LogDbParameterCollection_logs_at_specified_level(LogLevel level)
    {
        var logger = new RecordingLogger();
        var parameters = CreateCollection();

        logger.LogDbParameterCollection(parameters, level);

        var entry = Assert.Single(logger.Entries);
        Assert.Equal(level, entry.Level);
    }



    [Fact]
    public void LogDbParameterCollection_with_empty_collection_logs_count_zero_and_empty_list()
    {
        var logger = new RecordingLogger();
        var parameters = CreateCollection();

        logger.LogDbParameterCollection(parameters);

        var entry = Assert.Single(logger.Entries);
        Assert.Equal(0, entry.GetValue("Count"));
        var snaps = (IReadOnlyList<LoggedDbParameter>)entry.GetValue("Parameters")!;
        Assert.Empty(snaps);
    }



    [Fact]
    public void LogDbParameterCollection_logs_count_and_all_parameters()
    {
        var logger = new RecordingLogger();
        var parameters = CreateCollection();
        Add(parameters, "@id", 1, DbType.Int32);
        Add(parameters, "@name", "alice", DbType.String);

        logger.LogDbParameterCollection(parameters);

        var entry = Assert.Single(logger.Entries);
        Assert.Equal(2, entry.GetValue("Count"));
        var snaps = (IReadOnlyList<LoggedDbParameter>)entry.GetValue("Parameters")!;
        Assert.Equal(2, snaps.Count);
        Assert.Equal("@id", snaps[0].Name);
        Assert.Equal(1, snaps[0].Value);
        Assert.Equal("@name", snaps[1].Name);
        Assert.Equal("alice", snaps[1].Value);
    }



    [Fact]
    public void LogDbParameterCollection_redacts_values_for_excluded_names()
    {
        var logger = new RecordingLogger();
        var parameters = CreateCollection();
        Add(parameters, "@user", "alice");
        Add(parameters, "@password", "secret");

        logger.LogDbParameterCollection(parameters, new[] { "@password" });

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
    [InlineData(":password")]
    [InlineData("?password")]
    public void LogDbParameterCollection_excluded_name_matches_case_insensitively_and_ignores_prefix(string excludedName)
    {
        var logger = new RecordingLogger();
        var parameters = CreateCollection();
        Add(parameters, "@password", "secret");

        logger.LogDbParameterCollection(parameters, new[] { excludedName });

        var entry = Assert.Single(logger.Entries);
        var snap = ((IReadOnlyList<LoggedDbParameter>)entry.GetValue("Parameters")!)[0];
        Assert.Equal("[REDACTED]", snap.Value);
    }



    [Fact]
    public void LogDbParameterCollection_with_null_excluded_does_not_redact()
    {
        var logger = new RecordingLogger();
        var parameters = CreateCollection();
        Add(parameters, "@password", "secret");

        logger.LogDbParameterCollection(parameters, excludedParameterNames: null);

        var entry = Assert.Single(logger.Entries);
        var snap = ((IReadOnlyList<LoggedDbParameter>)entry.GetValue("Parameters")!)[0];
        Assert.Equal("secret", snap.Value);
    }



    [Fact]
    public void LogDbParameterCollection_with_excluded_and_level_uses_both()
    {
        var logger = new RecordingLogger();
        var parameters = CreateCollection();
        Add(parameters, "@p", "v");

        logger.LogDbParameterCollection(parameters, new[] { "p" }, LogLevel.Warning);

        var entry = Assert.Single(logger.Entries);
        Assert.Equal(LogLevel.Warning, entry.Level);
        var snap = ((IReadOnlyList<LoggedDbParameter>)entry.GetValue("Parameters")!)[0];
        Assert.Equal("[REDACTED]", snap.Value);
    }
}
