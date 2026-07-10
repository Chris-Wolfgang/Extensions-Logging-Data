# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added

### Changed

### Deprecated

### Removed

### Fixed

### Security

## [0.2.0] - 2026-07-06

### Added

- New companion package **`Wolfgang.Extensions.Logging.Data.EntityFramework6`** for classic EF6 integration (#66).
- `LogDbCommand(DbCommand)` core overloads on the primary `Wolfgang.Extensions.Logging.Data` package (#66).
- `LogCommandText` with Dapper-style anonymous-object parameter overloads (closes #3).

### Changed

- Normalized `PublicAPI.Shipped.txt` to the canonical nullable-annotated format used across the fleet.
## [0.1.1] - 2026-06-12

Canonical maintenance round + binding-stability fix. No public API or
runtime behavior change vs v0.1.0.

### Added

- **D8** — docs-site version-picker dropdown + purple-W logo/favicon,
  with the picker bootstrap wired into `docfx.json` so the dropdown
  renders on the published site; `verify-docs-build` job in
  `release.yaml` runs DocFX before the NuGet push so a docs build
  failure blocks the package from shipping.
- **A1** — `PublicApiAnalyzers` with a baselined public surface
  (`PublicAPI.Shipped.txt`), so breaking-change detection is active
  from this release forward.
- **CI3** — canonical NuGet package metadata: `Authors`, `Copyright`,
  `RepositoryType`, SourceLink, snupkg symbol packages, deterministic
  CI build flag, and `EmbedUntrackedSources` hoisted to
  `Directory.Build.props`.
- **T1** — coverage report published to the docs site.
- **T3** — Stryker mutation-testing workflow (`stryker.yaml`).
- **S1** — CodeQL `security-extended` query pack.
- **D6** — `versions.json` preservation guard on the docs deploy.
- **P1/P2** — BenchmarkDotNet baseline project (`LogDbConnection`
  fast-path / full-work / explicit-level scenarios) with a
  `benchmarks.yaml` workflow that publishes results to the gh-pages
  chart.
- An integration test suite exercising `LogDbConnection` against a
  real `Microsoft.Data.Sqlite` connection and a real
  `Microsoft.Extensions.Logging` pipeline, plus globalization
  (tr-TR/de-DE/zh-CN/ar-SA/ja-JP) invariance and allocation-free
  hot-path verification tests.

### Changed

- **C1** — fleet-wide template-drift sync: workflow files (`pr.yaml`,
  `release.yaml`, `docfx.yaml`, `codeql.yaml`,
  `build-all-versions.yaml`, `stryker.yaml`), `.editorconfig`,
  `BannedSymbols.txt`, `Directory.Build.props`, and per-context
  `tests/Directory.Build.props` consolidated to the canonical baseline.
- **Nullable** — `<Nullable>enable</Nullable>` consolidated into
  `Directory.Build.props` (was per-csproj).
- **CI2** — Dependabot `github-actions` ecosystem added; the
  `dotnet-dependencies` group bumped `Microsoft.Extensions.Logging`/
  `.Abstractions` to 10.0.9 across all projects.
- **README** — rewritten for accuracy: corrected package id
  (`Wolfgang.Extensions.Logging.Data`, dotted), canonical badge
  collection, accurate Quick Start + Features + Target Frameworks
  matching the actual public surface.

### Fixed

- **C4** — pinned explicit `<AssemblyVersion>1.0.0.0</AssemblyVersion>`
  and a prerelease-safe `<FileVersion>` to the src csproj. Without an
  explicit pin, SDK-derived `AssemblyVersion` would change on every
  minor/patch release, breaking `net462` .NET Framework consumers
  without a binding redirect.

## [0.1.0] - 2026-05-02

Initial release.

### Added

- `ILogger.LogDbConnection(DbConnection)` — logs a single structured
  entry describing a `DbConnection` at `LogLevel.Information`.
- `ILogger.LogDbConnection(DbConnection, LogLevel)` — same, at a
  caller-specified level.
- Built-in credential redaction: the `Password` / `Pwd` keys are
  stripped from the logged connection string (case-insensitive) so
  credentials are never written to the log; all other keys, including
  the user name, are preserved.
- Structured-logging output (`ConnectionType`, `Database`,
  `DataSource`, `ServerVersion`, `State`, `ConnectionTimeout`,
  `ConnectionString`) and an `IsEnabled` short-circuit so the
  redaction work is skipped when the log level isn't enabled.
- Multi-targeting: `net462`, `netstandard2.0`, `netstandard2.1`,
  `net10.0`.

[Unreleased]: https://github.com/Chris-Wolfgang/Extensions-Logging-Data/compare/v0.2.0...HEAD
[0.2.0]: https://github.com/Chris-Wolfgang/Extensions-Logging-Data/compare/v0.1.1...v0.2.0
[0.1.1]: https://github.com/Chris-Wolfgang/Extensions-Logging-Data/compare/v0.1.0...v0.1.1
[0.1.0]: https://github.com/Chris-Wolfgang/Extensions-Logging-Data/releases/tag/v0.1.0
