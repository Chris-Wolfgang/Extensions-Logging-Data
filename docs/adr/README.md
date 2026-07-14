# Architecture Decision Records

This folder captures **non-obvious design decisions** — the ones whose rationale
is easy to lose six months after the PR that introduced them. Each ADR records
the *context*, the *decision*, and the *consequences* so a future maintainer (or
you-in-six-months) doesn't re-derive the same trade-off poorly, or silently undo
a deliberate choice.

## When to write one

Write an ADR when a decision is **not self-evident from the code** and would be
tempting to "clean up" without knowing why it's there. Examples: a deliberate
API-shape choice, a pinned version that looks stale, a banned API, a security
posture, a branching/release convention. Routine, obvious choices don't need one.

## How to add one

1. Copy [`TEMPLATE.md`](TEMPLATE.md) to `NNNN-short-kebab-title.md`, using the
   next free zero-padded number.
2. Fill in Context / Decision / Consequences. Keep it short — a screenful.
3. Set the status (`Proposed` → `Accepted`, later maybe `Superseded by ADR-XXXX`).
4. Add a row to the index below.

ADRs are **immutable once accepted** — don't rewrite history. If a decision
changes, write a new ADR that supersedes the old one and update both statuses.

## Index

| ADR | Title | Status |
|---|---|---|
| [0001](0001-vnext-protected-branching-model.md) | vNext / protected-vNext release branching model | Accepted |
| [0002](0002-pinned-assemblyversion.md) | Pin `AssemblyVersion` at `1.0.0.0` | Accepted |
| [0003](0003-anonymous-object-overload-not-aot-safe.md) | Anonymous-object `LogCommandText` overloads are not trim/AOT-safe | Accepted |
