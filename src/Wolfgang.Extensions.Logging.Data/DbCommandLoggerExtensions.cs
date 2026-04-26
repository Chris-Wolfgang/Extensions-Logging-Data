using System;
using System.Collections.Generic;
using System.Data.Common;
using Microsoft.Extensions.Logging;

namespace Wolfgang.Extensions.Logging.Data;

/// <summary>
/// Extension methods on <see cref="ILogger"/> for logging <see cref="DbCommand"/> instances.
/// </summary>
/// <remarks>
/// Each call emits a single structured log entry. The command's <see cref="DbCommand.Parameters"/>
/// are automatically captured into <see cref="LoggedDbParameter"/> snapshots and included in the entry.
/// Callers can pass a list of parameter names whose values should be replaced with the literal string
/// <c>[REDACTED]</c>; matching is case-insensitive and tolerant of provider prefixes
/// (<c>@name</c>, <c>:name</c>, <c>?name</c>, and <c>name</c> are all equivalent).
/// </remarks>
public static class DbCommandLoggerExtensions
{
    private const string MessageTemplate =
        "DbCommand {CommandType} CommandText={CommandText} CommandTimeout={CommandTimeout} Parameters={Parameters}";

    private const string RedactedValue = "[REDACTED]";



    /// <summary>
    /// Logs a single structured entry describing the <paramref name="command"/> and its parameters at
    /// <see cref="Microsoft.Extensions.Logging.LogLevel"/>.Information.
    /// </summary>
    /// <param name="logger">The logger to write to.</param>
    /// <param name="command">The command to log.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="logger"/> or <paramref name="command"/> is <see langword="null"/>.
    /// </exception>
    public static void LogDbCommand(this ILogger logger, DbCommand command)
    {
        LogDbCommand(logger, command, excludedParameterNames: null, LogLevel.Information);
    }



    /// <summary>
    /// Logs a single structured entry describing the <paramref name="command"/> and its parameters at
    /// the specified <paramref name="level"/>.
    /// </summary>
    /// <param name="logger">The logger to write to.</param>
    /// <param name="command">The command to log.</param>
    /// <param name="level">The log level to write the entry at.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="logger"/> or <paramref name="command"/> is <see langword="null"/>.
    /// </exception>
    public static void LogDbCommand(this ILogger logger, DbCommand command, LogLevel level)
    {
        LogDbCommand(logger, command, excludedParameterNames: null, level);
    }



    /// <summary>
    /// Logs a single structured entry describing the <paramref name="command"/> and its parameters at
    /// <see cref="Microsoft.Extensions.Logging.LogLevel"/>.Information, redacting the values of any
    /// parameters whose names appear in <paramref name="excludedParameterNames"/>.
    /// </summary>
    /// <param name="logger">The logger to write to.</param>
    /// <param name="command">The command to log.</param>
    /// <param name="excludedParameterNames">
    /// Parameter names whose values should be replaced with the literal string <c>[REDACTED]</c>.
    /// Matching is case-insensitive and ignores provider prefixes (<c>@</c>, <c>:</c>, <c>?</c>).
    /// May be <see langword="null"/> for no redaction.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="logger"/> or <paramref name="command"/> is <see langword="null"/>.
    /// </exception>
    public static void LogDbCommand(this ILogger logger, DbCommand command, IEnumerable<string>? excludedParameterNames)
    {
        LogDbCommand(logger, command, excludedParameterNames, LogLevel.Information);
    }



    /// <summary>
    /// Logs a single structured entry describing the <paramref name="command"/> and its parameters at
    /// the specified <paramref name="level"/>, redacting the values of any parameters whose names
    /// appear in <paramref name="excludedParameterNames"/>.
    /// </summary>
    /// <param name="logger">The logger to write to.</param>
    /// <param name="command">The command to log.</param>
    /// <param name="excludedParameterNames">
    /// Parameter names whose values should be replaced with the literal string <c>[REDACTED]</c>.
    /// Matching is case-insensitive and ignores provider prefixes (<c>@</c>, <c>:</c>, <c>?</c>).
    /// May be <see langword="null"/> for no redaction.
    /// </param>
    /// <param name="level">The log level to write the entry at.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="logger"/> or <paramref name="command"/> is <see langword="null"/>.
    /// </exception>
    public static void LogDbCommand
    (
        this ILogger logger,
        DbCommand command,
        IEnumerable<string>? excludedParameterNames,
        LogLevel level
    )
    {
        if (logger is null)
        {
            throw new ArgumentNullException(nameof(logger));
        }

        if (command is null)
        {
            throw new ArgumentNullException(nameof(command));
        }

        if (!logger.IsEnabled(level))
        {
            return;
        }

        var snapshots = SnapshotParameters(command.Parameters, excludedParameterNames);

        logger.Log
        (
            level,
            MessageTemplate,
            command.CommandType,
            command.CommandText,
            command.CommandTimeout,
            snapshots
        );
    }



    internal static List<LoggedDbParameter> SnapshotParameters
    (
        DbParameterCollection? parameters,
        IEnumerable<string>? excludedParameterNames
    )
    {
        if (parameters is null)
        {
            return new List<LoggedDbParameter>();
        }

        var excluded = NormalizeExcluded(excludedParameterNames);
        var result = new List<LoggedDbParameter>(parameters.Count);

        foreach (DbParameter p in parameters)
        {
            var redact = ShouldRedact(p.ParameterName, excluded);
            result.Add
            (
                new LoggedDbParameter
                (
                    p.ParameterName,
                    p.DbType,
                    p.Direction,
                    p.Size,
                    p.Precision,
                    p.Scale,
                    p.IsNullable,
                    redact ? RedactedValue : p.Value
                )
            );
        }

        return result;
    }



    internal static HashSet<string> NormalizeExcluded(IEnumerable<string>? excludedParameterNames)
    {
        var set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        if (excludedParameterNames is null)
        {
            return set;
        }

        foreach (var name in excludedParameterNames)
        {
            if (string.IsNullOrEmpty(name))
            {
                continue;
            }

            set.Add(StripPrefix(name));
        }

        return set;
    }



    internal static bool ShouldRedact(string? parameterName, HashSet<string> excluded)
    {
        if (excluded.Count == 0)
        {
            return false;
        }

        return excluded.Contains(StripPrefix(parameterName ?? string.Empty));
    }



    internal static string StripPrefix(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            return string.Empty;
        }

        var first = name[0];
        if (first == '@' || first == ':' || first == '?')
        {
            return name.Substring(1);
        }

        return name;
    }
}
