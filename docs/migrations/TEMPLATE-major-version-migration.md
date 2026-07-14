# Migrating from v{FROM} to v{TO}

> One-paragraph summary: who is affected, roughly how much work, and whether the
> upgrade can be done incrementally.

## At a glance

| Change | Kind | Action |
|---|---|---|
| `OldApi(...)` | Renamed → `NewApi(...)` | Mechanical rename |
| `SomeType.OldProp` | Removed | Use `NewProp` |
| `Method(...)` default behaviour | Changed | Review call sites; pass X explicitly to keep old behaviour |

## Breaking changes in detail

### 1. `<what changed>`

**Why:** <the reason — link the ADR or PR>.

**Before:**

```csharp
// old usage
```

**After:**

```csharp
// new usage
```

## Non-breaking additions worth adopting

- <new API> — <what it gives you>.

## Upgrade checklist

- [ ] Bump the package reference to `{TO}`.
- [ ] Apply the renames/replacements above (the compiler flags most of them).
- [ ] Re-run tests; review any changed-behaviour call sites.
- [ ] Remove any workarounds that the new version makes unnecessary.
