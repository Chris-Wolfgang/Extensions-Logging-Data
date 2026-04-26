using System;
using System.Data;
using System.Data.Common;
using Microsoft.Extensions.Logging;

namespace Wolfgang.Extensions.Logging.Data;

/// <summary>
/// Extension methods on <see cref="ILogger"/> for logging <see cref="DbConnection"/> instances.
/// </summary>
/// <remarks>
/// The <see cref="DbConnection.ConnectionString"/> is parsed via
/// <see cref="DbConnectionStringBuilder"/> and the <c>Password</c> and <c>Pwd</c> keys are
/// removed before logging so credentials are never written to the log. All other keys,
/// including the user name (<c>User ID</c>, <c>UID</c>, etc.), are preserved.
/// </remarks>
public static class DbConnectionLoggerExtensions
{
    private const string MessageTemplate =
        "DbConnection {ConnectionType} Database={Database} DataSource={DataSource} ServerVersion={ServerVersion} State={State} ConnectionTimeout={ConnectionTimeout} ConnectionString={ConnectionString}";



    /// <summary>
    /// Logs a single structured entry describing the <paramref name="connection"/> at
    /// <see cref="Microsoft.Extensions.Logging.LogLevel"/>.Information.
    /// </summary>
    /// <param name="logger">The logger to write to.</param>
    /// <param name="connection">The connection to log.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="logger"/> or <paramref name="connection"/> is <see langword="null"/>.
    /// </exception>
    /// <example>
    /// <code>
    /// using var connection = new SqlConnection(connectionString);
    /// await connection.OpenAsync();
    /// logger.LogDbConnection(connection);
    /// </code>
    /// </example>
    public static void LogDbConnection(this ILogger logger, DbConnection connection)
    {
        LogDbConnection(logger, connection, LogLevel.Information);
    }



    /// <summary>
    /// Logs a single structured entry describing the <paramref name="connection"/> at the
    /// specified <paramref name="level"/>.
    /// </summary>
    /// <param name="logger">The logger to write to.</param>
    /// <param name="connection">The connection to log.</param>
    /// <param name="level">The log level to write the entry at.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="logger"/> or <paramref name="connection"/> is <see langword="null"/>.
    /// </exception>
    public static void LogDbConnection(this ILogger logger, DbConnection connection, LogLevel level)
    {
        if (logger is null)
        {
            throw new ArgumentNullException(nameof(logger));
        }

        if (connection is null)
        {
            throw new ArgumentNullException(nameof(connection));
        }

        if (!logger.IsEnabled(level))
        {
            return;
        }

        var redactedConnectionString = RedactConnectionString(connection.ConnectionString);
        var serverVersion = TryGetServerVersion(connection);

        logger.Log
        (
            level,
            MessageTemplate,
            connection.GetType().FullName,
            connection.Database,
            connection.DataSource,
            serverVersion,
            connection.State,
            connection.ConnectionTimeout,
            redactedConnectionString
        );
    }



    /// <summary>
    /// Parses <paramref name="connectionString"/> via <see cref="DbConnectionStringBuilder"/>
    /// and removes the <c>Password</c> and <c>Pwd</c> keys (case-insensitive).
    /// </summary>
    /// <param name="connectionString">The raw connection string. May be <see langword="null"/> or empty.</param>
    /// <returns>
    /// The connection string with password keys removed, or <see cref="string.Empty"/> when the
    /// input is null or empty.
    /// </returns>
    internal static string RedactConnectionString(string? connectionString)
    {
        if (string.IsNullOrEmpty(connectionString))
        {
            return string.Empty;
        }

        var builder = new DbConnectionStringBuilder
        {
            ConnectionString = connectionString
        };

        if (builder.ContainsKey("Password"))
        {
            builder.Remove("Password");
        }

        if (builder.ContainsKey("Pwd"))
        {
            builder.Remove("Pwd");
        }

        return builder.ConnectionString ?? string.Empty;
    }



    private static string? TryGetServerVersion(DbConnection connection)
    {
        if (connection.State != ConnectionState.Open)
        {
            return null;
        }

        try
        {
            return connection.ServerVersion;
        }
        catch (InvalidOperationException)
        {
            return null;
        }
    }
}
