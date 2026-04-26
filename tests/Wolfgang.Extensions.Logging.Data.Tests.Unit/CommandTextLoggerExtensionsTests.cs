using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Wolfgang.Extensions.Logging.Data.Tests.Unit.TestHelpers;

namespace Wolfgang.Extensions.Logging.Data.Tests.Unit;

public class CommandTextLoggerExtensionsTests
{
    private static IReadOnlyDictionary<string, object?> Params(params (string Name, object? Value)[] items)
    {
        var dict = new Dictionary<string, object?>(items.Length);
        foreach (var (name, value) in items)
        {
            dict[name] = value;
        }
        return dict;
    }



    // ---- Null guards ----

    [Fact]
    public void LogCommandText_text_only_when_logger_is_null_throws_ArgumentNullException()
    {
        ILogger logger = null!;

        Assert.Throws<ArgumentNullException>(() => logger.LogCommandText("SELECT 1"));
    }



    [Fact]
    public void LogCommandText_text_with_level_when_logger_is_null_throws_ArgumentNullException()
    {
        ILogger logger = null!;

        Assert.Throws<ArgumentNullException>(() => logger.LogCommandText("SELECT 1", LogLevel.Warning));
    }



    [Fact]
    public void LogCommandText_with_parameters_when_logger_is_null_throws_ArgumentNullException()
    {
        ILogger logger = null!;

        Assert.Throws<ArgumentNullException>(() => logger.LogCommandText("SELECT 1", Params()));
    }



    [Fact]
    public void LogCommandText_with_parameters_and_level_when_logger_is_null_throws_ArgumentNullException()
    {
        ILogger logger = null!;

        Assert.Throws<ArgumentNullException>(() => logger.LogCommandText("SELECT 1", Params(), LogLevel.Warning));
    }



    [Fact]
    public void LogCommandText_with_excluded_when_logger_is_null_throws_ArgumentNullException()
    {
        ILogger logger = null!;

        Assert.Throws<ArgumentNullException>(() => logger.LogCommandText("SELECT 1", Params(), new[] { "@p" }));
    }



    [Fact]
    public void LogCommandText_with_excluded_and_level_when_logger_is_null_throws_ArgumentNullException()
    {
        ILogger logger = null!;

        Assert.Throws<ArgumentNullException>(() => logger.LogCommandText("SELECT 1", Params(), new[] { "@p" }, LogLevel.Warning));
    }



    [Fact]
    public void LogCommandText_text_only_when_text_is_null_throws_ArgumentNullException()
    {
        var logger = new RecordingLogger();

        Assert.Throws<ArgumentNullException>(() => logger.LogCommandText(commandText: null!));
    }



    [Fact]
    public void LogCommandText_text_with_level_when_text_is_null_throws_ArgumentNullException()
    {
        var logger = new RecordingLogger();

        Assert.Throws<ArgumentNullException>(() => logger.LogCommandText(null!, LogLevel.Warning));
    }



    [Fact]
    public void LogCommandText_with_parameters_when_text_is_null_throws_ArgumentNullException()
    {
        var logger = new RecordingLogger();

        Assert.Throws<ArgumentNullException>(() => logger.LogCommandText(null!, Params()));
    }



    [Fact]
    public void LogCommandText_with_parameters_when_parameters_is_null_throws_ArgumentNullException()
    {
        var logger = new RecordingLogger();

        Assert.Throws<ArgumentNullException>(() => logger.LogCommandText("SELECT 1", parameters: null!));
    }



    [Fact]
    public void LogCommandText_with_parameters_and_excluded_when_parameters_is_null_throws_ArgumentNullException()
    {
        var logger = new RecordingLogger();

        Assert.Throws<ArgumentNullException>(() => logger.LogCommandText("SELECT 1", parameters: null!, new[] { "@p" }));
    }



    // ---- Level filtering ----

