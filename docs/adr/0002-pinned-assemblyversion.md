# ADR-0002: Pin `AssemblyVersion` at `1.0.0.0`

- **Status:** Accepted
- **Date:** 2026-07-13 (documenting a decision first made for the v0.1.1 cycle)

## Context

The src project sets an explicit `<AssemblyVersion>1.0.0.0</AssemblyVersion>` while
the package `<Version>` moves with every release (0.1.0, 0.1.1, 0.2.0, …). This
looks stale and is a tempting "cleanup" target — surely the assembly version
should track the package version?

The package ships a `net462` target. On .NET Framework, assembly binding is
**version-exact**: if `AssemblyVersion` changes on every minor/patch release, every
consumer must recompile or add a binding redirect just to pick up a bug-fix. A
regression that let the SDK derive `AssemblyVersion` from `<Version>` (dropping the
pin) reached a release elsewhere in the fleet and broke .NET Framework consumers —
the post-mortem is why this is now an explicit, documented decision.

## Decision

Keep `<AssemblyVersion>1.0.0.0</AssemblyVersion>` pinned. Carry the real release
version in `<FileVersion>` (regex-stripped from `<Version>`) and
`<InformationalVersion>` (auto-derived), which do **not** participate in binding.
Bump `AssemblyVersion` only on a deliberate **breaking** API change.

## Consequences

- .NET Framework consumers get patch/minor updates without recompiles or binding
  redirects.
- `AssemblyVersion` no longer identifies the release — use `FileVersion` /
  `InformationalVersion` / the NuGet package version for that.
- **Do not** remove the pin or let it be SDK-derived, even though it looks stale.
  If tooling or a template sync tries to drop it, restore it.
