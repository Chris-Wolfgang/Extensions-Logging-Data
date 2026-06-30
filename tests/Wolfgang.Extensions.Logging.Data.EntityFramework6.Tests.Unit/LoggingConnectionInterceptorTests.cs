using System;
using System.Data.Entity.Infrastructure.Interception;
using Microsoft.Extensions.Logging;
using Wolfgang.Extensions.Logging.Data.EntityFramework6.Tests.Unit.TestHelpers;

namespace Wolfgang.Extensions.Logging.Data.EntityFramework6.Tests.Unit;

public class LoggingConnectionInterceptorTests
{
    [Fact]
    public void Opening_logs_the_connection_at_the_configured_level()
    {
        var logger = new RecordingLogger();
        var sut = new LoggingConnectionInterceptor(logger, LogLevel.Debug);

        sut.Opening(new FakeDbConnection(), new DbConnectionInterceptionContext());

        var entry = Assert.Single(logger.Entries);
        Assert.Equal(LogLevel.Debug, entry.Level);
        // The rich connection entry carries the redacted connection string.
        var connectionString = (string)entry.GetValue("ConnectionString")!;
        Assert.DoesNotContain("secret", connectionString, StringComparison.OrdinalIgnoreCase);
    }



    [Fact]
    public void Opening_with_null_connection_logs_nothing()
    {
        var logger = new RecordingLogger();
        var sut = new LoggingConnectionInterceptor(logger, LogLevel.Information);

        sut.Opening(null!, new DbConnectionInterceptionContext());

        Assert.Empty(logger.Entries);
    }



    [Fact]
    public void Opened_without_an_exception_logs_a_state_transition()
    {
        var logger = new RecordingLogger();
        var sut = new LoggingConnectionInterceptor(logger, LogLevel.Information);

        sut.Opened(new FakeDbConnection(), new DbConnectionInterceptionContext());

        var entry = Assert.Single(logger.Entries);
        Assert.Equal(LogLevel.Information, entry.Level);
        Assert.Contains("opened", entry.Message, StringComparison.OrdinalIgnoreCase);
    }



    [Fact]
    public void Opened_with_an_exception_logs_it_at_Error()
    {
        var logger = new RecordingLogger();
        var sut = new LoggingConnectionInterceptor(logger, LogLevel.Information);
        var boom = new InvalidOperationException("open failed");
        var context = new DbConnectionInterceptionContext { Exception = boom };

        sut.Opened(new FakeDbConnection(), context);

        var entry = Assert.Single(logger.Entries);
        Assert.Equal(LogLevel.Error, entry.Level);
        Assert.Same(boom, entry.Exception);
    }



    [Fact]
    public void Closing_logs_nothing()
    {
        var logger = new RecordingLogger();
        var sut = new LoggingConnectionInterceptor(logger, LogLevel.Information);

        sut.Closing(new FakeDbConnection(), new DbConnectionInterceptionContext());

        Assert.Empty(logger.Entries);
    }



    [Fact]
    public void Closed_without_an_exception_logs_a_state_transition()
    {
        var logger = new RecordingLogger();
        var sut = new LoggingConnectionInterceptor(logger, LogLevel.Information);

        sut.Closed(new FakeDbConnection(), new DbConnectionInterceptionContext());

        Assert.Contains("closed", Assert.Single(logger.Entries).Message, StringComparison.OrdinalIgnoreCase);
    }



    [Fact]
    public void Closed_with_an_exception_logs_it_at_Error()
    {
        var logger = new RecordingLogger();
        var sut = new LoggingConnectionInterceptor(logger, LogLevel.Information);
        var context = new DbConnectionInterceptionContext { Exception = new Exception("close failed") };

        sut.Closed(new FakeDbConnection(), context);

        Assert.Equal(LogLevel.Error, Assert.Single(logger.Entries).Level);
    }



    [Fact]
    public void Property_and_transaction_callbacks_are_silent_noops()
    {
        var logger = new RecordingLogger();
        var sut = new LoggingConnectionInterceptor(logger, LogLevel.Information);
        var c = new FakeDbConnection();

        sut.BeganTransaction(c, new BeginTransactionInterceptionContext());
        sut.BeginningTransaction(c, new BeginTransactionInterceptionContext());
        sut.ConnectionStringGetting(c, new DbConnectionInterceptionContext<string>());
        sut.ConnectionStringGot(c, new DbConnectionInterceptionContext<string>());
        sut.ConnectionStringSetting(c, new DbConnectionPropertyInterceptionContext<string>());
        sut.ConnectionStringSet(c, new DbConnectionPropertyInterceptionContext<string>());
        sut.ConnectionTimeoutGetting(c, new DbConnectionInterceptionContext<int>());
        sut.ConnectionTimeoutGot(c, new DbConnectionInterceptionContext<int>());
        sut.DatabaseGetting(c, new DbConnectionInterceptionContext<string>());
        sut.DatabaseGot(c, new DbConnectionInterceptionContext<string>());
        sut.DataSourceGetting(c, new DbConnectionInterceptionContext<string>());
        sut.DataSourceGot(c, new DbConnectionInterceptionContext<string>());
        sut.Disposing(c, new DbConnectionInterceptionContext());
        sut.Disposed(c, new DbConnectionInterceptionContext());
        sut.EnlistedTransaction(c, new EnlistTransactionInterceptionContext());
        sut.EnlistingTransaction(c, new EnlistTransactionInterceptionContext());
        sut.ServerVersionGetting(c, new DbConnectionInterceptionContext<string>());
        sut.ServerVersionGot(c, new DbConnectionInterceptionContext<string>());
        sut.StateGetting(c, new DbConnectionInterceptionContext<System.Data.ConnectionState>());
        sut.StateGot(c, new DbConnectionInterceptionContext<System.Data.ConnectionState>());

        Assert.Empty(logger.Entries);
    }
}
