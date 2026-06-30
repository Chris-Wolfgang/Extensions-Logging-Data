using System;
using System.Data;
using System.Data.Common;
using System.Data.Entity.Infrastructure.Interception;
using Microsoft.Extensions.Logging;

namespace Wolfgang.Extensions.Logging.Data.EntityFramework6;

/// <summary>
/// EF6 <see cref="IDbTransactionInterceptor"/> that logs transaction commit and rollback at
/// the configured level, and logs a failed commit/rollback at <c>LogLevel.Error</c>
/// from the interception context's exception. The connection / isolation-level / dispose
/// accessor callbacks are intentional no-ops.
/// </summary>
internal sealed class LoggingTransactionInterceptor : IDbTransactionInterceptor
{
    private readonly ILogger _logger;
    private readonly LogLevel _level;

    public LoggingTransactionInterceptor(ILogger logger, LogLevel level)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _level = level;
    }

    public void Committing(DbTransaction transaction, DbTransactionInterceptionContext interceptionContext)
    {
        _logger.Log(_level, "EF6 transaction committing");
    }

    public void Committed(DbTransaction transaction, DbTransactionInterceptionContext interceptionContext)
    {
        if (!LogErrorIfFailed(interceptionContext, "EF6 transaction commit failed"))
        {
            _logger.Log(_level, "EF6 transaction committed");
        }
    }

    public void RollingBack(DbTransaction transaction, DbTransactionInterceptionContext interceptionContext)
    {
        _logger.Log(_level, "EF6 transaction rolling back");
    }

    public void RolledBack(DbTransaction transaction, DbTransactionInterceptionContext interceptionContext)
    {
        if (!LogErrorIfFailed(interceptionContext, "EF6 transaction rollback failed"))
        {
            _logger.Log(_level, "EF6 transaction rolled back");
        }
    }

    private bool LogErrorIfFailed(DbTransactionInterceptionContext interceptionContext, string message)
    {
        if (interceptionContext?.Exception is null)
        {
            return false;
        }

        _logger.Log(LogLevel.Error, interceptionContext.Exception, message);
        return true;
    }

    // Deliberate no-ops — accessor callbacks carry no value for passive logging.

    public void ConnectionGetting(DbTransaction transaction, DbTransactionInterceptionContext<DbConnection> interceptionContext)
    {
    }

    public void ConnectionGot(DbTransaction transaction, DbTransactionInterceptionContext<DbConnection> interceptionContext)
    {
    }

    public void Disposing(DbTransaction transaction, DbTransactionInterceptionContext interceptionContext)
    {
    }

    public void Disposed(DbTransaction transaction, DbTransactionInterceptionContext interceptionContext)
    {
    }

    public void IsolationLevelGetting(DbTransaction transaction, DbTransactionInterceptionContext<IsolationLevel> interceptionContext)
    {
    }

    public void IsolationLevelGot(DbTransaction transaction, DbTransactionInterceptionContext<IsolationLevel> interceptionContext)
    {
    }
}
