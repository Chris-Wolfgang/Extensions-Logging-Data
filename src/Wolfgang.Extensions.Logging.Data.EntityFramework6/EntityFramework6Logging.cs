using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure.Interception;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;

namespace Wolfgang.Extensions.Logging.Data.EntityFramework6;

/// <summary>
/// One-call registration of passive EF6 logging. Calling <c>AddLoggingInterceptors</c>
/// once at startup wires command, connection, and transaction interceptors into the
/// process-global EF6 interception pipeline, so every <c>DbContext</c> in the process
/// logs its activity with no per-query call sites.
/// </summary>
/// <remarks>
/// <para>
/// <see cref="DbInterception.Add"/> is process-global and not idempotent — adding the same
/// interceptor twice doubles every log entry. To make the common "call it once per logger"
/// pattern safe, this type keeps a static record of the loggers it has already registered
/// and treats a repeat registration of the same logger instance as a no-op.
/// </para>
/// <para>
/// EF6 offers no way to remove a registration through this type, so every registered logger is
/// retained (and keeps logging) for the lifetime of the process. Register long-lived (singleton)
/// loggers only; do not register a per-request or per-scope logger.
/// </para>
/// </remarks>
public static class EntityFramework6Logging
{
    private static readonly object Gate = new object();

    // Identity comparison: the same logger instance must not be registered twice, but two
    // distinct loggers (e.g. different categories) are legitimately allowed to both log.
    private static readonly HashSet<ILogger> RegisteredLoggers =
        new HashSet<ILogger>(ReferenceEqualityComparer.Instance);

    /// <summary>
    /// Registers command, connection, and transaction logging interceptors for the given
    /// <paramref name="logger"/> into the EF6 interception pipeline, logging every non-error
    /// event at <c>LogLevel.Information</c> with no parameter redaction. Safe to call more than
    /// once with the same <paramref name="logger"/> instance — repeat calls are a no-op.
    /// </summary>
    /// <param name="logger">The logger every EF6 event is written to.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="logger"/> is <see langword="null"/>.</exception>
    /// <example>
    /// <code>
    /// EntityFramework6Logging.AddLoggingInterceptors(logger);
    /// </code>
    /// </example>
    public static void AddLoggingInterceptors(ILogger logger)
    {
        AddLoggingInterceptors(logger, excludedParameterNames: null, LogLevel.Information, DbInterception.Add);
    }



    /// <summary>
    /// Registers command, connection, and transaction logging interceptors for the given
    /// <paramref name="logger"/> into the EF6 interception pipeline, logging every non-error
    /// event at <paramref name="level"/> with no parameter redaction. Safe to call more than
    /// once with the same <paramref name="logger"/> instance — repeat calls are a no-op.
    /// </summary>
    /// <param name="logger">The logger every EF6 event is written to.</param>
    /// <param name="level">The level every non-error event is logged at.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="logger"/> is <see langword="null"/>.</exception>
    /// <example>
    /// <code>
    /// EntityFramework6Logging.AddLoggingInterceptors(logger, LogLevel.Debug);
    /// </code>
    /// </example>
    public static void AddLoggingInterceptors(ILogger logger, LogLevel level)
    {
        AddLoggingInterceptors(logger, excludedParameterNames: null, level, DbInterception.Add);
    }



    /// <summary>
    /// Registers command, connection, and transaction logging interceptors for the given
    /// <paramref name="logger"/> into the EF6 interception pipeline, logging every non-error
    /// event at <c>LogLevel.Information</c> and redacting the values of
    /// <paramref name="excludedParameterNames"/>. Safe to call more than once with the same
    /// <paramref name="logger"/> instance — repeat calls are a no-op.
    /// </summary>
    /// <param name="logger">The logger every EF6 event is written to.</param>
    /// <param name="excludedParameterNames">
    /// Parameter names whose values are redacted in logged commands. Matching is
    /// case-insensitive and ADO.NET-prefix-tolerant.
    /// </param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="logger"/> is <see langword="null"/>.</exception>
    /// <example>
    /// <code>
    /// EntityFramework6Logging.AddLoggingInterceptors(logger, new[] { "password" });
    /// </code>
    /// </example>
    public static void AddLoggingInterceptors(ILogger logger, IEnumerable<string> excludedParameterNames)
    {
        AddLoggingInterceptors(logger, excludedParameterNames, LogLevel.Information, DbInterception.Add);
    }



    /// <summary>
    /// Registers command, connection, and transaction logging interceptors for the given
    /// <paramref name="logger"/> into the EF6 interception pipeline. Safe to call more than
    /// once with the same <paramref name="logger"/> instance — repeat calls are a no-op.
    /// </summary>
    /// <param name="logger">The logger every EF6 event is written to.</param>
    /// <param name="excludedParameterNames">
    /// Optional parameter names whose values are redacted in logged commands. Matching is
    /// case-insensitive and ADO.NET-prefix-tolerant. When <see langword="null"/>, nothing is
    /// redacted.
    /// </param>
    /// <param name="level">The level every non-error event is logged at. Defaults to <c>LogLevel.Information</c>.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="logger"/> is <see langword="null"/>.</exception>
    /// <remarks>
    /// This overload's default values are compiled into each call site. The overloads with no
    /// optional parameters cover every shape that omits an argument, so such a call binds to
    /// one of them instead and no default value is compiled into it.
    /// </remarks>
    public static void AddLoggingInterceptors(ILogger logger, IEnumerable<string>? excludedParameterNames = null, LogLevel level = LogLevel.Information)
    {
        AddLoggingInterceptors(logger, excludedParameterNames, level, DbInterception.Add);
    }



    /// <summary>
    /// Registration seam used by the public overload and by tests. The <paramref name="register"/>
    /// delegate is <see cref="DbInterception.Add"/> in production; tests supply a recording
    /// delegate so they can assert what was registered without mutating EF6's process-global
    /// interception state.
    /// </summary>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="logger"/> is <see langword="null"/>.</exception>
    internal static void AddLoggingInterceptors(ILogger logger, IEnumerable<string>? excludedParameterNames, LogLevel level, Action<IDbInterceptor> register)
    {
        if (logger is null)
        {
            throw new ArgumentNullException(nameof(logger));
        }

        var excluded = excludedParameterNames ?? Array.Empty<string>();

        lock (Gate)
        {
            if (!RegisteredLoggers.Add(logger))
            {
                return;
            }

            register(new LoggingCommandInterceptor(logger, excluded, level));
            register(new LoggingConnectionInterceptor(logger, level));
            register(new LoggingTransactionInterceptor(logger, level));
        }
    }



    /// <summary>
    /// Reference-identity comparer so the registration set distinguishes logger instances by
    /// identity rather than by any overridden <see cref="object.Equals(object)"/>.
    /// </summary>
    private sealed class ReferenceEqualityComparer : IEqualityComparer<ILogger>
    {
        internal static readonly ReferenceEqualityComparer Instance = new ReferenceEqualityComparer();



        public bool Equals(ILogger? x, ILogger? y) => ReferenceEquals(x, y);



        public int GetHashCode(ILogger obj) => RuntimeHelpers.GetHashCode(obj);
    }
}
