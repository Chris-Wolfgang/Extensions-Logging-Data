# Reproducible builds — independent verification

This package is built with reproducibility **inputs** enabled in
`Directory.Build.props`:

- `<Deterministic>true</Deterministic>` — compiler output doesn't depend on wall
  clock, machine paths, or ordering.
- `<ContinuousIntegrationBuild>true</ContinuousIntegrationBuild>` (on CI) —
  normalizes stored source paths.
- SourceLink + `<EmbedUntrackedSources>` — the exact source is discoverable from
  the symbols.

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
