using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Logging;

namespace Wolfgang.Extensions.Logging.Data;

/// <summary>
/// Extension methods on <see cref="ILogger"/> for logging a SQL command's text
/// alongside its parameters, with opt-in redaction of sensitive parameter values.
/// </summary>
/// <remarks>
/// <para>
/// Two parameter-supplying styles are provided:
/// </para>
/// <list type="bullet">
/// <item>
/// An explicit <see cref="IReadOnlyDictionary{TKey, TValue}"/> of parameter name to
/// value — the lowest-overhead path.
/// </item>
/// <item>
/// A Dapper-style anonymous object (e.g. <c>new { id = 1, name = "abc" }</c>) — its
/// public readable properties are reflected into a dictionary and then logged via the
/// dictionary path. Per-type property accessors are cached so the reflection cost is
/// paid once per parameter type, not once per call.
/// </item>
/// </list>
/// <para>
/// Parameter-name matching for <c>excludedParameterNames</c> is case-insensitive and
/// prefix-tolerant: <c>@name</c>, <c>:name</c>, <c>?name</c>, and <c>name</c> are all
/// treated as the same parameter, so a connection's ADO.NET prefix convention does not
/// have to be known by the caller. The value of an excluded parameter is replaced with
/// <c>***</c> in the logged output; the parameter name itself is preserved so the shape
/// of the command is still visible.
/// </para>
/// </remarks>
public static class DbCommandLoggerExtensions
{
    private const string MessageTemplate =
        "DbCommand CommandText={CommandText} Parameters={Parameters}";

    private const string RedactedValue = "***";

    private const string ReflectionParametersTrimMessage =
        "The anonymous-object parameter overload reflects over the runtime type's public " +
        "properties, which the trimmer cannot statically preserve. In trimmed / Native AOT " +
        "applications, pass an IReadOnlyDictionary<string, object?> instead.";

    /// <summary>
    /// Per-type cache of "read the public properties of an instance of this type into a
    /// name/value dictionary" delegates. Building the property list via reflection is the
    /// expensive part of the anonymous-object path; caching it per <see cref="Type"/>
    /// means it happens once per parameter shape rather than once per call.
    /// </summary>
    private static readonly ConcurrentDictionary<Type, Func<object, IReadOnlyDictionary<string, object?>>> PropertyReaderCache = new();



    /// <summary>
    /// Logs the <paramref name="commandText"/> and its <paramref name="parameters"/> at
    /// <see cref="Microsoft.Extensions.Logging.LogLevel"/>.Information.
    /// </summary>
    /// <param name="logger">The logger to write to.</param>
    /// <param name="commandText">The SQL command text.</param>
    /// <param name="parameters">The command parameters, keyed by name.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="logger"/>, <paramref name="commandText"/>, or
    /// <paramref name="parameters"/> is <see langword="null"/>.
    /// </exception>
    /// <example>
    /// <code>
    /// var p = new Dictionary&lt;string, object?&gt; { ["id"] = 1, ["password"] = "hunter2" };
    /// logger.LogCommandText("SELECT * FROM Users WHERE Id = @id", p, new[] { "password" });
    /// </code>
    /// </example>
    public static void LogCommandText(this ILogger logger, string commandText, IReadOnlyDictionary<string, object?> parameters)
    {
        LogCommandText(logger, commandText, parameters, Array.Empty<string>(), LogLevel.Information);
    }



    /// <summary>
    /// Logs the <paramref name="commandText"/> and its <paramref name="parameters"/> at the
    /// specified <paramref name="level"/>.
    /// </summary>
    /// <param name="logger">The logger to write to.</param>
    /// <param name="commandText">The SQL command text.</param>
    /// <param name="parameters">The command parameters, keyed by name.</param>
    /// <param name="level">The log level to write the entry at.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="logger"/>, <paramref name="commandText"/>, or
    /// <paramref name="parameters"/> is <see langword="null"/>.
    /// </exception>
    public static void LogCommandText(this ILogger logger, string commandText, IReadOnlyDictionary<string, object?> parameters, LogLevel level)
    {
        LogCommandText(logger, commandText, parameters, Array.Empty<string>(), level);
    }



