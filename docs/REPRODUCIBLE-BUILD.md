# Verifying a release of Wolfgang.Extensions.Logging.Data

Anyone can check that a published release of Wolfgang.Extensions.Logging.Data (and of
Wolfgang.Extensions.Logging.Data.EntityFramework6, built from the same tag) was built from the tagged source, without
trusting this repository's CI. This page is for a consumer who has never seen the repository.

## What "reproducible" means here

The same tag, built with the SDK recorded in the release's `BUILD-MANIFEST.txt`, gives:

- byte-identical `.dll` and `.pdb` files for every target framework, and
- `.nupkg` / `.snupkg` packages whose **entries** match the published packages entry by entry.

What is expected to differ, and is never compared:

- **the package file itself** (`.nupkg` / `.snupkg` as a zip): NuGet stamps every zip entry with the pack time, so
  two packs of the same commit never give the same zip bytes, even on one machine. Compare the entries.
- **a build on another operating system**: cross-OS identity is reported by the
  [Reproducible Build workflow](https://github.com/Chris-Wolfgang/Extensions-Logging-Data/actions/workflows/reproducible-build.yaml), not promised.

This repository's release packs on `windows-latest`, and `BUILD-MANIFEST.txt` names that runner. The cross-OS inputs
are pinned (`Directory.Build.targets` rewrites the SDK-generated sources to LF, and every `net4x` target compiles
against the `Microsoft.NETFramework.ReferenceAssemblies` package), so a Linux rebuild is expected to match; the
workflow's Windows-report job shows whether it does for the current commit. If a Linux rebuild differs, rebuild on
Windows before treating it as a discrepancy.

Every pull request is checked the same way: the workflow packs the commit twice on Ubuntu and fails unless both
packs are identical by the rules above.

## What a release publishes

Each GitHub Release carries, next to the packages:

| Asset | Contents |
|---|---|
| `SHA256SUMS.txt` | `<sha256>  <tfm>/<file>` for every `.dll`/`.pdb`, and `<sha256>  <package-file>` for every `.nupkg`/`.snupkg` |
| `BUILD-MANIFEST.txt` | exact SDK version (`dotnet --version`), runner image and version, commit SHA, tag, and the `CI` / `ContinuousIntegrationBuild` / `Deterministic` values |

Both are subjects of the release's build-provenance attestation, so step 5 below covers them as well as the packages.

## Verify a release

You need git, the GitHub CLI (`gh`), Python 3 and a Linux machine (or container).

1. **Clone the tag.**

   ```bash
   TAG=v1.2.3   # the release you are checking
   git clone --branch "$TAG" --depth 1 https://github.com/Chris-Wolfgang/Extensions-Logging-Data.git
   cd Extensions-Logging-Data
   gh release download "$TAG" --repo Chris-Wolfgang/Extensions-Logging-Data --dir ../release
   cat ../release/BUILD-MANIFEST.txt
   ```

2. **Install the recorded SDK.** `global.json` pins the major version; install the exact version that
   `BUILD-MANIFEST.txt` names (for example with `dotnet-install.sh --version <sdk>`), and check that `dotnet --version`
   prints it from inside the clone.

3. **Pack exactly as the workflow does**, once per packable project under `src/`:

   ```bash
   export CI=true
   for proj in $(git ls-files 'src/*.csproj' 'src/**/*.csproj'); do
     [ "$(dotnet msbuild "$proj" -getProperty:IsPackable | tr -d '[:space:]')" = "true" ] || continue
     dotnet pack "$proj" -c Release -p:ContinuousIntegrationBuild=true -p:Deterministic=true -o ../rebuilt
   done
   ```

4. **Compare.**

   ```bash
   # Assemblies and symbols, keyed <tfm>/<file> exactly as in SHA256SUMS.txt.
   for d in $(git ls-files 'src/*.csproj' 'src/**/*.csproj' | xargs -n1 dirname); do
     [ -d "$d/bin/Release" ] || continue
     (cd "$d/bin/Release" && find . -mindepth 2 -maxdepth 2 \( -name '*.dll' -o -name '*.pdb' \) \
       | sed 's|^\./||' | xargs sha256sum | sed 's/ \*/  /')   # Git Bash on Windows prints ' *<file>' (binary mode)
   done | sort -u -k2 > ../rebuilt-assemblies.txt
   grep -vE '\.s?nupkg$' ../release/SHA256SUMS.txt | sort -k2 | diff - ../rebuilt-assemblies.txt \
     && echo "Assemblies and symbols: identical"

   # Packages, entry by entry.
   python3 - ../release ../rebuilt <<'PY'
   import hashlib, pathlib, sys, zipfile
   def entries(p):
       with zipfile.ZipFile(p) as z:
           return {e.filename: hashlib.sha256(z.read(e)).hexdigest() for e in z.infolist() if not e.is_dir()}
   ok = True
   for pub in sorted(pathlib.Path(sys.argv[1]).glob("*.*nupkg")):
       mine = pathlib.Path(sys.argv[2]) / pub.name
       same = mine.exists() and entries(pub) == entries(mine)
       ok &= same
       print(("identical entries: " if same else "DIFFERS: ") + pub.name)
   sys.exit(0 if ok else 1)
   PY
   ```

5. **Check the provenance** of the downloaded assets against this repository:

   ```bash
   for f in ../release/*; do gh attestation verify "$f" --repo Chris-Wolfgang/Extensions-Logging-Data; done
   ```

## If the hashes differ

First rule out the benign causes: a different SDK patch (the `.pdb` records the compiler version, and the `.dll`
hashes the `.pdb`), or comparing the `.nupkg` zip bytes instead of its entries. If the assemblies still differ after
that, **treat it as a potential supply-chain discrepancy and report it**:

- Open an issue at <https://github.com/Chris-Wolfgang/Extensions-Logging-Data/issues> titled
  "Reproducibility mismatch for v\<VERSION\>", with the tag, your exact `dotnet --version` and OS, and the `diff`
  output from step 4.
- If you believe a published package was tampered with (not just a toolchain difference), also follow the reporting
  path in [`SECURITY.md`](../SECURITY.md) / [`DISASTER-RECOVERY.md`](DISASTER-RECOVERY.md).

## Releases cut before the standard

A tag cut before this repository took the reproducible-builds pieces (literal copyright year, LF-normalized generated
sources, the reference-assemblies pin, `global.json`) has no `SHA256SUMS.txt` or `BUILD-MANIFEST.txt`, and its copyright
attribute may carry the year it was built in. To rebuild such a tag, pass the original year
(`-p:Copyright="Copyright (c) <year> <holder>"`) and expect only the Linux-to-Linux comparison to hold.
