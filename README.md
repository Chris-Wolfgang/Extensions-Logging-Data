# Wolfgang.Extensions.Logging.Data

Extension methods for `ILogger` and `ILogger<T>` that log `DbConnection`, `DbCommand`, `DbParameter`, `DbParameterCollection`, and raw SQL command text from `System.Data.Common` &mdash; with built-in secret redaction (passwords in connection strings, configurable parameter values).

[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)
[![.NET](https://img.shields.io/badge/.NET-Multi--Targeted-purple.svg)](https://dotnet.microsoft.com/)
[![GitHub](https://img.shields.io/badge/GitHub-Repository-181717?logo=github)](https://github.com/Chris-Wolfgang/Extensions-Logging-Data)

---

## 📦 Installation

```bash
dotnet add package Wolfgang.Extensions.Logging.Data
```

**NuGet Package:** Coming soon to NuGet.org

---

## 📄 License

This project is licensed under the **MIT License**. See the [LICENSE](LICENSE) file for details.

---

## 📚 Documentation

- **GitHub Repository:** [https://github.com/Chris-Wolfgang/Extensions-Logging-Data](https://github.com/Chris-Wolfgang/Extensions-Logging-Data)
- **API Documentation:** https://Chris-Wolfgang.github.io/Extensions-Logging-Data/
- **Formatting Guide:** [README-FORMATTING.md](README-FORMATTING.md)
- **Contributing Guide:** [CONTRIBUTING.md](CONTRIBUTING.md)

---

## 🚀 Quick Start

```csharp
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using Wolfgang.Extensions.Logging.Data;

using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
ILogger logger = loggerFactory.CreateLogger("Demo");

await using var connection = new SqliteConnection("Data Source=:memory:;User=alice;Password=topsecret");
await connection.OpenAsync();

// Logs ConnectionType, Database, DataSource, ServerVersion, State,
// ConnectionTimeout, and the connection string with Password removed.
logger.LogDbConnection(connection);

await using var command = connection.CreateCommand();
command.CommandText = "INSERT INTO Users (Email, PasswordHash) VALUES (@email, @password)";
command.Parameters.AddWithValue("@email", "alice@example.com");
command.Parameters.AddWithValue("@password", "hashed-secret");

// Logs the command and every parameter; the @password value is replaced
// with "[REDACTED]" while name, DbType, direction, etc. are preserved.
logger.LogDbCommand(command, excludedParameterNames: new[] { "@password" });
```

A full runnable example lives in [examples/Wolfgang.Extensions.Logging.Data.Example](examples/Wolfgang.Extensions.Logging.Data.Example).

---

## ✨ Features

| Method | What it captures | Redaction |
|---|---|---|
| `LogDbConnection` | `ConnectionType`, `Database`, `DataSource`, `ServerVersion` (when open), `State`, `ConnectionTimeout`, `ConnectionString` | `Password` and `Pwd` keys are removed from the connection string (case-insensitive). User name (`User ID`, `UID`, etc.) is preserved. |
| `LogDbCommand` | `CommandType`, `CommandText`, `CommandTimeout`, snapshot of every parameter (name, `DbType`, direction, size, precision, scale, nullability, value) | Pass `excludedParameterNames`; values for matching parameters become `"[REDACTED]"`. |
| `LogDbParameter` | A single parameter's full metadata + value | Pass `redactValue: true` to replace the value with `"[REDACTED]"`. |
| `LogDbParameterCollection` | `Count` and a snapshot of every parameter | Pass `excludedParameterNames`; values for matching parameters become `"[REDACTED]"`. |
| `LogCommandText` | Raw SQL `CommandText` and an optional `IReadOnlyDictionary<string, object?>` of parameter values | Pass `excludedParameterNames`; values for matching keys become `"[REDACTED]"`. |

**Parameter name matching for `excludedParameterNames`** is **case-insensitive** and **tolerant of provider prefixes** &mdash; `@password`, `:password`, `?password`, and `password` all match a parameter named `@password`. Each method emits **one structured log entry per call** so sinks like Serilog, Seq, or Application Insights can index/filter on the named template fields.

---

## 🎯 Target Frameworks

The library targets a broad range of TFMs so it can be consumed from .NET Framework, .NET Standard, and modern .NET projects alike.

| Framework | Versions |
|-----------|----------|
| .NET Framework | 4.6.2 and later |
| .NET Standard | 2.0, 2.1 |
| .NET | 10.0 |

---

## 🔍 Code Quality & Static Analysis

This project enforces **strict code quality standards** through **7 specialized analyzers** and custom async-first rules:

### Analyzers in Use

1. **Microsoft.CodeAnalysis.NetAnalyzers** - Built-in .NET analyzers for correctness and performance
2. **Roslynator.Analyzers** - Advanced refactoring and code quality rules
3. **AsyncFixer** - Async/await best practices and anti-pattern detection
4. **Microsoft.VisualStudio.Threading.Analyzers** - Thread safety and async patterns
5. **Microsoft.CodeAnalysis.BannedApiAnalyzers** - Prevents usage of banned synchronous APIs
6. **Meziantou.Analyzer** - Comprehensive code quality rules
7. **SonarAnalyzer.CSharp** - Industry-standard code analysis

### Async-First Enforcement

This library uses **`BannedSymbols.txt`** to prohibit synchronous APIs and enforce async-first patterns:

**Blocked APIs Include:**
- ❌ `Task.Wait()`, `Task.Result` - Use `await` instead
- ❌ `Thread.Sleep()` - Use `await Task.Delay()` instead
- ❌ Synchronous file I/O (`File.ReadAllText`) - Use async versions
- ❌ Synchronous stream operations - Use `ReadAsync()`, `WriteAsync()`
- ❌ `Parallel.For/ForEach` - Use `Task.WhenAll()` or `Parallel.ForEachAsync()`
- ❌ Obsolete APIs (`WebClient`, `BinaryFormatter`)

**Why?** To ensure all code is **truly async** and **non-blocking** for optimal performance in async contexts.

---

## 🛠️ Building from Source

### Prerequisites
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download) or later
- Optional: [PowerShell Core](https://github.com/PowerShell/PowerShell) for formatting scripts

### Build Steps

```bash
# Clone the repository
git clone https://github.com/Chris-Wolfgang/Extensions-Logging-Data.git
cd Extensions-Logging-Data

# Restore dependencies
dotnet restore

# Build the solution
dotnet build --configuration Release

# Run tests
dotnet test --configuration Release

# Run code formatting (PowerShell Core)
pwsh ./format.ps1
```

### Code Formatting

This project uses `.editorconfig` and `dotnet format`:

```bash
# Format code
dotnet format

# Verify formatting (as CI does)
dotnet format --verify-no-changes
```

See [README-FORMATTING.md](README-FORMATTING.md) for detailed formatting guidelines.

### Building Documentation

This project uses [DocFX](https://dotnet.github.io/docfx/) to generate API documentation:

```bash
# Install DocFX (one-time setup)
dotnet tool install -g docfx

# Generate API metadata and build documentation
cd docfx_project
docfx metadata  # Extract API metadata from source code
docfx build     # Build HTML documentation

# Documentation is generated in the docs/ folder at the repository root
```

The documentation is automatically built and deployed to GitHub Pages when changes are pushed to the `main` branch.

**Local Preview:**
```bash
# Serve documentation locally (with live reload)
cd docfx_project
docfx build --serve

# Open http://localhost:8080 in your browser
```

**Documentation Structure:**
- `docfx_project/` - DocFX configuration and source files
- `docs/` - Generated HTML documentation (published to GitHub Pages)
- `docfx_project/index.md` - Main landing page content
- `docfx_project/docs/` - Additional documentation articles
- `docfx_project/api/` - Auto-generated API reference YAML files

---

## 🤝 Contributing

Contributions are welcome! Please see [CONTRIBUTING.md](CONTRIBUTING.md) for:
- Code quality standards
- Build and test instructions
- Pull request guidelines
- Analyzer configuration details

---


## 🙏 Acknowledgments

- Built on top of [Microsoft.Extensions.Logging.Abstractions](https://www.nuget.org/packages/Microsoft.Extensions.Logging.Abstractions/) and `System.Data.Common`.
- Maintained by [Chris Wolfgang](https://github.com/Chris-Wolfgang).

