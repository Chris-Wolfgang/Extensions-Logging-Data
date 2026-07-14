# ADR-0001: vNext / protected-vNext release branching model

- **Status:** Accepted
- **Date:** 2026-07-13

## Context

The PR pipeline (`pr.yaml`) runs on `pull_request_target` targeting `main`, and a
`Detect .NET Projects` guard **fails by design** whenever a PR touches a protected
configuration file (`.github/workflows/*`, `Directory.Build.props`,
`Directory.Build.targets`, `.editorconfig`, `BannedSymbols.txt`, `*.globalconfig`,
`*.ruleset`). This makes protected-file PRs unmergeable without an admin bypass —
intentional, so those files can't be weakened from within a PR.

That is safe but awkward when a release bundles several changes, some of which
touch protected files: every such PR would need its own admin-bypass, and the
test stages never run (they `needs: detect-projects`, which fails).

## Decision

Accumulate a release on a per-cycle **`vNext`** integration branch, then land it
through a short chain:

```
feature PRs ──▶ vNext ──▶ protected/vNext ──▶ main
```

- Feature PRs target **`vNext`**. Because `pr.yaml` only triggers on PRs whose base
  is `main`, PRs into `vNext` don't run the guard at all — they merge normally.
- `vNext → protected/vNext` is an integration PR (base isn't `main`, so no guard).
- `protected/vNext → main` is the **single** admin-bypass hop that carries the
  whole batch (protected files included) onto `main`.

`vNext` and `protected/vNext` are **per-release-cycle**: created from `main` when a
cycle starts, deleted after the batch lands. They are not long-lived branches.

## Consequences

- One admin-bypass per release instead of one per protected-file PR.
- The trade-off: PRs targeting `vNext` get **no CI** (the stages are `main`-gated),
  so real validation happens (a) locally before each push and (b) at the final
  `protected/vNext → main` PR, where everything runs against `main`. Contributors
  must verify locally — see `CONTRIBUTING.md`.
- Do **not** treat `vNext` as permanent. After a release, leave it deleted; don't
  auto-restore it. A new cycle recreates it from `main`.
- A protected-file PR will always show `Detect .NET Projects: fail` and skipped
  test stages — that is the guard working, not a broken pipeline.
