# Getting Started

This guide will help you quickly get up and running with Wolfgang.Extensions.Logging.Data.

## Prerequisites

- An application that logs through `Microsoft.Extensions.Logging` (any `ILogger` or `ILogger<T>`).
- A target framework the package supports: .NET Framework 4.6.2+, .NET Standard 2.0 / 2.1 hosts, or .NET 10.
- Any ADO.NET provider (`System.Data.SqlClient`, `Microsoft.Data.SqlClient`, `Microsoft.Data.Sqlite`, Npgsql, ...).
  The methods take the provider-neutral `DbConnection` and `DbCommand` base types.

## Installation

### Via NuGet Package Manager

```bash
dotnet add package Wolfgang.Extensions.Logging.Data
```

### Via Package Manager Console

```powershell
Install-Package Wolfgang.Extensions.Logging.Data
```

## Quick Start

```csharp
using System.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Wolfgang.Extensions.Logging.Data;

ILogger logger = /* ... your DI-resolved ILogger ... */;

using var connection = new SqlConnection(connectionString);
await connection.OpenAsync();

// Logs one structured entry: ConnectionType, Database, DataSource,
// ServerVersion, State, ConnectionTimeout, ConnectionString - with
// Password / Pwd keys removed before the entry is written.
logger.LogDbConnection(connection);

// A different level than the default Information:
logger.LogDbConnection(connection, LogLevel.Debug);
```

Log a command and its parameters, redacting the ones you name:

```csharp
using var command = connection.CreateCommand();
command.CommandText = "UPDATE Users SET PasswordHash = @hash WHERE Id = @id";
// ... add @hash and @id parameters ...

logger.LogDbCommand(command, new[] { "hash" });
```

Excluded names are matched case-insensitively and with or without the provider prefix (`@`, `:`, `?`).

### Entity Framework 6

The optional `Wolfgang.Extensions.Logging.Data.EntityFramework6` package logs every EF6 command,
connection and transaction event with no per-query call sites. Register once at startup:

```csharp
EntityFramework6Logging.AddLoggingInterceptors(
    logger,
    excludedParameterNames: new[] { "password" },
    level: LogLevel.Information);
```

## Next Steps

- Explore the [API Reference](../api/index.md) for detailed documentation
- Read the [Introduction](introduction.md) to learn more about Wolfgang.Extensions.Logging.Data
- Check out example projects in the [GitHub repository](https://github.com/Chris-Wolfgang/Extensions-Logging-Data)

## Common Issues

- **Nothing is logged.** Every method checks `logger.IsEnabled(level)` first and does no work when the
  level is disabled. Check the minimum level configured for the logger's category.
- **The anonymous-object `LogCommandText` overload warns in a trimmed or Native AOT app.** It reflects over
  the object's properties and is marked `[RequiresUnreferencedCode]`. Pass an
  `IReadOnlyDictionary<string, object?>` instead, or use `LogDbCommand`.
- **EF6 entries appear twice.** `DbInterception.Add` is process-global. Calling `AddLoggingInterceptors`
  again with the same logger instance is a no-op, but registering two different loggers logs to both.

## Additional Resources

- [GitHub Repository](https://github.com/Chris-Wolfgang/Extensions-Logging-Data)
- [Contributing Guidelines](https://github.com/Chris-Wolfgang/Extensions-Logging-Data/blob/main/CONTRIBUTING.md)
- [Report an Issue](https://github.com/Chris-Wolfgang/Extensions-Logging-Data/issues)
