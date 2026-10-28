# Copilot Coding Agent Instructions

## Repository summary

`Chris-Wolfgang/Extensions-Logging-Data` ships two NuGet packages:

| Package | Project | Target frameworks |
|---|---|---|
| `Wolfgang.Extensions.Logging.Data` | `src/Wolfgang.Extensions.Logging.Data` | `net462`, `netstandard2.0`, `netstandard2.1`, `net10.0` |
| `Wolfgang.Extensions.Logging.Data.EntityFramework6` | `src/Wolfgang.Extensions.Logging.Data.EntityFramework6` | `net462`, `net48`, `netstandard2.1` |

The core package adds `ILogger` extension methods (`LogDbConnection`, `LogDbCommand`, `LogCommandText`)
that log ADO.NET connections and commands as structured entries, with connection-string credentials and
opted-in parameter values redacted. The EF6 package registers command, connection and transaction
interceptors that call those methods for every EF6 operation.

## Projects

- `src/` - the two packages above.
- `tests/Wolfgang.Extensions.Logging.Data.Tests.Unit` - unit tests for the core package (net462 through net10.0).
- `tests/Wolfgang.Extensions.Logging.Data.EntityFramework6.Tests.Unit` - unit tests for the EF6 package.
- `tests/Wolfgang.Extensions.Logging.Data.Tests.Integration` - the real `Microsoft.Extensions.Logging` pipeline over SQLite.
- `tests/Wolfgang.Extensions.Logging.Data.Tests.Docs` - compiles the XML-doc `<code>` examples.
- `tests/Wolfgang.Extensions.Logging.Data.AotSmoke` - a Native AOT console app that proves the trim-safe surface publishes.
- `benchmarks/` - BenchmarkDotNet benchmarks (not part of the test gate).

## Build and validate

Run the same gate CI runs, from the repository root:

```powershell
pwsh ./scripts/build-pr.ps1             # restore, Release build (warnings as errors), every test TFM, coverage gate, security scan
pwsh ./scripts/build-pr.ps1 -SkipSecurity
```

Or by hand:

```powershell
dotnet restore
dotnet build --no-restore --configuration Release
dotnet test --no-build --configuration Release
```

Coverage gates (enforced by `.github/workflows/pr.yaml` and `scripts/build-pr.ps1`): production assemblies
are held to `CODECOV_MINIMUM` and every test assembly to `CODECOV_TEST_MINIMUM` (see the `env:` block at the
top of `pr.yaml` for the current values).

Every PR that changes `src/` adds a changelog fragment under `changelog/unreleased/`
(see `changelog/unreleased/README.md`); `CHANGELOG.md` is assembled at release time.

## Conventions

- Allman braces, 4-space indentation, file-scoped namespaces, explicit `using` directives on every TFM
  (no implicit or global usings).
- Public API changes must be recorded in each project's `PublicAPI.Unshipped.txt`.
- Synchronous blocking APIs are banned by `BannedSymbols.txt`.
- Warnings are errors in Release builds; fix them rather than suppressing.
- Test names follow `MethodUnderTest_when_condition_expected_result`.

## Security scanning

- gitleaks (secrets), CodeQL, Semgrep, zizmor (workflows), ReSharper InspectCode and OpenSSF Scorecard run
  as GitHub Actions workflows. DevSkim runs in `pr.yaml`.

## Release

Releases are cut by publishing a GitHub Release; `.github/workflows/release.yaml` builds, tests, packs and
publishes to NuGet.org through NuGet Trusted Publishing (OIDC). There is no NuGet API key secret. See
`docs/RELEASE-WORKFLOW-SETUP.md`.
