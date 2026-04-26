# Introduction

`Wolfgang.Extensions.Logging.Data` adds `ILogger` extension methods that produce a single structured log entry for the most common ADO.NET objects. It is designed for diagnostic logging of database calls in production code where connection strings and parameter values may contain secrets.

## What it does

| Method | Purpose |
|---|---|
| `LogDbConnection` | Logs a `DbConnection`'s state, server, database, and connection string (with password keys removed). |
| `LogDbCommand` | Logs a `DbCommand`'s text, type, timeout, and a snapshot of every parameter. |
| `LogDbParameter` | Logs a single `DbParameter`'s metadata and (optionally redacted) value. |
| `LogDbParameterCollection` | Logs the count and a snapshot of every parameter in a collection. |
| `LogCommandText` | Logs raw SQL plus an optional `IReadOnlyDictionary<string, object?>` of parameter values, for callers without a `DbCommand` in hand. |

## Design principles

- **One entry per call.** Each method emits exactly one structured log entry with named template fields, so sinks like Serilog, Seq, or Application Insights can index and filter on them.
- **Secure by default for connection strings.** `LogDbConnection` parses the connection string via `DbConnectionStringBuilder` and removes the `Password` and `Pwd` keys (case-insensitive). Username keys (`User ID`, `UID`, etc.) are preserved.
- **Opt-in redaction for parameters.** Methods that take parameters accept an `excludedParameterNames` list; values for matching parameters become the literal string `"[REDACTED]"` while name, `DbType`, direction, size, precision, scale, and nullability are preserved.
- **Tolerant matching.** Excluded-name matching is case-insensitive and ignores provider prefixes &mdash; `@password`, `:password`, `?password`, and `password` are all equivalent.
- **No I/O.** The methods are pure logging calls; they don't open connections, execute commands, or touch the database.

## When to use it

- Diagnostic logging of database operations in services, jobs, or background workers.
- Auditing what a command actually sent to the database, including its parameters, without leaking credentials or secrets.
- Reproducing customer-reported bugs by capturing the exact `DbCommand` shape that was executed.

## When *not* to use it

- High-throughput hot paths where every microsecond matters &mdash; consider a `LoggerMessage` source-generator approach for those (a future enhancement is tracked in [issue #3](https://github.com/Chris-Wolfgang/Extensions-Logging-Data/issues/3)).
- Replacing your provider's existing diagnostic source / `EventCounter` instrumentation. This library complements that, it doesn't replace it.

## Getting Help

- Read the [Getting Started](getting-started.md) guide
- Browse the [API Reference](xref:Wolfgang.Extensions.Logging.Data)
- File issues at [GitHub Issues](https://github.com/Chris-Wolfgang/Extensions-Logging-Data/issues)
