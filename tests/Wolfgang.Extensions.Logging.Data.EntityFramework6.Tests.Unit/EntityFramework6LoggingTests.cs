using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure.Interception;
using Microsoft.Extensions.Logging;
using Wolfgang.Extensions.Logging.Data.EntityFramework6.Tests.Unit.TestHelpers;

namespace Wolfgang.Extensions.Logging.Data.EntityFramework6.Tests.Unit;

public class EntityFramework6LoggingTests
{
    [Fact]
    public void AddLoggingInterceptors_when_logger_is_null_throws_ArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>
        (
            () => EntityFramework6Logging.AddLoggingInterceptors(null!, null, LogLevel.Information, _ => { })
        );
    }



    [Fact]
    public void AddLoggingInterceptors_registers_command_connection_and_transaction_interceptors()
    {
        var logger = new RecordingLogger();
        var registered = new List<IDbInterceptor>();

        EntityFramework6Logging.AddLoggingInterceptors(logger, null, LogLevel.Information, registered.Add);

        Assert.Collection
        (
            registered,
            i => Assert.IsType<LoggingCommandInterceptor>(i),
            i => Assert.IsType<LoggingConnectionInterceptor>(i),
            i => Assert.IsType<LoggingTransactionInterceptor>(i)
        );
    }



    [Fact]
    public void AddLoggingInterceptors_when_called_twice_with_same_logger_registers_once()
    {
        var logger = new RecordingLogger();
        var registered = new List<IDbInterceptor>();

        EntityFramework6Logging.AddLoggingInterceptors(logger, null, LogLevel.Information, registered.Add);
        EntityFramework6Logging.AddLoggingInterceptors(logger, null, LogLevel.Information, registered.Add);

        Assert.Equal(3, registered.Count);
    }



    [Fact]
    public void AddLoggingInterceptors_with_distinct_loggers_registers_each()
    {
        var registered = new List<IDbInterceptor>();

        EntityFramework6Logging.AddLoggingInterceptors(new RecordingLogger(), null, LogLevel.Information, registered.Add);
        EntityFramework6Logging.AddLoggingInterceptors(new RecordingLogger(), null, LogLevel.Information, registered.Add);

        Assert.Equal(6, registered.Count);
    }



    [Fact]
    public void AddLoggingInterceptors_public_overload_registers_into_EF6_without_throwing()
    {
        // Exercises the real DbInterception.Add path. The interceptors register into EF6's
        // process-global pipeline but never fire (no DbContext runs in a unit test), and a
        // repeat call with the same logger is the documented no-op.
        var logger = new RecordingLogger();

        EntityFramework6Logging.AddLoggingInterceptors(logger);
        EntityFramework6Logging.AddLoggingInterceptors(logger, new[] { "password" }, LogLevel.Debug);

        Assert.Empty(logger.Entries);
    }
}
