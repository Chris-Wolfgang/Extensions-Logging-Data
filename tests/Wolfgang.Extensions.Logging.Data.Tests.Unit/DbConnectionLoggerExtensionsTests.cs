using System;
using System.Data;
using System.Data.Common;
using Microsoft.Extensions.Logging;
using Wolfgang.Extensions.Logging.Data.Tests.Unit.TestHelpers;

namespace Wolfgang.Extensions.Logging.Data.Tests.Unit;

public class DbConnectionLoggerExtensionsTests
{
    [Fact]
    public void LogDbConnection_when_logger_is_null_throws_ArgumentNullException()
    {
        ILogger logger = null!;
        var connection = new FakeDbConnection();

        Assert.Throws<ArgumentNullException>(() => logger.LogDbConnection(connection));
    }



    [Fact]
    public void LogDbConnection_with_level_when_logger_is_null_throws_ArgumentNullException()
    {
        ILogger logger = null!;
        var connection = new FakeDbConnection();

        Assert.Throws<ArgumentNullException>(() => logger.LogDbConnection(connection, LogLevel.Warning));
    }



    [Fact]
    public void LogDbConnection_when_connection_is_null_throws_ArgumentNullException()
    {
        var logger = new RecordingLogger();

        Assert.Throws<ArgumentNullException>(() => logger.LogDbConnection(null!));
    }



    [Fact]
    public void LogDbConnection_with_level_when_connection_is_null_throws_ArgumentNullException()
    {
        var logger = new RecordingLogger();

        Assert.Throws<ArgumentNullException>(() => logger.LogDbConnection(null!, LogLevel.Warning));
    }



    [Fact]
    public void LogDbConnection_when_log_level_is_disabled_writes_nothing()
    {
        var logger = new RecordingLogger { MinimumLevel = LogLevel.Warning };
        var connection = new FakeDbConnection();

        logger.LogDbConnection(connection, LogLevel.Information);

        Assert.Empty(logger.Entries);
    }



