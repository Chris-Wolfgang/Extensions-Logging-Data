using System;
using System.Collections.Generic;
using System.Data.Common;
using Microsoft.Extensions.Logging;

namespace Wolfgang.Extensions.Logging.Data;

/// <summary>
/// Extension methods on <see cref="ILogger"/> for logging <see cref="DbParameterCollection"/> instances.
/// </summary>
/// <remarks>
/// Each call emits a single structured log entry containing the count and a snapshot of every
/// parameter in the collection. Parameter values can be selectively redacted by passing a list of
/// parameter names; matching is case-insensitive and tolerant of provider prefixes
/// (<c>@name</c>, <c>:name</c>, <c>?name</c>, and <c>name</c> are all equivalent).
/// </remarks>
public static class DbParameterCollectionLoggerExtensions
{
    private const string MessageTemplate =
        "DbParameterCollection Count={Count} Parameters={Parameters}";



    /// <summary>
    /// Logs a single structured entry describing the <paramref name="parameters"/> at
    /// <see cref="Microsoft.Extensions.Logging.LogLevel"/>.Information.
    /// </summary>
    /// <param name="logger">The logger to write to.</param>
    /// <param name="parameters">The parameter collection to log.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="logger"/> or <paramref name="parameters"/> is <see langword="null"/>.
    /// </exception>
    public static void LogDbParameterCollection(this ILogger logger, DbParameterCollection parameters)
    {
        LogDbParameterCollection(logger, parameters, excludedParameterNames: null, LogLevel.Information);
    }



    /// <summary>
    /// Logs a single structured entry describing the <paramref name="parameters"/> at the specified
    /// <paramref name="level"/>.
    /// </summary>
    /// <param name="logger">The logger to write to.</param>
    /// <param name="parameters">The parameter collection to log.</param>
    /// <param name="level">The log level to write the entry at.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="logger"/> or <paramref name="parameters"/> is <see langword="null"/>.
    /// </exception>
    public static void LogDbParameterCollection(this ILogger logger, DbParameterCollection parameters, LogLevel level)
    {
        LogDbParameterCollection(logger, parameters, excludedParameterNames: null, level);
    }



    /// <summary>
    /// Logs a single structured entry describing the <paramref name="parameters"/> at
    /// <see cref="Microsoft.Extensions.Logging.LogLevel"/>.Information, redacting the values of any
    /// parameters whose names appear in <paramref name="excludedParameterNames"/>.
    /// </summary>
    /// <param name="logger">The logger to write to.</param>
    /// <param name="parameters">The parameter collection to log.</param>
    /// <param name="excludedParameterNames">
    /// Parameter names whose values should be replaced with the literal string <c>[REDACTED]</c>.
    /// Matching is case-insensitive and ignores provider prefixes (<c>@</c>, <c>:</c>, <c>?</c>).
    /// May be <see langword="null"/> for no redaction.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="logger"/> or <paramref name="parameters"/> is <see langword="null"/>.
    /// </exception>
    public static void LogDbParameterCollection
    (
        this ILogger logger,
        DbParameterCollection parameters,
        IEnumerable<string>? excludedParameterNames
    )
    {
        LogDbParameterCollection(logger, parameters, excludedParameterNames, LogLevel.Information);
    }



    /// <summary>
    /// Logs a single structured entry describing the <paramref name="parameters"/> at the specified
    /// <paramref name="level"/>, redacting the values of any parameters whose names appear in
    /// <paramref name="excludedParameterNames"/>.
    /// </summary>
    /// <param name="logger">The logger to write to.</param>
    /// <param name="parameters">The parameter collection to log.</param>
    /// <param name="excludedParameterNames">
    /// Parameter names whose values should be replaced with the literal string <c>[REDACTED]</c>.
    /// Matching is case-insensitive and ignores provider prefixes (<c>@</c>, <c>:</c>, <c>?</c>).
    /// May be <see langword="null"/> for no redaction.
    /// </param>
    /// <param name="level">The log level to write the entry at.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="logger"/> or <paramref name="parameters"/> is <see langword="null"/>.
    /// </exception>
    public static void LogDbParameterCollection
    (
        this ILogger logger,
        DbParameterCollection parameters,
        IEnumerable<string>? excludedParameterNames,
        LogLevel level
    )
    {
        if (logger is null)
        {
            throw new ArgumentNullException(nameof(logger));
        }

        if (parameters is null)
        {
            throw new ArgumentNullException(nameof(parameters));
        }

        if (!logger.IsEnabled(level))
        {
            return;
        }

        var snapshots = DbCommandLoggerExtensions.SnapshotParameters(parameters, excludedParameterNames);

        logger.Log
        (
            level,
            MessageTemplate,
            parameters.Count,
            snapshots
        );
    }
}
