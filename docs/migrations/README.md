# Migration guides

When a **major** version ships with breaking changes (e.g. `0.x → 1.0`, or
`1.x → 2.0`), consumers need a written upgrade path: what was renamed / removed /
behaviourally changed, the replacement, and before/after code.

No major version with breaking changes has shipped yet, so there are no migration
guides here — only the **convention**, established now so it's ready when needed:

## Convention

- One file per major transition: `v{from}-to-v{to}.md` (e.g. `v1-to-v2.md`).
- Start from [`TEMPLATE-major-version-migration.md`](TEMPLATE-major-version-migration.md).
- Write it **as part of the PR that introduces the break**, not after release —
  the author has the context then.
- Link it from the `CHANGELOG.md` entry for that release and from the release notes.
- SemVer reminder: breaking changes are only allowed in a **major** bump (and, for
  `0.x`, a minor bump per SemVer's 0.x clause). If you're writing one of these for
  a patch, something is wrong.

## Index

_None yet — the library is pre-1.0. The first entry lands with the first breaking
major release._
