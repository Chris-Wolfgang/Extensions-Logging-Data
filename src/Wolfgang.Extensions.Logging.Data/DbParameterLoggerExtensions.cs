using System;
using System.Data.Common;
using Microsoft.Extensions.Logging;

namespace Wolfgang.Extensions.Logging.Data;

/// <summary>
/// Extension methods on <see cref="ILogger"/> for logging individual <see cref="DbParameter"/> instances.
/// </summary>
/// <remarks>
/// Each call emits a single structured log entry containing the parameter's metadata
/// (<c>ParameterName</c>, <c>DbType</c>, <c>Direction</c>, <c>Size</c>, <c>Precision</c>,
/// <c>Scale</c>, <c>IsNullable</c>) and either its <c>Value</c> or the literal
/// string <c>[REDACTED]</c> when the caller passes <c>redactValue: true</c>.
/// </remarks>
public static class DbParameterLoggerExtensions
{
    private const string MessageTemplate =
        "DbParameter Name={ParameterName} DbType={DbType} Direction={Direction} Size={Size} Precision={Precision} Scale={Scale} IsNullable={IsNullable} Value={Value}";

    private const string RedactedValue = "[REDACTED]";



    /// <summary>
    /// Logs a single structured entry describing the <paramref name="parameter"/> at
    /// <see cref="Microsoft.Extensions.Logging.LogLevel"/>.Information.
    /// </summary>
    /// <param name="logger">The logger to write to.</param>
    /// <param name="parameter">The parameter to log.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="logger"/> or <paramref name="parameter"/> is <see langword="null"/>.
    /// </exception>
    public static void LogDbParameter(this ILogger logger, DbParameter parameter)
    {
        LogDbParameter(logger, parameter, redactValue: false, LogLevel.Information);
    }



    /// <summary>
    /// Logs a single structured entry describing the <paramref name="parameter"/> at the specified
    /// <paramref name="level"/>.
    /// </summary>
    /// <param name="logger">The logger to write to.</param>
    /// <param name="parameter">The parameter to log.</param>
    /// <param name="level">The log level to write the entry at.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="logger"/> or <paramref name="parameter"/> is <see langword="null"/>.
    /// </exception>
    public static void LogDbParameter(this ILogger logger, DbParameter parameter, LogLevel level)
    {
        LogDbParameter(logger, parameter, redactValue: false, level);
    }



    /// <summary>
    /// Logs a single structured entry describing the <paramref name="parameter"/> at
    /// <see cref="Microsoft.Extensions.Logging.LogLevel"/>.Information, optionally redacting the value.
    /// </summary>
    /// <param name="logger">The logger to write to.</param>
    /// <param name="parameter">The parameter to log.</param>
    /// <param name="redactValue">
    /// When <see langword="true"/>, the parameter value is replaced with the literal string
    /// <c>[REDACTED]</c>. All other metadata is preserved.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="logger"/> or <paramref name="parameter"/> is <see langword="null"/>.
    /// </exception>
    public static void LogDbParameter(this ILogger logger, DbParameter parameter, bool redactValue)
    {
        LogDbParameter(logger, parameter, redactValue, LogLevel.Information);
    }



    /// <summary>
    /// Logs a single structured entry describing the <paramref name="parameter"/> at the specified
    /// <paramref name="level"/>, optionally redacting the value.
    /// </summary>
    /// <param name="logger">The logger to write to.</param>
    /// <param name="parameter">The parameter to log.</param>
    /// <param name="redactValue">
    /// When <see langword="true"/>, the parameter value is replaced with the literal string
    /// <c>[REDACTED]</c>. All other metadata is preserved.
    /// </param>
    /// <param name="level">The log level to write the entry at.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="logger"/> or <paramref name="parameter"/> is <see langword="null"/>.
    /// </exception>
    public static void LogDbParameter
    (
        this ILogger logger,
        DbParameter parameter,
        bool redactValue,
        LogLevel level
    )
    {
        if (logger is null)
        {
            throw new ArgumentNullException(nameof(logger));
        }

        if (parameter is null)
        {
            throw new ArgumentNullException(nameof(parameter));
        }

        if (!logger.IsEnabled(level))
        {
            return;
        }

        var value = redactValue ? (object?)RedactedValue : parameter.Value;

        logger.Log
        (
            level,
            MessageTemplate,
            parameter.ParameterName,
            parameter.DbType,
            parameter.Direction,
            parameter.Size,
            parameter.Precision,
            parameter.Scale,
            parameter.IsNullable,
            value
        );
    }
}
