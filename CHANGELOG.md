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

## [0.3.1] - 2026-08-21

Both packages get this release. `Wolfgang.Extensions.Logging.Data` moves to
`0.3.1` and `Wolfgang.Extensions.Logging.Data.EntityFramework6` moves to
`0.2.1`. No public API changes on either package — this is a
maintenance-only patch that raises the `Microsoft.Extensions.Logging.Abstractions`
floor, adds a build-time ABI gate, and hardens the release pipeline.

### Added

- **PackageValidation gate on both packages.** `dotnet pack` now runs
  `Microsoft.DotNet.ApiCompat` against the last-published NuGet version
  (`0.3.0` for `Wolfgang.Extensions.Logging.Data`, `0.2.0` for
  `Wolfgang.Extensions.Logging.Data.EntityFramework6`). Accidental ABI
  breaks — removed / renamed / type-changed public members, dropped
  target frameworks — now fail `dotnet pack` instead of shipping. The
  `release.yaml` pack-and-validate job exercises this at release time,
  and `dotnet pack -c Release` reproduces it locally. (#175)

- **Consumer supply-chain verification docs.** `SECURITY.md` now includes
  a *Supply-Chain Verification* section covering how to inspect the
  CycloneDX SBOM (`*.bom.json`) attached to every release and how to
  verify each `.nupkg` against its SLSA build-provenance attestation
  with `gh attestation verify`. The mechanics were already in place;
  the docs close the loop by making the verification path
  copy-pasteable. Package signing via a code-signing certificate stays
  deferred (tracked in #181). (#90)

- **SourceLink verification workflow.** New `.github/workflows/sourcelink.yaml`
  builds every `net10.0`-targeted `src/` project with
  `ContinuousIntegrationBuild=true`, then runs `dotnet sourcelink test`
  on every produced PDB. Any regression that would break "step into
  library source" (F11) in a debugger — mis-mapped commit SHA, unreachable
  raw-URL, checksum drift — now fails the PR before the package ships.
  Projects without a `net10.0` target (the `EntityFramework6` adapter's
  `net462;net48;netstandard2.1` triad) are skipped; their PDBs ship in
  their own package and are covered by that package's SourceLink. (#171)

- **`SECURITY.md` release-path & compromise-scope appendix.** A concise
  runbook a maintainer would need at 2am if the release identity is
  compromised: the OIDC trust boundary (Trusted Publishing has no
  long-lived NuGet API key to rotate), the two-package coordinates for
  unlisting on nuget.org, and the ownership/downstream-consumer facts.
  Generic incident-response steps (rotating credentials, publishing
  advisories) intentionally aren't duplicated here — GitHub's and
  NuGet's own docs update faster than a checked-in runbook. (#102)

### Changed

- **`Microsoft.Extensions.Logging.Abstractions` floor bumped to 10.0.11**
  in both shipped packages, via Dependabot's grouped `dotnet-dependencies`
  updates. Consumers pinning lockfiles will see the floor move; API-level
  behavior is unchanged. (#183, #186)

- **InspectCode noise floor driven to zero on main** via targeted fixes
  (`InvalidXmlDocComment` in an integration test, `CheckNamespace` scoped
  to the `RequiresUnreferencedCode` polyfill) plus `.DotSettings`
  suppressions for defensive-null-check inspections that are intentional
  in a library called from nullable-oblivious / older-TFM code, and for
  the `MA0009` double-report. Result: `PR Checks v3 (Gated)` reports 0
  InspectCode findings on a clean build. (#136)

- **README `Supported Frameworks` section standardized** to the
  fleet-canonical badge collection and target-framework matrix.
  Consumers scanning the README can now match this repo's TFM story
  against the same shape used across the `Wolfgang.*` family. (#174)

- **Redundant `Microsoft.Extensions.Logging.Abstractions`
  `PackageReference` removed from `Tests.Unit`.** The pin was already
  provided transitively via the `ProjectReference` to the src project;
  the explicit duplicate caused an NU1605 downgrade every time src's
  floor bumped (seen on #183 and #186, both hand-fixed post-merge).
  Test-project-only change — nothing shipped changes.

### Security

- **Migrated NuGet publish to Trusted Publishing (OIDC).** The
  `release.yaml` workflow no longer relies on a long-lived NuGet API
  key stored in GitHub secrets; instead `NuGet/login@v1` mints an
  ephemeral push token per run via the workflow's OIDC identity
  (`Chris-Wolfgang/Extensions-Logging-Data`). Rotating the release
  credential is now automatic — there is no key to leak. (#168)

- **SHA-pinned every GitHub Action across all workflows** — fleet
  mitigation for tj-actions-style attacks, applied to `actions/checkout`,
  `actions/setup-dotnet`, `actions/upload-artifact`,
  `actions/download-artifact`, `actions/setup-python`,
  `actions/attest-build-provenance`, `github/codeql-action/*`,
  `ossf/scorecard-action`, and `reviewdog/action-actionlint`. Dependabot's
  `github-actions` ecosystem keeps the SHAs fresh weekly, so the
  maintenance overhead stays automatic. (#187)

- **Reduced OSSF Scorecard open-alert count from 76 to 2.** SHA-pinning
  retired 58 `PinnedDependenciesID` alerts; a `jq`-based SARIF-filter
  step in `scorecard.yml` durably suppresses 16 more that are
  false-positive or structurally wrong-for-this-repo (deliberate
  `pull_request_target` usage, solo-maintainer branch protection,
  `nugetCommand` sub-check on `dotnet tool install` steps, etc.). The
  two survivors (`CITestsID` and `SASTID`, both 9/10) are real
  actionable signal — one older commit each is missing CI/CodeQL
  coverage — and are intentionally left alive. (#184, #187, #188, #189)

## [0.3.0] - 2026-07-14

`Wolfgang.Extensions.Logging.Data` only. The `EntityFramework6` companion package
is unchanged and stays at 0.2.0.

### Added

- **Trim / Native AOT compatibility.** The `LogDbConnection`, `LogDbCommand`,
  and dictionary-based `LogCommandText` surface is verified trim- and AOT-safe by
  a `PublishAot` + `PublishTrimmed` smoke consumer run in CI. (#94)

### Changed

- The **anonymous-object `LogCommandText(..., object parameters, ...)` overloads
  are now marked `[RequiresUnreferencedCode]`** — they reflect over the runtime
  type's properties, which the trimmer cannot preserve. Callers in trimmed /
  Native AOT apps get an `IL2026` warning; use the
  `IReadOnlyDictionary<string, object?>` overload instead. No change for
  non-trimmed consumers. (#94)

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

[Unreleased]: https://github.com/Chris-Wolfgang/Extensions-Logging-Data/compare/v0.3.1...HEAD
[0.3.1]: https://github.com/Chris-Wolfgang/Extensions-Logging-Data/compare/v0.3.0...v0.3.1
[0.3.0]: https://github.com/Chris-Wolfgang/Extensions-Logging-Data/compare/v0.2.0...v0.3.0
[0.2.0]: https://github.com/Chris-Wolfgang/Extensions-Logging-Data/compare/v0.1.1...v0.2.0
[0.1.1]: https://github.com/Chris-Wolfgang/Extensions-Logging-Data/compare/v0.1.0...v0.1.1
[0.1.0]: https://github.com/Chris-Wolfgang/Extensions-Logging-Data/releases/tag/v0.1.0
