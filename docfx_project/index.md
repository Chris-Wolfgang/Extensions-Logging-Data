---
_layout: landing
---

# Wolfgang.Extensions.Logging.Data

Extension methods for `ILogger` and `ILogger<T>` that log `DbConnection`, `DbCommand`, `DbParameter`, `DbParameterCollection`, and raw SQL command text from `System.Data.Common` &mdash; with built-in secret redaction.

## Quick Links

- [Introduction](docs/introduction.md) &mdash; what the library does and why
- [Getting Started](docs/getting-started.md) &mdash; install, first call, and the redaction model
- [API Reference](xref:Wolfgang.Extensions.Logging.Data) &mdash; full type and method documentation
- [GitHub Repository](https://github.com/Chris-Wolfgang/Extensions-Logging-Data)

## Installation

```bash
dotnet add package Wolfgang.Extensions.Logging.Data
```

## At a glance

```csharp
using Microsoft.Extensions.Logging;
using Wolfgang.Extensions.Logging.Data;

logger.LogDbConnection(connection);
logger.LogDbCommand(command, excludedParameterNames: new[] { "@password" });
```

Each call emits a single structured log entry with named template fields (`ConnectionType`, `Database`, `CommandText`, `Parameters`, &hellip;) so sinks like Serilog, Seq, or Application Insights can index and filter on them. Connection-string passwords are removed automatically; parameter values are redacted by name on an opt-in basis.

## Documentation Sections

### 📖 [Documentation](docs/getting-started.md)
Step-by-step guides covering installation, the five extension methods, and the redaction model.

### 📚 [API Reference](xref:Wolfgang.Extensions.Logging.Data)
Complete API documentation generated from XML doc comments in the source.

## Additional Resources

- [Contributing Guidelines](https://github.com/Chris-Wolfgang/Extensions-Logging-Data/blob/main/CONTRIBUTING.md)
- [Code of Conduct](https://github.com/Chris-Wolfgang/Extensions-Logging-Data/blob/main/CODE_OF_CONDUCT.md)
- [License](https://github.com/Chris-Wolfgang/Extensions-Logging-Data/blob/main/LICENSE)

---

*Documentation built with [DocFX](https://dotnet.github.io/docfx/)*
