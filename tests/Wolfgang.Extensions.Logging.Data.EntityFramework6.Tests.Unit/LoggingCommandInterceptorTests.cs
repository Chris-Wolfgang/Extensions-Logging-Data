using System;
using System.Data.Entity.Infrastructure.Interception;
using Microsoft.Extensions.Logging;
using Wolfgang.Extensions.Logging.Data.EntityFramework6.Tests.Unit.TestHelpers;

namespace Wolfgang.Extensions.Logging.Data.EntityFramework6.Tests.Unit;

public class LoggingCommandInterceptorTests
{
    private static FakeDbCommand SampleCommand()
    {
        var command = new FakeDbCommand { CommandText = "SELECT * FROM Users WHERE Id = @id" };
        command.AddParameter("@id", 1);
        command.AddParameter("@password", "hunter2");
        return command;
    }



    [Fact]
    public void NonQueryExecuting_logs_the_command_at_the_configured_level()
    {
        var logger = new RecordingLogger();
        var sut = new LoggingCommandInterceptor(logger, Array.Empty<string>(), LogLevel.Debug);

        sut.NonQueryExecuting(SampleCommand(), new DbCommandInterceptionContext<int>());

        var entry = Assert.Single(logger.Entries);
        Assert.Equal(LogLevel.Debug, entry.Level);
        Assert.Equal("SELECT * FROM Users WHERE Id = @id", entry.GetValue("CommandText"));
    }



    [Fact]
    public void ReaderExecuting_logs_the_command()
    {
        var logger = new RecordingLogger();
        var sut = new LoggingCommandInterceptor(logger, Array.Empty<string>(), LogLevel.Information);

        sut.ReaderExecuting(SampleCommand(), new DbCommandInterceptionContext<System.Data.Common.DbDataReader>());

        Assert.Single(logger.Entries);
    }



    [Fact]
    public void ScalarExecuting_logs_the_command()
    {
        var logger = new RecordingLogger();
        var sut = new LoggingCommandInterceptor(logger, Array.Empty<string>(), LogLevel.Information);

        sut.ScalarExecuting(SampleCommand(), new DbCommandInterceptionContext<object>());

        Assert.Single(logger.Entries);
    }



    [Fact]
    public void Executing_redacts_excluded_parameter_values()
    {
        var logger = new RecordingLogger();
        var sut = new LoggingCommandInterceptor(logger, new[] { "password" }, LogLevel.Information);

        sut.NonQueryExecuting(SampleCommand(), new DbCommandInterceptionContext<int>());

        var rendered = (string)Assert.Single(logger.Entries).GetValue("Parameters")!;
        Assert.DoesNotContain("hunter2", rendered, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("@password=***", rendered, StringComparison.Ordinal);
    }



    [Fact]
    public void Executing_with_null_command_logs_nothing()
    {
        var logger = new RecordingLogger();
        var sut = new LoggingCommandInterceptor(logger, Array.Empty<string>(), LogLevel.Information);

        sut.NonQueryExecuting(null!, new DbCommandInterceptionContext<int>());

        Assert.Empty(logger.Entries);
    }



    [Fact]
    public void Executed_without_an_exception_logs_nothing()
    {
        var logger = new RecordingLogger();
        var sut = new LoggingCommandInterceptor(logger, Array.Empty<string>(), LogLevel.Information);

        sut.NonQueryExecuted(SampleCommand(), new DbCommandInterceptionContext<int>());

        Assert.Empty(logger.Entries);
    }



    [Fact]
    public void NonQueryExecuted_with_an_exception_logs_it_at_Error()
    {
        var logger = new RecordingLogger();
        var sut = new LoggingCommandInterceptor(logger, Array.Empty<string>(), LogLevel.Information);
        var boom = new InvalidOperationException("boom");
        var context = new DbCommandInterceptionContext<int> { Exception = boom };

        sut.NonQueryExecuted(SampleCommand(), context);

        var entry = Assert.Single(logger.Entries);
        Assert.Equal(LogLevel.Error, entry.Level);
        Assert.Same(boom, entry.Exception);
    }



    [Fact]
    public void ReaderExecuted_with_an_exception_logs_it_at_Error()
    {
        var logger = new RecordingLogger();
        var sut = new LoggingCommandInterceptor(logger, Array.Empty<string>(), LogLevel.Information);
        var context = new DbCommandInterceptionContext<System.Data.Common.DbDataReader> { Exception = new Exception("x") };

        sut.ReaderExecuted(SampleCommand(), context);

        Assert.Equal(LogLevel.Error, Assert.Single(logger.Entries).Level);
    }



    [Fact]
    public void ScalarExecuted_with_an_exception_logs_it_at_Error()
    {
        var logger = new RecordingLogger();
        var sut = new LoggingCommandInterceptor(logger, Array.Empty<string>(), LogLevel.Information);
        var context = new DbCommandInterceptionContext<object> { Exception = new Exception("x") };

        sut.ScalarExecuted(SampleCommand(), context);

        Assert.Equal(LogLevel.Error, Assert.Single(logger.Entries).Level);
    }
}
