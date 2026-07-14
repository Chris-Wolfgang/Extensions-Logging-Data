# Wolfgang.Extensions.Logging.Data

Extension methods on `ILogger` for logging `DbConnection`, `DbCommand`, and other `System.Data` types — with credential redaction baked in so passwords never reach the log.

[![NuGet](https://img.shields.io/nuget/v/Wolfgang.Extensions.Logging.Data.svg?logo=nuget&label=NuGet)](https://www.nuget.org/packages/Wolfgang.Extensions.Logging.Data)
[![NuGet downloads](https://img.shields.io/nuget/dt/Wolfgang.Extensions.Logging.Data.svg?logo=nuget&label=downloads)](https://www.nuget.org/packages/Wolfgang.Extensions.Logging.Data)
[![PR build](https://img.shields.io/github/actions/workflow/status/Chris-Wolfgang/Extensions-Logging-Data/pr.yaml?event=pull_request_target&label=PR%20build&logo=github)](https://github.com/Chris-Wolfgang/Extensions-Logging-Data/actions/workflows/pr.yaml)
[![Release](https://img.shields.io/github/actions/workflow/status/Chris-Wolfgang/Extensions-Logging-Data/release.yaml?label=release&logo=github)](https://github.com/Chris-Wolfgang/Extensions-Logging-Data/actions/workflows/release.yaml)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)
[![.NET](https://img.shields.io/badge/.NET-Multi--Targeted-purple.svg)](https://dotnet.microsoft.com/)
[![GitHub](https://img.shields.io/badge/GitHub-Repository-181717?logo=github)](https://github.com/Chris-Wolfgang/Extensions-Logging-Data)
[![OpenSSF Scorecard](https://api.scorecard.dev/projects/github.com/Chris-Wolfgang/Extensions-Logging-Data/badge)](https://scorecard.dev/viewer/?uri=github.com/Chris-Wolfgang/Extensions-Logging-Data)

---

## 📦 Installation

```bash
dotnet add package Wolfgang.Extensions.Logging.Data
```

**NuGet Package:** [Wolfgang.Extensions.Logging.Data](https://www.nuget.org/packages/Wolfgang.Extensions.Logging.Data)

---

## 📄 License

This project is licensed under the **MIT License**. See the [LICENSE](LICENSE) file for details.

---

## 📚 Documentation

- **GitHub Repository:** [https://github.com/Chris-Wolfgang/Extensions-Logging-Data](https://github.com/Chris-Wolfgang/Extensions-Logging-Data)
- **API Documentation:** https://Chris-Wolfgang.github.io/Extensions-Logging-Data/
- **CHANGELOG:** [CHANGELOG.md](CHANGELOG.md)
- **Contributing Guide:** [CONTRIBUTING.md](CONTRIBUTING.md)
- **Architecture Decision Records:** [docs/adr/](docs/adr/README.md)
- **Reproducible builds (independent verification):** [docs/REPRODUCIBLE-BUILDS.md](docs/REPRODUCIBLE-BUILDS.md)
- **Migration guides:** [docs/migrations/](docs/migrations/README.md)
- **Disaster recovery runbook:** [docs/DISASTER-RECOVERY.md](docs/DISASTER-RECOVERY.md)
- **DocFX Version Picker Troubleshooting:** [docs/DOCFX-VERSION-PICKER.md](docs/DOCFX-VERSION-PICKER.md)

---

## 🚀 Quick Start

```csharp
using System.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Wolfgang.Extensions.Logging.Data;

ILogger logger = /* … your DI-resolved ILogger … */;

using var connection = new SqlConnection(connectionString);
await connection.OpenAsync();

// Logs one structured entry: ConnectionType, Database, DataSource,
// ServerVersion, State, ConnectionTimeout, ConnectionString — with
// Password / Pwd keys removed before the entry is written.
logger.LogDbConnection(connection);
```

Want it at a different level than the default `Information`? Pass one:

```csharp
logger.LogDbConnection(connection, LogLevel.Debug);
```

---

## ✨ Features

| Feature | Method |
|---|---|
| Log a `DbConnection` at `LogLevel.Information` (default) | `ILogger.LogDbConnection(DbConnection)` |
| Log a `DbConnection` at a specific `LogLevel` | `ILogger.LogDbConnection(DbConnection, LogLevel)` |
| **Credential redaction** — `Password` / `Pwd` keys are stripped from the connection string before it's emitted | (built-in to every overload) |
| **Structured logging** — emits as templated log entry with named properties (`{ConnectionType}`, `{Database}`, `{DataSource}`, etc.) for structured-log sinks like Serilog, Application Insights, or Elastic | (built-in) |
| **`IsEnabled(level)` short-circuit** — work is skipped if the log level isn't enabled, so the redaction / `DbConnectionStringBuilder` parse only runs when the entry will actually be written | (built-in) |
| **Null-safe** — throws `ArgumentNullException` (not `NullReferenceException`) when `logger` or `connection` is `null` | (every overload) |

`DbCommand` logging is also available via `ILogger.LogCommandText(...)` (command text + parameter snapshot with opt-in redaction) and `ILogger.LogDbCommand(DbCommand, ...)` (logs a live command directly). The library is intentionally narrow and grows one type at a time as needed.

> **Trimming / Native AOT:** `LogDbConnection`, `LogDbCommand`, and the `IReadOnlyDictionary<string, object?>` overloads of `LogCommandText` are trim- and AOT-safe (verified by a `PublishAot` smoke in CI). The **anonymous-object** `LogCommandText(..., object parameters, ...)` overloads reflect over the parameter's runtime properties and are marked `[RequiresUnreferencedCode]` — in trimmed/AOT apps use the dictionary overload instead.

---

## 🪵 Entity Framework 6 companion

The optional **`Wolfgang.Extensions.Logging.Data.EntityFramework6`** package wires these extension methods into the EF6 interceptor pipeline so a legacy EF6 app logs *every* command, connection, and transaction event with **no per-query call sites**. Register once at startup:

```csharp
// At application startup, before any DbContext is used:
EntityFramework6Logging.AddLoggingInterceptors(
    logger,
    excludedParameterNames: new[] { "password" },   // optional — redacted in logged commands
    level: LogLevel.Information);                    // optional — defaults to Information
```

From then on, every `DbContext` in the process logs:

- **Commands** — text + parameter snapshot (redacted), via `LogDbCommand`.
- **Connections** — open/close lifecycle, with connection-string redaction, via `LogDbConnection`.
- **Transactions** — commit / rollback.
- **Failures** — surfaced at `LogLevel.Error` from the EF6 interception context.

`DbInterception.Add` is process-global and not idempotent, so `AddLoggingInterceptors` is a no-op if called again with the same `logger` instance. Targets `net462; net48; netstandard2.1`.

```bash
dotnet add package Wolfgang.Extensions.Logging.Data.EntityFramework6
```

---

## 🎯 Target Frameworks

| Family | Targets |
|---|---|
| .NET Framework | `net462` |
| .NET Standard | `netstandard2.0`, `netstandard2.1` |
| Modern .NET | `net10.0` |

`net462` is included so the library is usable from .NET Framework 4.6.2+ application hosts (existing `System.Data.SqlClient` consumers, classic web apps).

---

## 🔍 Code Quality & Static Analysis

This project enforces strict code quality standards through **7 specialized analyzers** and async-first custom rules:

1. **Microsoft.CodeAnalysis.NetAnalyzers** — built-in .NET analyzers for correctness and performance
2. **Roslynator.Analyzers** — advanced refactoring and code quality rules
3. **AsyncFixer** — async/await best practices and anti-pattern detection
4. **Microsoft.VisualStudio.Threading.Analyzers** — thread safety and async patterns
5. **Microsoft.CodeAnalysis.BannedApiAnalyzers** — prevents usage of banned synchronous APIs
6. **Meziantou.Analyzer** — comprehensive code quality rules
7. **SonarAnalyzer.CSharp** — industry-standard code analysis

### Async-First Enforcement

The repo uses **`BannedSymbols.txt`** to prohibit blocking APIs:

- ❌ `Task.Wait()`, `Task.Result` — use `await`
- ❌ `Thread.Sleep()` — use `await Task.Delay()`
- ❌ Synchronous file I/O (`File.ReadAllText`) — use async equivalents
- ❌ Synchronous stream operations — use `ReadAsync()` / `WriteAsync()`
- ❌ `Parallel.For` / `Parallel.ForEach` — use `Task.WhenAll()` or `Parallel.ForEachAsync()`
- ❌ Obsolete APIs (`WebClient`, `BinaryFormatter`)

---

## 🛠️ Building from Source

### Prerequisites

- .NET 10.0 SDK (also need .NET 8 / 9 SDKs to run the full multi-targeting test matrix)
- Optional: [PowerShell Core](https://github.com/PowerShell/PowerShell) for the formatting / build helper scripts

### Build steps

```bash
# Clone
git clone https://github.com/Chris-Wolfgang/Extensions-Logging-Data.git
cd Extensions-Logging-Data

# Restore + build + test
dotnet restore
dotnet build --configuration Release
dotnet test  --configuration Release

# Format (PowerShell Core)
pwsh ./scripts/format.ps1
```

### Building documentation locally

```bash
# Install DocFX once
dotnet tool update -g docfx

# Build + serve
cd docfx_project
docfx metadata
docfx build --serve   # http://localhost:8080
```

---

## 🤝 Contributing

Contributions are welcome — see [CONTRIBUTING.md](CONTRIBUTING.md) for code quality standards, build and test instructions, pull request guidelines, and analyzer configuration details.

---

## 🙏 Acknowledgments

- **[Microsoft.Extensions.Logging](https://learn.microsoft.com/dotnet/core/extensions/logging)** — the `ILogger` abstraction this library extends
- **[System.Data.Common.DbConnectionStringBuilder](https://learn.microsoft.com/dotnet/api/system.data.common.dbconnectionstringbuilder)** — the parser the redaction logic is built around
- The broader Wolfgang.Extensions.* family for the canonical CI / docs / packaging patterns this repo inherits
