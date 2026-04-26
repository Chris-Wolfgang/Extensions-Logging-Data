using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace Wolfgang.Extensions.Logging.Data;

/// <summary>
/// Extension methods on <see cref="ILogger"/> for logging raw SQL command text and an optional
/// dictionary of parameter values, for callers that don't have a <see cref="System.Data.Common.DbCommand"/>
/// in hand.
/// </summary>
/// <remarks>
/// Each call emits a single structured log entry. Parameter values can be selectively redacted by
/// passing a list of parameter names; matching is case-insensitive and tolerant of provider prefixes
/// (<c>@name</c>, <c>:name</c>, <c>?name</c>, and <c>name</c> are all equivalent). Redacted values
/// are replaced with the literal string <c>[REDACTED]</c>.
/// </remarks>
public static class CommandTextLoggerExtensions
{
    private const string TextOnlyTemplate = "CommandText {CommandText}";

    private const string TextAndParametersTemplate = "CommandText {CommandText} Parameters={Parameters}";

    private const string RedactedValue = "[REDACTED]";



    /// <summary>
    /// Logs <paramref name="commandText"/> as a single structured entry at
    /// <see cref="Microsoft.Extensions.Logging.LogLevel"/>.Information.
    /// </summary>
    /// <param name="logger">The logger to write to.</param>
    /// <param name="commandText">The SQL command text to log.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="logger"/> or <paramref name="commandText"/> is <see langword="null"/>.
    /// </exception>
    public static void LogCommandText(this ILogger logger, string commandText)
    {
        LogCommandText(logger, commandText, LogLevel.Information);
    }



    /// <summary>
    /// Logs <paramref name="commandText"/> as a single structured entry at the specified
    /// <paramref name="level"/>.
    /// </summary>
    /// <param name="logger">The logger to write to.</param>
    /// <param name="commandText">The SQL command text to log.</param>
    /// <param name="level">The log level to write the entry at.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="logger"/> or <paramref name="commandText"/> is <see langword="null"/>.
    /// </exception>
    public static void LogCommandText(this ILogger logger, string commandText, LogLevel level)
    {
        if (logger is null)
        {
            throw new ArgumentNullException(nameof(logger));
        }

        if (commandText is null)
        {
            throw new ArgumentNullException(nameof(commandText));
        }

        if (!logger.IsEnabled(level))
        {
            return;
        }

        logger.Log(level, TextOnlyTemplate, commandText);
    }



    /// <summary>
    /// Logs <paramref name="commandText"/> together with <paramref name="parameters"/> as a single
    /// structured entry at <see cref="Microsoft.Extensions.Logging.LogLevel"/>.Information.
    /// </summary>
    /// <param name="logger">The logger to write to.</param>
    /// <param name="commandText">The SQL command text to log.</param>
    /// <param name="parameters">The parameter name/value pairs to log alongside the command text.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when any argument is <see langword="null"/>.
    /// </exception>
    public static void LogCommandText
    (
        this ILogger logger,
        string commandText,
        IReadOnlyDictionary<string, object?> parameters
    )
    {
        LogCommandText(logger, commandText, parameters, excludedParameterNames: null, LogLevel.Information);
    }



    /// <summary>
    /// Logs <paramref name="commandText"/> together with <paramref name="parameters"/> as a single
    /// structured entry at the specified <paramref name="level"/>.
    /// </summary>
    /// <param name="logger">The logger to write to.</param>
    /// <param name="commandText">The SQL command text to log.</param>
    /// <param name="parameters">The parameter name/value pairs to log alongside the command text.</param>
    /// <param name="level">The log level to write the entry at.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when any argument is <see langword="null"/>.
    /// </exception>
    public static void LogCommandText
    (
        this ILogger logger,
        string commandText,
        IReadOnlyDictionary<string, object?> parameters,
        LogLevel level
    )
    {
        LogCommandText(logger, commandText, parameters, excludedParameterNames: null, level);
    }



    /// <summary>
    /// Logs <paramref name="commandText"/> together with <paramref name="parameters"/> as a single
    /// structured entry at <see cref="Microsoft.Extensions.Logging.LogLevel"/>.Information, redacting
    /// the values of any parameters whose names appear in <paramref name="excludedParameterNames"/>.
    /// </summary>
    /// <param name="logger">The logger to write to.</param>
    /// <param name="commandText">The SQL command text to log.</param>
    /// <param name="parameters">The parameter name/value pairs to log alongside the command text.</param>
    /// <param name="excludedParameterNames">
    /// Parameter names whose values should be replaced with the literal string <c>[REDACTED]</c>.
    /// Matching is case-insensitive and ignores provider prefixes (<c>@</c>, <c>:</c>, <c>?</c>).
    /// May be <see langword="null"/> for no redaction.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="logger"/>, <paramref name="commandText"/>, or
    /// <paramref name="parameters"/> is <see langword="null"/>.
    /// </exception>
    public static void LogCommandText
    (
        this ILogger logger,
        string commandText,
        IReadOnlyDictionary<string, object?> parameters,
        IEnumerable<string>? excludedParameterNames
    )
    {
        LogCommandText(logger, commandText, parameters, excludedParameterNames, LogLevel.Information);
    }



    /// <summary>
    /// Logs <paramref name="commandText"/> together with <paramref name="parameters"/> as a single
    /// structured entry at the specified <paramref name="level"/>, redacting the values of any
    /// parameters whose names appear in <paramref name="excludedParameterNames"/>.
    /// </summary>
    /// <param name="logger">The logger to write to.</param>
    /// <param name="commandText">The SQL command text to log.</param>
    /// <param name="parameters">The parameter name/value pairs to log alongside the command text.</param>
    /// <param name="excludedParameterNames">
    /// Parameter names whose values should be replaced with the literal string <c>[REDACTED]</c>.
    /// Matching is case-insensitive and ignores provider prefixes (<c>@</c>, <c>:</c>, <c>?</c>).
    /// May be <see langword="null"/> for no redaction.
    /// </param>
    /// <param name="level">The log level to write the entry at.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="logger"/>, <paramref name="commandText"/>, or
    /// <paramref name="parameters"/> is <see langword="null"/>.
    /// </exception>
    public static void LogCommandText
    (
        this ILogger logger,
        string commandText,
        IReadOnlyDictionary<string, object?> parameters,
        IEnumerable<string>? excludedParameterNames,
        LogLevel level
    )
    {
        if (logger is null)
        {
            throw new ArgumentNullException(nameof(logger));
        }

        if (commandText is null)
        {
            throw new ArgumentNullException(nameof(commandText));
        }

        if (parameters is null)
        {
            throw new ArgumentNullException(nameof(parameters));
        }

        if (!logger.IsEnabled(level))
        {
            return;
        }

        var loggedParameters = ApplyRedaction(parameters, excludedParameterNames);

        logger.Log(level, TextAndParametersTemplate, commandText, loggedParameters);
    }



    internal static IReadOnlyDictionary<string, object?> ApplyRedaction
    (
        IReadOnlyDictionary<string, object?> parameters,
        IEnumerable<string>? excludedParameterNames
    )
    {
        var excluded = DbCommandLoggerExtensions.NormalizeExcluded(excludedParameterNames);
        if (excluded.Count == 0 || parameters.Count == 0)
        {
            return parameters;
        }

        var result = new Dictionary<string, object?>(parameters.Count);
        foreach (var kvp in parameters)
        {
            var redact = DbCommandLoggerExtensions.ShouldRedact(kvp.Key, excluded);
            result[kvp.Key] = redact ? RedactedValue : kvp.Value;
        }
        return result;
    }
}