    /// <summary>
    /// Logs the <paramref name="commandText"/> and its <paramref name="parameters"/> at
    /// <see cref="Microsoft.Extensions.Logging.LogLevel"/>.Information, redacting the values of any parameters named in
    /// <paramref name="excludedParameterNames"/>.
    /// </summary>
    /// <param name="logger">The logger to write to.</param>
    /// <param name="commandText">The SQL command text.</param>
    /// <param name="parameters">The command parameters, keyed by name.</param>
    /// <param name="excludedParameterNames">
    /// Parameter names whose values should be redacted. Matching is case-insensitive and
    /// prefix-tolerant (<c>@name</c>, <c>:name</c>, <c>?name</c>, <c>name</c> are equivalent).
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="logger"/>, <paramref name="commandText"/>,
    /// <paramref name="parameters"/>, or <paramref name="excludedParameterNames"/> is
    /// <see langword="null"/>.
    /// </exception>
    public static void LogCommandText(this ILogger logger, string commandText, IReadOnlyDictionary<string, object?> parameters, IEnumerable<string> excludedParameterNames)
    {
        LogCommandText(logger, commandText, parameters, excludedParameterNames, LogLevel.Information);
    }



    /// <summary>
    /// Logs the <paramref name="commandText"/> and its <paramref name="parameters"/> at the
    /// specified <paramref name="level"/>, redacting the values of any parameters named in
    /// <paramref name="excludedParameterNames"/>.
    /// </summary>
    /// <param name="logger">The logger to write to.</param>
    /// <param name="commandText">The SQL command text.</param>
    /// <param name="parameters">The command parameters, keyed by name.</param>
    /// <param name="excludedParameterNames">
    /// Parameter names whose values should be redacted. Matching is case-insensitive and
    /// prefix-tolerant (<c>@name</c>, <c>:name</c>, <c>?name</c>, <c>name</c> are equivalent).
    /// </param>
    /// <param name="level">The log level to write the entry at.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="logger"/>, <paramref name="commandText"/>,
    /// <paramref name="parameters"/>, or <paramref name="excludedParameterNames"/> is
    /// <see langword="null"/>.
    /// </exception>
    public static void LogCommandText(this ILogger logger, string commandText, IReadOnlyDictionary<string, object?> parameters, IEnumerable<string> excludedParameterNames, LogLevel level)
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

        if (excludedParameterNames is null)
        {
            throw new ArgumentNullException(nameof(excludedParameterNames));
        }

        if (!logger.IsEnabled(level))
        {
            return;
        }

        var rendered = RenderParameters(parameters, excludedParameterNames);

