# Getting Started

This guide walks through installing `Wolfgang.Extensions.Logging.Data`, making your first call, and understanding the redaction model.

## Prerequisites

- An `ILogger` from `Microsoft.Extensions.Logging.Abstractions` (any provider works &mdash; Serilog, console, Seq, Application Insights, in-memory, etc.).
- A target framework of .NET Framework 4.6.2 or later, .NET Standard 2.0 or 2.1, or .NET 10.0 or later.

## Installation

### Via NuGet Package Manager

```bash
dotnet add package Wolfgang.Extensions.Logging.Data
```

### Via Package Manager Console

```powershell
Install-Package Wolfgang.Extensions.Logging.Data
```

## Your first call

```csharp
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using Wolfgang.Extensions.Logging.Data;

using var loggerFactory = LoggerFactory.Create(b => b.AddConsole());
ILogger logger = loggerFactory.CreateLogger("Demo");

await using var connection = new SqliteConnection("Data Source=:memory:;Password=topsecret");
await connection.OpenAsync();

logger.LogDbConnection(connection);
```

The output (with the default console formatter) looks something like:

```
info: Demo[0]
      DbConnection Microsoft.Data.Sqlite.SqliteConnection Database=main DataSource=:memory: ServerVersion=3.42.0 State=Open ConnectionTimeout=30 ConnectionString=Data Source=:memory:
```

Note that the password is gone but everything else &mdash; including the username, if present &mdash; is preserved.

## The five extension methods

### `LogDbConnection`

```csharp
logger.LogDbConnection(connection);
logger.LogDbConnection(connection, LogLevel.Debug);
```

Logs `ConnectionType`, `Database`, `DataSource`, `ServerVersion`, `State`, `ConnectionTimeout`, and the redacted `ConnectionString`. `ServerVersion` is read only when the connection is open; if the provider throws `InvalidOperationException` reading it, the field is logged as `null` rather than bubbling.

### `LogDbCommand`

```csharp
logger.LogDbCommand(command);
logger.LogDbCommand(command, excludedParameterNames: new[] { "@password" });
logger.LogDbCommand(command, excluded, LogLevel.Debug);
```

Logs `CommandType`, `CommandText`, `CommandTimeout`, and a snapshot of every parameter (name, `DbType`, direction, size, precision, scale, nullability, value). Names in `excludedParameterNames` cause the matching parameter's value to be replaced with `"[REDACTED]"`; all other metadata is preserved.

### `LogDbParameter`

```csharp
logger.LogDbParameter(parameter);
logger.LogDbParameter(parameter, redactValue: true);
```

Logs a single parameter's full metadata + value. The `redactValue` flag (defaulting to `false`) replaces the value with `"[REDACTED]"`.

### `LogDbParameterCollection`

```csharp
logger.LogDbParameterCollection(command.Parameters);
logger.LogDbParameterCollection(command.Parameters, new[] { "@password" });
```

Logs `Count` and a snapshot of every parameter. Same redaction rules as `LogDbCommand`.

### `LogCommandText`

For code paths that have raw SQL and a parameter dictionary but no `DbCommand`:

```csharp
logger.LogCommandText("SELECT * FROM Users WHERE Email=@email");

logger.LogCommandText(
    "UPDATE Users SET PasswordHash=@password WHERE Email=@email",
    new Dictionary<string, object?>
    {
        ["@email"] = "alice@example.com",
        ["@password"] = "new-hash"
    },
    excludedParameterNames: new[] { "@password" });
```

## The redaction model

- **Connection-string passwords are always redacted.** `LogDbConnection` removes `Password` and `Pwd` keys (case-insensitive) using `DbConnectionStringBuilder`.
- **Parameter-value redaction is opt-in.** Methods that touch parameters take an `excludedParameterNames` list; only the values for matching names are replaced. Name, `DbType`, direction, etc. are always preserved so logs remain useful.
- **Name matching is case-insensitive and prefix-tolerant.** `@password`, `:password`, `?password`, and `password` are all equivalent for matching purposes.
- **The marker is the literal string `"[REDACTED]"`.**

## Where to next

- Browse the [API Reference](xref:Wolfgang.Extensions.Logging.Data) for the full method signatures and XML docs.
- Look at the [example app](https://github.com/Chris-Wolfgang/Extensions-Logging-Data/tree/main/examples/Wolfgang.Extensions.Logging.Data.Example) for a working end-to-end demo against an in-memory SQLite database.
- File issues or feature requests at [GitHub Issues](https://github.com/Chris-Wolfgang/Extensions-Logging-Data/issues).
