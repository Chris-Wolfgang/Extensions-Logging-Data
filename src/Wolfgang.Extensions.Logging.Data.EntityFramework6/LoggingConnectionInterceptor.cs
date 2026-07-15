using System;
using System.Data;
using System.Data.Common;
using System.Data.Entity.Infrastructure.Interception;
using Microsoft.Extensions.Logging;

namespace Wolfgang.Extensions.Logging.Data.EntityFramework6;

/// <summary>
/// EF6 <see cref="IDbConnectionInterceptor"/> that logs the connection lifecycle. The rich,
/// redacted connection entry is written on <see cref="Opening"/> via
/// <c>ILogger.LogDbConnection</c>;
/// open/close transitions are logged at the configured level and a failed open/close is logged
/// at <c>LogLevel.Error</c> from the interception context's exception. The many
/// property-getter / setter callbacks EF6 fires (<c>DataSourceGetting</c>, <c>StateGot</c>, …)
/// are intentionally no-ops — logging them would bury the meaningful events in noise.
/// </summary>
internal sealed class LoggingConnectionInterceptor : IDbConnectionInterceptor
{
    private readonly ILogger _logger;
    private readonly LogLevel _level;

    public LoggingConnectionInterceptor(ILogger logger, LogLevel level)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _level = level;
    }

    public void Opening(DbConnection connection, DbConnectionInterceptionContext interceptionContext)
    {
        if (connection != null)
        {
            _logger.LogDbConnection(connection, _level);
        }
    }

    public void Opened(DbConnection connection, DbConnectionInterceptionContext interceptionContext)
    {
        if (!LogErrorIfFailed(interceptionContext, "EF6 connection open failed"))
        {
            _logger.Log(_level, "EF6 connection opened");
        }
    }

    public void Closing(DbConnection connection, DbConnectionInterceptionContext interceptionContext)
    {
    }

    public void Closed(DbConnection connection, DbConnectionInterceptionContext interceptionContext)
    {
        if (!LogErrorIfFailed(interceptionContext, "EF6 connection close failed"))
        {
            _logger.Log(_level, "EF6 connection closed");
        }
    }

    private bool LogErrorIfFailed(DbConnectionInterceptionContext interceptionContext, string message)
    {
        if (interceptionContext?.Exception is null)
        {
            return false;
        }

        _logger.Log(LogLevel.Error, interceptionContext.Exception, message);
        return true;
    }

    // The remaining callbacks are deliberate no-ops: transaction lifecycle is handled by
    // LoggingTransactionInterceptor, and the property accessor callbacks are too noisy for
    // passive logging.

    public void BeganTransaction(DbConnection connection, BeginTransactionInterceptionContext interceptionContext)
    {
    }

    public void BeginningTransaction(DbConnection connection, BeginTransactionInterceptionContext interceptionContext)
    {
    }

    public void ConnectionStringGetting(DbConnection connection, DbConnectionInterceptionContext<string> interceptionContext)
    {
    }

    public void ConnectionStringGot(DbConnection connection, DbConnectionInterceptionContext<string> interceptionContext)
    {
    }

    public void ConnectionStringSetting(DbConnection connection, DbConnectionPropertyInterceptionContext<string> interceptionContext)
    {
    }

    public void ConnectionStringSet(DbConnection connection, DbConnectionPropertyInterceptionContext<string> interceptionContext)
    {
    }

    public void ConnectionTimeoutGetting(DbConnection connection, DbConnectionInterceptionContext<int> interceptionContext)
    {
    }

    public void ConnectionTimeoutGot(DbConnection connection, DbConnectionInterceptionContext<int> interceptionContext)
    {
    }

    public void DatabaseGetting(DbConnection connection, DbConnectionInterceptionContext<string> interceptionContext)
    {
    }

    public void DatabaseGot(DbConnection connection, DbConnectionInterceptionContext<string> interceptionContext)
    {
    }

    public void DataSourceGetting(DbConnection connection, DbConnectionInterceptionContext<string> interceptionContext)
    {
    }

    public void DataSourceGot(DbConnection connection, DbConnectionInterceptionContext<string> interceptionContext)
    {
    }

    public void Disposing(DbConnection connection, DbConnectionInterceptionContext interceptionContext)
    {
    }

    public void Disposed(DbConnection connection, DbConnectionInterceptionContext interceptionContext)
    {
    }

    public void EnlistedTransaction(DbConnection connection, EnlistTransactionInterceptionContext interceptionContext)
    {
    }

    public void EnlistingTransaction(DbConnection connection, EnlistTransactionInterceptionContext interceptionContext)
    {
    }

    public void ServerVersionGetting(DbConnection connection, DbConnectionInterceptionContext<string> interceptionContext)
    {
    }

    public void ServerVersionGot(DbConnection connection, DbConnectionInterceptionContext<string> interceptionContext)
    {
    }

    public void StateGetting(DbConnection connection, DbConnectionInterceptionContext<ConnectionState> interceptionContext)
    {
    }

    public void StateGot(DbConnection connection, DbConnectionInterceptionContext<ConnectionState> interceptionContext)
    {
    }
}
