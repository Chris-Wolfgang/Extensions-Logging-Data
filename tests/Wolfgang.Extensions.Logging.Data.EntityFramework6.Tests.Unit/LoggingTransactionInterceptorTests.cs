using System;
using System.Data.Entity.Infrastructure.Interception;
using Microsoft.Extensions.Logging;
using Wolfgang.Extensions.Logging.Data.EntityFramework6.Tests.Unit.TestHelpers;

namespace Wolfgang.Extensions.Logging.Data.EntityFramework6.Tests.Unit;

public class LoggingTransactionInterceptorTests
{
    [Fact]
    public void Committing_logs_at_the_configured_level()
    {
        var logger = new RecordingLogger();
        var sut = new LoggingTransactionInterceptor(logger, LogLevel.Debug);

        sut.Committing(null!, new DbTransactionInterceptionContext());

        var entry = Assert.Single(logger.Entries);
        Assert.Equal(LogLevel.Debug, entry.Level);
        Assert.Contains("committing", entry.Message, StringComparison.OrdinalIgnoreCase);
    }



    [Fact]
    public void Committed_without_an_exception_logs_a_state_transition()
    {
        var logger = new RecordingLogger();
        var sut = new LoggingTransactionInterceptor(logger, LogLevel.Information);

        sut.Committed(null!, new DbTransactionInterceptionContext());

        Assert.Contains("committed", Assert.Single(logger.Entries).Message, StringComparison.OrdinalIgnoreCase);
    }



    [Fact]
    public void Committed_with_an_exception_logs_it_at_Error()
    {
        var logger = new RecordingLogger();
        var sut = new LoggingTransactionInterceptor(logger, LogLevel.Information);
        var boom = new InvalidOperationException("commit failed");
        var context = new DbTransactionInterceptionContext { Exception = boom };

        sut.Committed(null!, context);

        var entry = Assert.Single(logger.Entries);
        Assert.Equal(LogLevel.Error, entry.Level);
        Assert.Same(boom, entry.Exception);
    }



    [Fact]
    public void RollingBack_logs_at_the_configured_level()
    {
        var logger = new RecordingLogger();
        var sut = new LoggingTransactionInterceptor(logger, LogLevel.Information);

        sut.RollingBack(null!, new DbTransactionInterceptionContext());

        Assert.Contains("rolling back", Assert.Single(logger.Entries).Message, StringComparison.OrdinalIgnoreCase);
    }



    [Fact]
    public void RolledBack_without_an_exception_logs_a_state_transition()
    {
        var logger = new RecordingLogger();
        var sut = new LoggingTransactionInterceptor(logger, LogLevel.Information);

        sut.RolledBack(null!, new DbTransactionInterceptionContext());

        Assert.Contains("rolled back", Assert.Single(logger.Entries).Message, StringComparison.OrdinalIgnoreCase);
    }



    [Fact]
    public void RolledBack_with_an_exception_logs_it_at_Error()
    {
        var logger = new RecordingLogger();
        var sut = new LoggingTransactionInterceptor(logger, LogLevel.Information);
        var context = new DbTransactionInterceptionContext { Exception = new Exception("rollback failed") };

        sut.RolledBack(null!, context);

        Assert.Equal(LogLevel.Error, Assert.Single(logger.Entries).Level);
    }



    [Fact]
    public void Accessor_callbacks_are_silent_noops()
    {
        var logger = new RecordingLogger();
        var sut = new LoggingTransactionInterceptor(logger, LogLevel.Information);

        sut.ConnectionGetting(null!, new DbTransactionInterceptionContext<System.Data.Common.DbConnection>());
        sut.ConnectionGot(null!, new DbTransactionInterceptionContext<System.Data.Common.DbConnection>());
        sut.IsolationLevelGetting(null!, new DbTransactionInterceptionContext<System.Data.IsolationLevel>());
        sut.IsolationLevelGot(null!, new DbTransactionInterceptionContext<System.Data.IsolationLevel>());
        sut.Disposing(null!, new DbTransactionInterceptionContext());
        sut.Disposed(null!, new DbTransactionInterceptionContext());

        Assert.Empty(logger.Entries);
    }
}
