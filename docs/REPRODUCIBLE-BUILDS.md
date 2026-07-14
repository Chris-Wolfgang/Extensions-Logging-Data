# Reproducible builds — independent verification

This package is built with reproducibility **inputs** enabled:

- **Deterministic compilation** — `<Deterministic>` defaults to `true` for
  SDK-style projects, so compiler output doesn't depend on the wall clock, machine
  paths, or ordering.
- `<ContinuousIntegrationBuild>true</ContinuousIntegrationBuild>` on CI (set in
  `Directory.Build.props`) — normalizes stored source paths.
- SourceLink + `<EmbedUntrackedSources>` (in `Directory.Build.props`) — the exact
  source is discoverable from the symbols.

That means a third party can rebuild the published package from source and confirm
it matches — "trust, but verify". Here's how.

## Verify a published version yourself

**Prerequisites:** the .NET SDK version the release was built with (see the release
notes / `global.json` if present), and `git`.

```bash
# 1. Get the exact source for the version you want to verify.
git clone https://github.com/Chris-Wolfgang/Extensions-Logging-Data.git
cd Extensions-Logging-Data
git checkout v<VERSION>          # e.g. v0.2.0 — the tag matches the package version

# 2. Build the package the same way the release does.
dotnet pack src/Wolfgang.Extensions.Logging.Data/Wolfgang.Extensions.Logging.Data.csproj \
  -c Release -o ./verify

# 3. Download the published package from NuGet.
curl -sSL -o ./verify/published.nupkg \
  "https://api.nuget.org/v3-flatcontainer/wolfgang.extensions.logging.data/<VERSION>/wolfgang.extensions.logging.data.<VERSION>.nupkg"

# 4. Compare the compiled assembly inside each .nupkg. A .nupkg is a zip; the
#    assembly is the reproducible artifact (the .nupkg envelope itself carries
#    timestamps, so compare the DLLs, not the zip bytes).
mkdir -p verify/mine verify/theirs
unzip -oq verify/Wolfgang.Extensions.Logging.Data.<VERSION>.nupkg -d verify/mine
unzip -oq verify/published.nupkg -d verify/theirs
find verify/mine   -name '*.dll' -exec sha256sum {} \; | sed 's#verify/mine/##'   | sort > verify/mine.sha
find verify/theirs -name '*.dll' -exec sha256sum {} \; | sed 's#verify/theirs/##' | sort > verify/theirs.sha
diff verify/mine.sha verify/theirs.sha && echo "✅ assemblies are byte-identical"
```

A clean `diff` proves the published binaries were built from this source with no
injection in between.

## If the hashes differ

First rule out the benign causes below (a different SDK, or comparing the `.nupkg`
zip instead of the assemblies). If the assemblies still differ after that, **treat
it as a potential supply-chain discrepancy and report it**:

- Open an issue at
  <https://github.com/Chris-Wolfgang/Extensions-Logging-Data/issues> titled
  "Reproducibility mismatch for v\<VERSION\>", including: the package version, your
  **exact `dotnet --version`** and OS, and the two `sha256` lists (`verify/mine.sha`
  and `verify/theirs.sha`) plus their `diff`.
- If you believe the published package was tampered with (not just a toolchain
  difference), also follow the reporting path in
  [`SECURITY.md`](../SECURITY.md) / [`DISASTER-RECOVERY.md`](DISASTER-RECOVERY.md).

## Notes / caveats

- Compare the **assemblies** (`lib/**/*.dll`), not the `.nupkg` zip bytes — the
  package envelope stores creation timestamps and isn't itself byte-reproducible.
- Use the **same SDK major.minor**. A different Roslyn version can legitimately
  produce different (but equally correct) IL.
- The `net462` target may embed a framework-reference hash; if a specific TFM
  differs, verify the others and file an issue noting the TFM.
- This is the consumer-side counterpart to the repo's own build-reproducibility
  check (which builds the same commit twice on CI and compares). If that check is
  green and this local diff is clean, the chain from source to NuGet is verified.
