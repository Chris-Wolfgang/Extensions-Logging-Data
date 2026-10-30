using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Wolfgang.Extensions.Logging.Data.Tests.Unit.TestHelpers;
using Xunit;

namespace Wolfgang.Extensions.Logging.Data.Tests.Unit;

public class DbCommandLoggerExtensionsTests
{
    private static Dictionary<string, object?> SampleParameters() => new()
    {
        ["id"] = 1,
        ["name"] = "abc"
    };



    [Fact]
    public void LogCommandText_dict_when_logger_is_null_throws_ArgumentNullException()
    {
        ILogger logger = null!;

        Assert.Throws<ArgumentNullException>(() => logger.LogCommandText("SELECT 1", SampleParameters()));
    }



    [Fact]
    public void LogCommandText_dict_when_commandText_is_null_throws_ArgumentNullException()
    {
        var logger = new RecordingLogger();

        Assert.Throws<ArgumentNullException>(() => logger.LogCommandText(null!, SampleParameters()));
    }



    [Fact]
    public void LogCommandText_dict_when_parameters_is_null_throws_ArgumentNullException()
    {
        var logger = new RecordingLogger();

        Assert.Throws<ArgumentNullException>(() => logger.LogCommandText("SELECT 1", (IReadOnlyDictionary<string, object?>)null!));
    }



    [Fact]
    public void LogCommandText_dict_when_excludedParameterNames_is_null_throws_ArgumentNullException()
    {
        var logger = new RecordingLogger();

        Assert.Throws<ArgumentNullException>(() => logger.LogCommandText("SELECT 1", SampleParameters(), (IEnumerable<string>)null!));
    }



    [Fact]
    public void LogCommandText_object_when_parameters_is_null_throws_ArgumentNullException()
    {
        var logger = new RecordingLogger();

        Assert.Throws<ArgumentNullException>(() => logger.LogCommandText("SELECT 1", (object)null!));
    }



    [Fact]
    public void LogCommandText_when_log_level_is_disabled_writes_nothing()
    {
        var logger = new RecordingLogger { MinimumLevel = LogLevel.Warning };

        logger.LogCommandText("SELECT 1", SampleParameters(), LogLevel.Information);

        Assert.Empty(logger.Entries);
    }



