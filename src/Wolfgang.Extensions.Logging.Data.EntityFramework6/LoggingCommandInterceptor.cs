using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity.Infrastructure.Interception;
using Microsoft.Extensions.Logging;
using Wolfgang.Extensions.Logging.Data;

namespace Wolfgang.Extensions.Logging.Data.EntityFramework6;

/// <summary>
/// EF6 <see cref="IDbCommandInterceptor"/> that logs every command the EF6 pipeline
/// executes. The <c>*Executing</c> callbacks fire before the command runs and are
/// routed to <c>ILogger.LogDbCommand</c>;
/// the <c>*Executed</c> callbacks log a failure at <c>LogLevel.Error</c> when the
/// interception context carries an exception (EF6 surfaces the exception on the context
/// rather than throwing at the interceptor).
/// </summary>
internal sealed class LoggingCommandInterceptor : IDbCommandInterceptor
{
    private readonly ILogger _logger;
    private readonly IEnumerable<string> _excludedParameterNames;
    private readonly LogLevel _level;

    public LoggingCommandInterceptor(ILogger logger, IEnumerable<string> excludedParameterNames, LogLevel level)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _excludedParameterNames = excludedParameterNames ?? Array.Empty<string>();
        _level = level;
    }

    public void NonQueryExecuting(DbCommand command, DbCommandInterceptionContext<int> interceptionContext)
    {
        LogExecuting(command);
    }

    public void NonQueryExecuted(DbCommand command, DbCommandInterceptionContext<int> interceptionContext)
    {
        LogFailure(command, interceptionContext);
    }

    public void ReaderExecuting(DbCommand command, DbCommandInterceptionContext<DbDataReader> interceptionContext)
    {
        LogExecuting(command);
    }

    public void ReaderExecuted(DbCommand command, DbCommandInterceptionContext<DbDataReader> interceptionContext)
    {
        LogFailure(command, interceptionContext);
    }

    public void ScalarExecuting(DbCommand command, DbCommandInterceptionContext<object> interceptionContext)
    {
        LogExecuting(command);
    }

    public void ScalarExecuted(DbCommand command, DbCommandInterceptionContext<object> interceptionContext)
    {
        LogFailure(command, interceptionContext);
    }

    private void LogExecuting(DbCommand command)
    {
        if (command is null)
        {
            return;
        }

        _logger.LogDbCommand(command, _excludedParameterNames, _level);
    }

    private void LogFailure<TResult>(DbCommand command, DbCommandInterceptionContext<TResult> interceptionContext)
    {
        if (command is null || interceptionContext?.Exception is null)
        {
            return;
        }

        _logger.Log(LogLevel.Error, interceptionContext.Exception, "EF6 command failed: {CommandText}", command.CommandText);
    }
}