        logger.Log
        (
            level,
            MessageTemplate,
            commandText,
            rendered
        );
    }



    /// <summary>
    /// Logs the <paramref name="commandText"/> and the public readable properties of the
    /// anonymous (or any) <paramref name="parameters"/> object at <see cref="Microsoft.Extensions.Logging.LogLevel"/>.Information.
    /// </summary>
    /// <param name="logger">The logger to write to.</param>
    /// <param name="commandText">The SQL command text.</param>
    /// <param name="parameters">
    /// An object whose public readable properties become the command parameters — e.g.
    /// <c>new { id = 1, name = "abc" }</c>.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="logger"/>, <paramref name="commandText"/>, or
    /// <paramref name="parameters"/> is <see langword="null"/>.
    /// </exception>
    /// <example>
    /// <code>
    /// logger.LogCommandText(
    ///     "SELECT * FROM Users WHERE Id = @id",
    ///     new { id = 1, password = "hunter2" },
    ///     new[] { "password" });
    /// </code>
    /// </example>
    [RequiresUnreferencedCode(ReflectionParametersTrimMessage)]
    public static void LogCommandText(this ILogger logger, string commandText, object parameters)
    {
        LogCommandText(logger, commandText, ToDictionary(parameters), Array.Empty<string>(), LogLevel.Information);
    }



    /// <summary>
    /// Logs the <paramref name="commandText"/> and the public readable properties of the
    /// <paramref name="parameters"/> object at the specified <paramref name="level"/>.
    /// </summary>
    /// <param name="logger">The logger to write to.</param>
    /// <param name="commandText">The SQL command text.</param>
    /// <param name="parameters">An object whose public readable properties become the command parameters.</param>
    /// <param name="level">The log level to write the entry at.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="logger"/>, <paramref name="commandText"/>, or
    /// <paramref name="parameters"/> is <see langword="null"/>.
    /// </exception>
    [RequiresUnreferencedCode(ReflectionParametersTrimMessage)]
    public static void LogCommandText(this ILogger logger, string commandText, object parameters, LogLevel level)
    {
        LogCommandText(logger, commandText, ToDictionary(parameters), Array.Empty<string>(), level);
    }



    /// <summary>
    /// Logs the <paramref name="commandText"/> and the public readable properties of the
    /// <paramref name="parameters"/> object at <see cref="Microsoft.Extensions.Logging.LogLevel"/>.Information, redacting
    /// the values of any properties named in <paramref name="excludedParameterNames"/>.
    /// </summary>
    /// <param name="logger">The logger to write to.</param>
    /// <param name="commandText">The SQL command text.</param>
    /// <param name="parameters">An object whose public readable properties become the command parameters.</param>
    /// <param name="excludedParameterNames">
    /// Parameter names whose values should be redacted. Matching is case-insensitive and
    /// prefix-tolerant (<c>@name</c>, <c>:name</c>, <c>?name</c>, <c>name</c> are equivalent).
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="logger"/>, <paramref name="commandText"/>,
    /// <paramref name="parameters"/>, or <paramref name="excludedParameterNames"/> is
    /// <see langword="null"/>.
    /// </exception>
    [RequiresUnreferencedCode(ReflectionParametersTrimMessage)]
    public static void LogCommandText(this ILogger logger, string commandText, object parameters, IEnumerable<string> excludedParameterNames)
    {
        LogCommandText(logger, commandText, ToDictionary(parameters), excludedParameterNames, LogLevel.Information);
    }



    /// <summary>
    /// Logs the <paramref name="commandText"/> and the public readable properties of the
    /// <paramref name="parameters"/> object at the specified <paramref name="level"/>, redacting
    /// the values of any properties named in <paramref name="excludedParameterNames"/>.
    /// </summary>
    /// <param name="logger">The logger to write to.</param>
    /// <param name="commandText">The SQL command text.</param>
    /// <param name="parameters">An object whose public readable properties become the command parameters.</param>
    /// <param name="excludedParameterNames">
    /// Parameter names whose values should be redacted. Matching is case-insensitive and
    /// prefix-tolerant (<c>@name</c>, <c>:name</c>, <c>?name</c>, <c>name</c> are equivalent).
    /// </param>
    /// <param name="level">The log level to write the entry at.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="logger"/>, <paramref name="commandText"/>,
    /// <paramref name="parameters"/>, or <paramref name="excludedParameterNames"/> is
    /// <see langword="null"/>.
    /// </exception>
    [RequiresUnreferencedCode(ReflectionParametersTrimMessage)]
    public static void LogCommandText(this ILogger logger, string commandText, object parameters, IEnumerable<string> excludedParameterNames, LogLevel level)
    {
        LogCommandText(logger, commandText, ToDictionary(parameters), excludedParameterNames, level);
    }



    /// <summary>
    /// Logs a live <see cref="DbCommand"/> — its <see cref="DbCommand.CommandText"/> and the
    /// name/value pairs of its <see cref="DbCommand.Parameters"/> — at <see cref="Microsoft.Extensions.Logging.LogLevel"/>.Information.
    /// </summary>
    /// <param name="logger">The logger to write to.</param>
    /// <param name="command">The command to log.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="logger"/> or <paramref name="command"/> is <see langword="null"/>.
    /// </exception>
    /// <example>
    /// <code>
    /// using var command = connection.CreateCommand();
    /// command.CommandText = "SELECT * FROM Users WHERE Id = @id";
    /// logger.LogDbCommand(command);
    /// </code>
    /// </example>
    public static void LogDbCommand(this ILogger logger, DbCommand command)
    {
        LogDbCommand(logger, command, Array.Empty<string>(), LogLevel.Information);
    }



    /// <summary>
    /// Logs a live <see cref="DbCommand"/> at the specified <paramref name="level"/>.
    /// </summary>
    /// <param name="logger">The logger to write to.</param>
    /// <param name="command">The command to log.</param>
    /// <param name="level">The log level to write the entry at.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="logger"/> or <paramref name="command"/> is <see langword="null"/>.
    /// </exception>
    public static void LogDbCommand(this ILogger logger, DbCommand command, LogLevel level)
    {
        LogDbCommand(logger, command, Array.Empty<string>(), level);
    }



    /// <summary>
    /// Logs a live <see cref="DbCommand"/> at <see cref="Microsoft.Extensions.Logging.LogLevel"/>.Information,
    /// redacting the values of any parameters named in <paramref name="excludedParameterNames"/>.
    /// </summary>
    /// <param name="logger">The logger to write to.</param>
    /// <param name="command">The command to log.</param>
    /// <param name="excludedParameterNames">
    /// Parameter names whose values should be redacted. Matching is case-insensitive and
    /// prefix-tolerant (<c>@name</c>, <c>:name</c>, <c>?name</c>, <c>name</c> are equivalent).
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="logger"/>, <paramref name="command"/>, or
    /// <paramref name="excludedParameterNames"/> is <see langword="null"/>.
    /// </exception>
    public static void LogDbCommand(this ILogger logger, DbCommand command, IEnumerable<string> excludedParameterNames)
    {
        LogDbCommand(logger, command, excludedParameterNames, LogLevel.Information);
    }



    /// <summary>
    /// Logs a live <see cref="DbCommand"/> at the specified <paramref name="level"/>, redacting the
    /// values of any parameters named in <paramref name="excludedParameterNames"/>.
    /// </summary>
    /// <param name="logger">The logger to write to.</param>
    /// <param name="command">The command to log.</param>
    /// <param name="excludedParameterNames">
    /// Parameter names whose values should be redacted. Matching is case-insensitive and
    /// prefix-tolerant (<c>@name</c>, <c>:name</c>, <c>?name</c>, <c>name</c> are equivalent).
    /// </param>
    /// <param name="level">The log level to write the entry at.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="logger"/>, <paramref name="command"/>, or
    /// <paramref name="excludedParameterNames"/> is <see langword="null"/>.
    /// </exception>
    public static void LogDbCommand(this ILogger logger, DbCommand command, IEnumerable<string> excludedParameterNames, LogLevel level)
    {
        if (logger is null)
        {
            throw new ArgumentNullException(nameof(logger));
        }

        if (command is null)
        {
            throw new ArgumentNullException(nameof(command));
        }

        if (excludedParameterNames is null)
        {
            throw new ArgumentNullException(nameof(excludedParameterNames));
        }

        if (!logger.IsEnabled(level))
        {
            return;
        }

        LogCommandText(logger, command.CommandText ?? string.Empty, ToDictionary(command.Parameters), excludedParameterNames, level);
    }



    /// <summary>
    /// Reads the name/value pairs of a <see cref="DbParameterCollection"/> into a dictionary,
    /// preserving each parameter's declared name (prefix and all) so the rendered output matches
    /// the command text. The last value wins if a name appears more than once.
    /// </summary>
    /// <param name="parameters">The parameter collection to read. Must not be <see langword="null"/>.</param>
    /// <returns>A dictionary of parameter name to value, with <see cref="DBNull"/> normalized to <see langword="null"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="parameters"/> is <see langword="null"/>.</exception>
    internal static IReadOnlyDictionary<string, object?> ToDictionary(DbParameterCollection parameters)
    {
        if (parameters is null)
        {
            throw new ArgumentNullException(nameof(parameters));
        }

        var result = new Dictionary<string, object?>(parameters.Count, StringComparer.Ordinal);
        foreach (DbParameter parameter in parameters)
        {
            result[parameter.ParameterName] = parameter.Value is DBNull ? null : parameter.Value;
        }

        return result;
    }



    /// <summary>
    /// Reflects the public readable instance properties of <paramref name="parameters"/> into a
    /// name/value dictionary, caching the property accessors per <see cref="Type"/>.
    /// </summary>
    /// <param name="parameters">The object to read. Must not be <see langword="null"/>.</param>
    /// <returns>A dictionary of property name to value.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="parameters"/> is <see langword="null"/>.</exception>
    [RequiresUnreferencedCode(ReflectionParametersTrimMessage)]
    internal static IReadOnlyDictionary<string, object?> ToDictionary(object parameters)
    {
        if (parameters is null)
        {
            throw new ArgumentNullException(nameof(parameters));
        }

        // A caller that already has a dictionary can flow straight through rather than
        // paying the reflection path (this also covers the unlikely case of the object
        // overload being selected for a dictionary via an `object`-typed local).
        if (parameters is IReadOnlyDictionary<string, object?> dictionary)
        {
            return dictionary;
        }

        var reader = PropertyReaderCache.GetOrAdd(parameters.GetType(), BuildPropertyReader);
        return reader(parameters);
    }



    /// <summary>
    /// Builds a delegate that reads the public readable instance properties of objects of the
    /// given <paramref name="type"/> into a dictionary. Called once per type via the cache.
    /// </summary>
    [RequiresUnreferencedCode(ReflectionParametersTrimMessage)]
    private static Func<object, IReadOnlyDictionary<string, object?>> BuildPropertyReader(Type type)
    {
        var properties = type
            .GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(p => p.CanRead && p.GetIndexParameters().Length == 0)
            .ToArray();

        return instance =>
        {
            var result = new Dictionary<string, object?>(properties.Length, StringComparer.Ordinal);
            foreach (var property in properties)
            {
                result[property.Name] = property.GetValue(instance);
            }

            return result;
        };
    }



    /// <summary>
    /// Renders the parameter set into an ordered, brace-wrapped string, replacing the values of
    /// excluded parameters with <see cref="RedactedValue"/>. Excluded-name matching strips a
    /// leading ADO.NET prefix (<c>@ : ?</c>) from both the parameter name and the excluded name
    /// and compares case-insensitively.
    /// </summary>
    private static string RenderParameters(IReadOnlyDictionary<string, object?> parameters, IEnumerable<string> excludedParameterNames)
    {
        var excluded = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var name in excludedParameterNames)
        {
            if (name is not null)
            {
                excluded.Add(NormalizeParameterName(name));
            }
        }

        var parts = new List<string>(parameters.Count);
        foreach (var kvp in parameters)
        {
            var isExcluded = excluded.Contains(NormalizeParameterName(kvp.Key));
            var value = isExcluded ? RedactedValue : (kvp.Value?.ToString() ?? "null");
            parts.Add($"{kvp.Key}={value}");
        }

        return "{" + string.Join(", ", parts) + "}";
    }



    /// <summary>
    /// Strips a single leading ADO.NET parameter-prefix character (<c>@ : ?</c>) so that
    /// <c>@name</c>, <c>:name</c>, <c>?name</c>, and <c>name</c> all normalize to <c>name</c>.
    /// </summary>
    private static string NormalizeParameterName(string name)
    {
        if (name.Length > 0 && (name[0] == '@' || name[0] == ':' || name[0] == '?'))
        {
            return name.Substring(1);
        }

        return name;
    }
}