    [Fact]
    public void LogCommandText_when_using_the_default_overload_logs_at_information()
    {
        var logger = new RecordingLogger();

        logger.LogCommandText("SELECT 1", SampleParameters());

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
    public void LogCommandText_when_a_level_is_specified_logs_at_that_level(LogLevel level)
    {
        var logger = new RecordingLogger();

        logger.LogCommandText("SELECT 1", SampleParameters(), level);

        var entry = Assert.Single(logger.Entries);
        Assert.Equal(level, entry.Level);
    }



    [Fact]
    public void LogCommandText_when_given_a_dictionary_logs_command_text_and_parameters_in_log_values()
    {
        var logger = new RecordingLogger();

        logger.LogCommandText("SELECT * FROM Users WHERE Id = @id", SampleParameters());

        var entry = Assert.Single(logger.Entries);
        Assert.Equal("SELECT * FROM Users WHERE Id = @id", entry.GetValue("CommandText"));
        var rendered = (string)entry.GetValue("Parameters")!;
        Assert.Contains("id=1", rendered, StringComparison.Ordinal);
        Assert.Contains("name=abc", rendered, StringComparison.Ordinal);
    }



    [Fact]
    public void LogCommandText_when_a_dictionary_parameter_is_excluded_redacts_its_value()
    {
        var logger = new RecordingLogger();
        var parameters = new Dictionary<string, object?> { ["id"] = 1, ["password"] = "hunter2" };

        logger.LogCommandText("INSERT ...", parameters, new[] { "password" });

        var rendered = (string)Assert.Single(logger.Entries).GetValue("Parameters")!;
        Assert.DoesNotContain("hunter2", rendered, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("password=***", rendered, StringComparison.Ordinal);
        // Non-excluded values are still present.
        Assert.Contains("id=1", rendered, StringComparison.Ordinal);
    }



    [Theory]
    [InlineData("password")]   // bare
    [InlineData("@password")]  // SQL Server / MySQL
    [InlineData(":password")]  // Oracle
    [InlineData("?password")]  // ODBC positional-ish
    [InlineData("PASSWORD")]   // case-insensitive
    [InlineData("@PassWord")]  // prefix + casing
    public void LogCommandText_when_an_excluded_name_differs_by_prefix_or_case_still_redacts_it(string excludedName)
    {
        var logger = new RecordingLogger();
        var parameters = new Dictionary<string, object?> { ["@password"] = "hunter2" };

        logger.LogCommandText("INSERT ...", parameters, new[] { excludedName });

        var rendered = (string)Assert.Single(logger.Entries).GetValue("Parameters")!;
        Assert.DoesNotContain("hunter2", rendered, StringComparison.OrdinalIgnoreCase);
    }



    [Fact]
    public void LogCommandText_when_given_an_object_reflects_public_properties_into_parameters()
    {
        var logger = new RecordingLogger();

        logger.LogCommandText("SELECT * FROM Users WHERE Id = @id", new { id = 7, name = "zoe" });

        var rendered = (string)Assert.Single(logger.Entries).GetValue("Parameters")!;
        Assert.Contains("id=7", rendered, StringComparison.Ordinal);
        Assert.Contains("name=zoe", rendered, StringComparison.Ordinal);
    }



    [Fact]
    public void LogCommandText_when_object_has_no_properties_logs_empty_parameter_set()
    {
        var logger = new RecordingLogger();

        logger.LogCommandText("SELECT 1", new { });

        var rendered = (string)Assert.Single(logger.Entries).GetValue("Parameters")!;
        Assert.Equal("{}", rendered);
    }



    [Fact]
    public void LogCommandText_when_an_object_property_is_null_renders_null_literal()
    {
        var logger = new RecordingLogger();

        logger.LogCommandText("UPDATE ...", new { id = 1, note = (string?)null });

        var rendered = (string)Assert.Single(logger.Entries).GetValue("Parameters")!;
        Assert.Contains("note=null", rendered, StringComparison.Ordinal);
    }



    [Fact]
    public void LogCommandText_when_object_has_mixed_value_types_renders_each_value()
    {
        var logger = new RecordingLogger();
        var when = new DateTime(2026, 6, 14, 0, 0, 0, DateTimeKind.Utc);

        logger.LogCommandText(
            "INSERT ...",
            new { id = 42, ratio = 1.5, active = true, when, tag = "x" });

        var rendered = (string)Assert.Single(logger.Entries).GetValue("Parameters")!;
        Assert.Contains("id=42", rendered, StringComparison.Ordinal);
        Assert.Contains("ratio=1.5", rendered, StringComparison.Ordinal);
        Assert.Contains("active=True", rendered, StringComparison.Ordinal);
        Assert.Contains("tag=x", rendered, StringComparison.Ordinal);
    }



    [Fact]
    public void LogCommandText_when_an_object_property_is_excluded_redacts_its_value()
    {
        var logger = new RecordingLogger();

        logger.LogCommandText("INSERT ...", new { user = "bob", password = "hunter2" }, new[] { "password" });

        var rendered = (string)Assert.Single(logger.Entries).GetValue("Parameters")!;
        Assert.DoesNotContain("hunter2", rendered, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("password=***", rendered, StringComparison.Ordinal);
        Assert.Contains("user=bob", rendered, StringComparison.Ordinal);
    }



    [Fact]
    public void LogCommandText_when_object_overload_is_given_a_level_logs_at_that_level()
    {
        var logger = new RecordingLogger();

        logger.LogCommandText("SELECT 1", new { id = 1 }, LogLevel.Debug);

        Assert.Equal(LogLevel.Debug, Assert.Single(logger.Entries).Level);
    }



    [Fact]
    public void ToDictionary_when_called_repeatedly_returns_consistent_results_from_cached_accessors()
    {
        // Two anonymous instances of the same shape exercise the per-type cache
        // (first call builds the reader, second reuses it). Both must render the
        // same property set — a cache that returned stale or wrong accessors
        // would surface as a mismatch here.
        var first = DbCommandLoggerExtensions.ToDictionary(new { id = 1, name = "a" });
        var second = DbCommandLoggerExtensions.ToDictionary(new { id = 2, name = "b" });

        Assert.Equal(new[] { "id", "name" }, first.Keys);
        Assert.Equal(new[] { "id", "name" }, second.Keys);
        Assert.Equal(1, first["id"]);
        Assert.Equal(2, second["id"]);
    }



    [Fact]
    public void ToDictionary_when_given_a_dictionary_passes_it_through_without_reflection()
    {
        var dict = new Dictionary<string, object?> { ["id"] = 1 };

        var result = DbCommandLoggerExtensions.ToDictionary(dict);

        Assert.Same(dict, result);
    }
}
