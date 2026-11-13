# Introduction

Welcome to Wolfgang.Extensions.Logging.Data!

## Overview

Extension methods on `ILogger` and `ILogger<T>` for logging `DbConnection`, `DbCommand`, and other
`System.Data` types, with credential redaction built in so passwords never reach the log.

The package is intentionally narrow: each method writes one structured log entry with named properties,
so sinks such as Serilog, Application Insights or Elastic can index the values instead of parsing text.

## Key Features

- **`LogDbConnection`** - logs connection type, database, data source, server version, state, timeout and
  the connection string, with `Password` / `Pwd` keys removed first.
- **`LogDbCommand`** - logs a live `DbCommand`: its text and a snapshot of its parameters.
- **`LogCommandText`** - logs command text with parameters supplied as a dictionary or an anonymous object.
- **Opt-in parameter redaction** - pass the parameter names whose values must not be logged; matching is
  case-insensitive and tolerant of the `@` / `:` / `?` provider prefixes.
- **`IsEnabled` short-circuit** - no parsing, redaction or allocation happens when the level is disabled.
- **Trim and Native AOT safe** - everything except the anonymous-object `LogCommandText` overloads, which
  are marked `[RequiresUnreferencedCode]`.
- **Entity Framework 6 companion package** - `Wolfgang.Extensions.Logging.Data.EntityFramework6` logs every
  EF6 command, connection and transaction event after one registration call.

## Getting Help

If you need help with Wolfgang.Extensions.Logging.Data, please:

- Check the [Getting Started](getting-started.md) guide
- Review the [API Reference](../api/index.md)
- Visit the [GitHub repository](https://github.com/Chris-Wolfgang/Extensions-Logging-Data)
- Open an issue on [GitHub Issues](https://github.com/Chris-Wolfgang/Extensions-Logging-Data/issues)
