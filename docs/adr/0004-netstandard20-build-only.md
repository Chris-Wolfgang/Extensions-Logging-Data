# ADR-0004: The `netstandard2.0` build is compiled and analysed, not executed by tests

- **Status:** Proposed
- **Date:** 2026-10-28

## Context

`Wolfgang.Extensions.Logging.Data` ships `net462`, `netstandard2.0`, `netstandard2.1` and
`net10.0`. NuGet picks the nearest asset for each test target framework:

| Test TFM | Asset it runs |
|---|---|
| `net462` - `net481` | `net462` |
| `net5.0` - `net9.0` (and `netcoreapp3.1`) | `netstandard2.1` |
| `net10.0` | `net10.0` |

No test target framework resolves `netstandard2.0`, so that build is compiled (with warnings
as errors and every analyzer) but never executed. The only runtimes that would pick it are
`netcoreapp2.x`, which is out of support and which the current test runners no longer load,
and non-.NET-Core `netstandard2.0` hosts (Unity, Xamarin), which the CI matrix does not have.

The source is the same for all four assets. Its only conditional block is the
`TrimAnnotations.cs` polyfill (`#if !NET5_0_OR_GREATER`), which the `net462` and
`netstandard2.1` assets compile too, and both of those are tested.

## Decision

Keep `netstandard2.0` (dropping it would be a breaking change for `netstandard2.0`-only hosts)
and treat it as build-only coverage: it must compile cleanly with every analyzer, and its
behaviour is covered by the `net462` and `netstandard2.1` test runs. Do not add an
out-of-support `netcoreapp2.x` test slot to force it.

## Consequences

- A defect that exists only in the `netstandard2.0` build would not be caught by tests. Today
  there is no code that only that build compiles; a future `#if NETSTANDARD2_0` branch would
  be untested, so prefer conditions that the `net462` build shares.
- Revisit when the package next makes a breaking change: dropping `netstandard2.0` then
  removes the untested asset entirely.