    [Fact]
    public void LogDbConnection_default_overload_logs_at_information()
    {
        var logger = new RecordingLogger();
        var connection = new FakeDbConnection();

        logger.LogDbConnection(connection);

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
    public void LogDbConnection_logs_at_specified_level(LogLevel level)
    {
        var logger = new RecordingLogger();
        var connection = new FakeDbConnection();

        logger.LogDbConnection(connection, level);

        var entry = Assert.Single(logger.Entries);
        Assert.Equal(level, entry.Level);
    }



    [Fact]
    public void LogDbConnection_includes_database_data_source_state_timeout_in_log_values()
    {
        var logger = new RecordingLogger();
        var connection = new FakeDbConnection
        {
            DatabaseValue = "MyDb",
            DataSourceValue = "tcp:server.example.com",
            StateValue = ConnectionState.Open,
            ServerVersionValue = "16.0.1000",
            ConnectionTimeoutValue = 45
        };

        logger.LogDbConnection(connection);

        var entry = Assert.Single(logger.Entries);
        Assert.Equal("MyDb", entry.GetValue("Database"));
        Assert.Equal("tcp:server.example.com", entry.GetValue("DataSource"));
        Assert.Equal(ConnectionState.Open, entry.GetValue("State"));
        Assert.Equal(45, entry.GetValue("ConnectionTimeout"));
        Assert.Equal("16.0.1000", entry.GetValue("ServerVersion"));
    }



    [Fact]
    public void LogDbConnection_includes_connection_type_full_name()
    {
        var logger = new RecordingLogger();
        var connection = new FakeDbConnection();

        logger.LogDbConnection(connection);

        var entry = Assert.Single(logger.Entries);
        Assert.Equal(typeof(FakeDbConnection).FullName, entry.GetValue("ConnectionType"));
    }



    [Fact]
    public void LogDbConnection_when_connection_is_closed_logs_null_server_version()
    {
        var logger = new RecordingLogger();
        var connection = new FakeDbConnection
        {
            StateValue = ConnectionState.Closed,
            ServerVersionValue = "ignored-because-closed"
        };

        logger.LogDbConnection(connection);

        var entry = Assert.Single(logger.Entries);
        Assert.Null(entry.GetValue("ServerVersion"));
    }



    [Fact]
    public void LogDbConnection_when_server_version_throws_InvalidOperationException_logs_null_server_version()
    {
        var logger = new RecordingLogger();
        var connection = new FakeDbConnection
        {
            StateValue = ConnectionState.Open,
            ServerVersionException = new InvalidOperationException("not available")
        };

        logger.LogDbConnection(connection);

        var entry = Assert.Single(logger.Entries);
        Assert.Null(entry.GetValue("ServerVersion"));
    }



    [Fact]
    public void LogDbConnection_redacts_password_from_logged_connection_string()
    {
        var logger = new RecordingLogger();
        var connection = new FakeDbConnection
        {
            ConnectionString = "Server=foo;User ID=bob;Password=topsecret"
        };

        logger.LogDbConnection(connection);

        var entry = Assert.Single(logger.Entries);
        var redacted = (string)entry.GetValue("ConnectionString")!;
        Assert.DoesNotContain("topsecret", redacted, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("bob", redacted, StringComparison.OrdinalIgnoreCase);
    }



    [Fact]
    public void RedactConnectionString_with_null_returns_empty_string()
    {
        Assert.Equal(string.Empty, DbConnectionLoggerExtensions.RedactConnectionString(connectionString: null));
    }



    [Fact]
    public void RedactConnectionString_with_empty_returns_empty_string()
    {
        Assert.Equal(string.Empty, DbConnectionLoggerExtensions.RedactConnectionString(string.Empty));
    }



    [Fact]
    public void RedactConnectionString_removes_Password_key()
    {
        var redacted = DbConnectionLoggerExtensions.RedactConnectionString("Server=foo;Password=secret");

        var parsed = new DbConnectionStringBuilder { ConnectionString = redacted };
        Assert.False(parsed.ContainsKey("Password"));
        Assert.True(parsed.ContainsKey("Server"));
    }



    [Fact]
    public void RedactConnectionString_removes_Pwd_key()
    {
        var redacted = DbConnectionLoggerExtensions.RedactConnectionString("Server=foo;Pwd=secret");

        var parsed = new DbConnectionStringBuilder { ConnectionString = redacted };
        Assert.False(parsed.ContainsKey("Pwd"));
        Assert.True(parsed.ContainsKey("Server"));
    }



    [Theory]
    [InlineData("PASSWORD")]
    [InlineData("password")]
    [InlineData("PassWord")]
    public void RedactConnectionString_removes_password_case_insensitively(string casing)
    {
        var redacted = DbConnectionLoggerExtensions.RedactConnectionString($"Server=foo;{casing}=secret");

        Assert.DoesNotContain("secret", redacted, StringComparison.OrdinalIgnoreCase);
    }



    [Theory]
    [InlineData("PWD")]
    [InlineData("pwd")]
    [InlineData("Pwd")]
    public void RedactConnectionString_removes_pwd_case_insensitively(string casing)
    {
        var redacted = DbConnectionLoggerExtensions.RedactConnectionString($"Server=foo;{casing}=secret");

        Assert.DoesNotContain("secret", redacted, StringComparison.OrdinalIgnoreCase);
    }



    [Fact]
    public void RedactConnectionString_keeps_user_id_value()
    {
        var redacted = DbConnectionLoggerExtensions.RedactConnectionString("Server=foo;User ID=bob;Password=secret");

        var parsed = new DbConnectionStringBuilder { ConnectionString = redacted };
        Assert.True(parsed.ContainsKey("User ID"));
        Assert.Equal("bob", parsed["User ID"]);
    }



    [Fact]
    public void RedactConnectionString_with_no_password_preserves_all_keys()
    {
        var redacted = DbConnectionLoggerExtensions.RedactConnectionString("Server=foo;User ID=bob");

        var parsed = new DbConnectionStringBuilder { ConnectionString = redacted };
        Assert.Equal("foo", parsed["Server"]);
        Assert.Equal("bob", parsed["User ID"]);
    }



    [Fact]
    public void RedactConnectionString_with_both_Password_and_Pwd_removes_both_values()
    {
        var redacted = DbConnectionLoggerExtensions.RedactConnectionString("Server=foo;Password=p1;Pwd=p2");

        Assert.DoesNotContain("p1", redacted, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("p2", redacted, StringComparison.OrdinalIgnoreCase);
    }
}
