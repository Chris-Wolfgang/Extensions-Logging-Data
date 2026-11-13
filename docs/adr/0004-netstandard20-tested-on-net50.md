# ADR-0004: The net5.0 test slot runs the `netstandard2.0` build

- **Status:** Proposed
- **Date:** 2026-10-28 (revised 2026-11-12)

## Context

`Wolfgang.Extensions.Logging.Data` ships `net462`, `netstandard2.0`, `netstandard2.1` and
`net10.0`. NuGet picks the nearest asset for each test target framework:

| Test TFM | Nearest asset |
|---|---|
| `net462` - `net481` | `net462` |
| `net5.0` - `net9.0` | `netstandard2.1` |
| `net10.0` | `net10.0` |

So no test target framework resolves `netstandard2.0` on its own, and that build would be
compiled and analysed but never executed. The runtimes that pick it naturally are
`netcoreapp2.x`, which is out of support and which the current test runners no longer load,
and non-.NET-Core `netstandard2.0` hosts (Unity, Xamarin), which the CI matrix does not have.

## Decision

Keep all four assets, and point the unit tests' `net5.0` slot at the `netstandard2.0` build
with a conditional `ProjectReference` that sets
`SetTargetFramework="TargetFramework=netstandard2.0"`. Every other slot keeps its nearest
asset, so the tested assets become:

| Test TFM | Asset it runs |
|---|---|
| `net462` - `net481` | `net462` |
| `net5.0` | `netstandard2.0` |
| `net6.0` - `net9.0` | `netstandard2.1` |
| `net10.0` | `net10.0` |

`TestedBuildAssetTests` (compiled for `net5.0` only) asserts the loaded library's
`TargetFrameworkAttribute` is `.NETStandard,Version=v2.0`, so losing the wiring fails a test
instead of silently falling back to `netstandard2.1`.

Rejected: dropping `netstandard2.1` so that `net5.0` - `net9.0` resolve `netstandard2.0`
naturally. That changes the shipped package to remove the 2.1 asset; this decision changes
only the test project. Also rejected: an out-of-support `netcoreapp2.x` test slot.

## Consequences

- Every shipped asset is now executed by tests: `net462`, `netstandard2.0`, `netstandard2.1`
  and `net10.0`.
- `netstandard2.0` runs on one runtime (.NET 5) instead of the hosts that would pick it in
  practice. A defect specific to Unity or Xamarin is still not covered.
- The `net5.0` slot no longer tests `netstandard2.1`; `net6.0` - `net9.0` still do.
- Only the unit test project is wired this way. The integration, docs and fuzz projects keep
  their nearest asset.
