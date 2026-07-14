# ADR-0003: Anonymous-object `LogCommandText` overloads are not trim/AOT-safe

- **Status:** Accepted
- **Date:** 2026-07-13

## Context

`LogCommandText` has two parameter-supplying styles: an explicit
`IReadOnlyDictionary<string, object?>` and a Dapper-style anonymous object
(`new { id = 1 }`). The anonymous-object path reflects over the runtime type's
public properties (`Type.GetProperties` + `PropertyInfo.GetValue`) to build the
name/value map.

Under IL trimming / Native AOT, the trimmer cannot statically see which properties
a reflected-over type has, so it may remove them — turning the overload into a
silent no-op (empty parameters). We evaluated annotating the parameter with
`[DynamicallyAccessedMembers(PublicProperties)]` to make the trimmer preserve them,
but that attribute is **only valid on `Type` / `string` parameters** — applying it
to an `object` parameter is `IL2098`. There is no annotation that makes reflecting
over an arbitrary `object`'s properties trim-safe.

## Decision

Mark the four anonymous-object `LogCommandText(..., object parameters, ...)`
overloads (and the internal `ToDictionary(object)` / `BuildPropertyReader` helpers)
`[RequiresUnreferencedCode]`. Consumers building trimmed / AOT apps get an `IL2026`
warning directing them to the dictionary overload, which is trim/AOT-safe.
`RequiresUnreferencedCodeAttribute` is polyfilled for the pre-net5 target
frameworks (net462, netstandard2.0/2.1).

The trim/AOT-safe surface (`LogDbConnection`, `LogDbCommand`, the dictionary
`LogCommandText`) is verified by a `PublishAot` smoke consumer in CI.

## Consequences

- The library's AOT story is honest and enforced: the safe surface is verified;
  the unsafe overload is flagged at the call site rather than failing silently.
- Non-trimmed consumers are unaffected — `[RequiresUnreferencedCode]` produces no
  warning outside trim/AOT analysis.
- **Do not** "fix" the `IL2026` by adding `[DynamicallyAccessedMembers]` to the
  `object` parameter — it is invalid there (`IL2098`). If a genuinely trim-safe
  design is wanted, it requires a source-generator or an explicit dictionary API,
  not an annotation.