    [Fact]
    public void LogCommandText_text_only_when_log_level_is_disabled_writes_nothing()
    {
        var logger = new RecordingLogger { MinimumLevel = LogLevel.Warning };

        logger.LogCommandText("SELECT 1", LogLevel.Information);

        Assert.Empty(logger.Entries);
    }



    [Fact]
    public void LogCommandText_with_parameters_when_log_level_is_disabled_writes_nothing()
    {
        var logger = new RecordingLogger { MinimumLevel = LogLevel.Warning };

        logger.LogCommandText("SELECT 1", Params(("@p", 1)), LogLevel.Information);

        Assert.Empty(logger.Entries);
    }



    [Fact]
    public void LogCommandText_text_only_default_overload_logs_at_information()
    {
        var logger = new RecordingLogger();

        logger.LogCommandText("SELECT 1");

        var entry = Assert.Single(logger.Entries);
        Assert.Equal(LogLevel.Information, entry.Level);
    }



    [Fact]
    public void LogCommandText_with_parameters_default_overload_logs_at_information()
    {
        var logger = new RecordingLogger();

        logger.LogCommandText("SELECT 1", Params());

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
    public void LogCommandText_text_only_logs_at_specified_level(LogLevel level)
    {
        var logger = new RecordingLogger();

        logger.LogCommandText("SELECT 1", level);

        var entry = Assert.Single(logger.Entries);
        Assert.Equal(level, entry.Level);
    }



    // ---- Content ----

    [Fact]
    public void LogCommandText_text_only_logs_command_text_field()
    {
        var logger = new RecordingLogger();

        logger.LogCommandText("SELECT * FROM Users");

        var entry = Assert.Single(logger.Entries);
        Assert.Equal("SELECT * FROM Users", entry.GetValue("CommandText"));
    }



    [Fact]
    public void LogCommandText_text_only_does_not_include_parameters_field()
    {
        var logger = new RecordingLogger();

        logger.LogCommandText("SELECT 1");

        var entry = Assert.Single(logger.Entries);
        Assert.Null(entry.GetValue("Parameters"));
    }



    [Fact]
    public void LogCommandText_with_empty_parameters_logs_empty_dictionary()
    {
        var logger = new RecordingLogger();

        logger.LogCommandText("SELECT 1", Params());

        var entry = Assert.Single(logger.Entries);
        var parameters = (IReadOnlyDictionary<string, object?>)entry.GetValue("Parameters")!;
        Assert.Empty(parameters);
    }



    [Fact]
    public void LogCommandText_with_parameters_logs_command_text_and_parameters()
    {
        var logger = new RecordingLogger();
        var input = Params(("@id", 42), ("@name", "alice"));

        logger.LogCommandText("SELECT * FROM Users WHERE Id=@id AND Name=@name", input);

        var entry = Assert.Single(logger.Entries);
        Assert.Equal("SELECT * FROM Users WHERE Id=@id AND Name=@name", entry.GetValue("CommandText"));
        var parameters = (IReadOnlyDictionary<string, object?>)entry.GetValue("Parameters")!;
        Assert.Equal(2, parameters.Count);
        Assert.Equal(42, parameters["@id"]);
        Assert.Equal("alice", parameters["@name"]);
    }



    // ---- Redaction ----

    [Fact]
    public void LogCommandText_with_excluded_redacts_only_named_value()
    {
        var logger = new RecordingLogger();
        var input = Params(("@user", "alice"), ("@password", "secret"));

        logger.LogCommandText("CALL Login(@user, @password)", input, new[] { "@password" });

        var entry = Assert.Single(logger.Entries);
        var parameters = (IReadOnlyDictionary<string, object?>)entry.GetValue("Parameters")!;
        Assert.Equal("alice", parameters["@user"]);
        Assert.Equal("[REDACTED]", parameters["@password"]);
    }



    [Theory]
    [InlineData("@password")]
    [InlineData("password")]
    [InlineData("PASSWORD")]
    [InlineData("PassWord")]
    [InlineData(":password")]
    [InlineData("?password")]
    public void LogCommandText_excluded_name_matches_case_insensitively_and_ignores_prefix(string excludedName)
    {
        var logger = new RecordingLogger();
        var input = Params(("@password", "secret"));

        logger.LogCommandText("CALL Login(@password)", input, new[] { excludedName });

        var entry = Assert.Single(logger.Entries);
        var parameters = (IReadOnlyDictionary<string, object?>)entry.GetValue("Parameters")!;
        Assert.Equal("[REDACTED]", parameters["@password"]);
    }



    [Theory]
    [InlineData("@password")]
    [InlineData(":password")]
    [InlineData("?password")]
    [InlineData("password")]
    public void LogCommandText_redacts_dict_key_regardless_of_actual_prefix(string actualKey)
    {
        var logger = new RecordingLogger();
        var input = Params((actualKey, "secret"));

        logger.LogCommandText("CALL Login", input, new[] { "password" });

        var entry = Assert.Single(logger.Entries);
        var parameters = (IReadOnlyDictionary<string, object?>)entry.GetValue("Parameters")!;
        Assert.Equal("[REDACTED]", parameters[actualKey]);
    }



    [Fact]
    public void LogCommandText_with_null_excluded_does_not_redact()
    {
        var logger = new RecordingLogger();
        var input = Params(("@password", "secret"));

        logger.LogCommandText("CALL Login(@password)", input, excludedParameterNames: null);

        var entry = Assert.Single(logger.Entries);
        var parameters = (IReadOnlyDictionary<string, object?>)entry.GetValue("Parameters")!;
        Assert.Equal("secret", parameters["@password"]);
    }



    [Fact]
    public void LogCommandText_with_excluded_and_level_uses_both()
    {
        var logger = new RecordingLogger();
        var input = Params(("@p", "v"));

        logger.LogCommandText("X", input, new[] { "p" }, LogLevel.Warning);

        var entry = Assert.Single(logger.Entries);
        Assert.Equal(LogLevel.Warning, entry.Level);
        var parameters = (IReadOnlyDictionary<string, object?>)entry.GetValue("Parameters")!;
        Assert.Equal("[REDACTED]", parameters["@p"]);
    }



    [Fact]
    public void LogCommandText_with_multiple_excluded_redacts_all_matching_values()
    {
        var logger = new RecordingLogger();
        var input = Params(("@user", "alice"), ("@password", "p1"), ("@token", "t1"));

        logger.LogCommandText("X", input, new[] { "password", "token" });

        var entry = Assert.Single(logger.Entries);
        var parameters = (IReadOnlyDictionary<string, object?>)entry.GetValue("Parameters")!;
        Assert.Equal("alice", parameters["@user"]);
        Assert.Equal("[REDACTED]", parameters["@password"]);
        Assert.Equal("[REDACTED]", parameters["@token"]);
    }



    [Fact]
    public void LogCommandText_preserves_null_parameter_values_when_not_excluded()
    {
        var logger = new RecordingLogger();
        var input = Params(("@id", null));

        logger.LogCommandText("SELECT * WHERE id=@id", input);

        var entry = Assert.Single(logger.Entries);
        var parameters = (IReadOnlyDictionary<string, object?>)entry.GetValue("Parameters")!;
        Assert.Null(parameters["@id"]);
    }



    [Fact]
    public void ApplyRedaction_with_no_excluded_returns_input_dictionary_unchanged()
    {
        var input = Params(("@p", 1));

        var result = CommandTextLoggerExtensions.ApplyRedaction(input, excludedParameterNames: null);

        Assert.Same(input, result);
    }



    [Fact]
    public void ApplyRedaction_with_empty_input_returns_input_dictionary_unchanged()
    {
        var input = Params();

        var result = CommandTextLoggerExtensions.ApplyRedaction(input, new[] { "anything" });

        Assert.Same(input, result);
    }
}
