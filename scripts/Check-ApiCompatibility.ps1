#!/usr/bin/env pwsh
<#
.SYNOPSIS
    ABI-compatibility gate: compares each freshly-built src package assembly
    against the previous version published on NuGet using Microsoft.DotNet.ApiCompat,
    and fails when a NON-major version bump introduces a breaking change.

.DESCRIPTION
    Complements PublicApiAnalyzers (which diffs signatures at source level). ApiCompat
    catches binary/behavioural breaks — default-value changes, nullability flips,
    removed members a consumer's compiled assembly binds to at runtime.

    Per SemVer: MAJOR may break ABI; MINOR/PATCH may not. For a 0.x package every
    bump is treated as potentially-breaking-allowed only when the MAJOR component is 0
    AND the caller opts in via -Allow0xMinorBreaks (0.x MINOR bumps are permitted to
    break per SemVer's 0.x clause). By default 0.x MINOR/PATCH breaks are reported and
    fail, which is the safer stance for a library approaching 1.0.

    Intentional, reviewed breaks are recorded one-per-line in compat-suppressions.txt
    (an ApiCompat suppression id or a substring of the message) so they don't recur
    silently.

.NOTES
    Requires the Microsoft.DotNet.ApiCompat.Tool global tool on PATH (installed by the
    release workflow). Runnable locally: pwsh scripts/Check-ApiCompatibility.ps1
#>
[CmdletBinding()]
param(
    [string]$Configuration = 'Release',
    [string]$SuppressionsFile = "$PSScriptRoot/../compat-suppressions.txt",
    [switch]$Allow0xMinorBreaks
)

$ErrorActionPreference = 'Stop'
$repoRoot = Resolve-Path "$PSScriptRoot/.."
$flat = 'https://api.nuget.org/v3-flatcontainer'

$suppressions = @()
if (Test-Path $SuppressionsFile) {
    $suppressions = Get-Content $SuppressionsFile |
        Where-Object { $_ -and -not $_.TrimStart().StartsWith('#') } |
        ForEach-Object { $_.Trim() }
}

function Get-PreviousVersion([string]$packageId, [string]$currentVersion) {
    $url = "$flat/$($packageId.ToLowerInvariant())/index.json"
    try {
        $all = (Invoke-RestMethod -Uri $url -ErrorAction Stop).versions
    }
    catch {
        # Only a genuine HTTP 404 means the package has never been published
        # (first release). Any other failure — a different HTTP status, or a
        # network/DNS/TLS error with no HTTP response at all — must NOT silently
        # skip the ABI gate; surface it and fail. `.Response` is null for the
        # no-response cases, so guard it before touching StatusCode.
        $response = $_.Exception.Response
        if ($response -and [int]$response.StatusCode -eq 404) {
            return $null
        }
        throw "Failed to query NuGet for '$packageId': $($_.Exception.Message)"
    }
    $cur = [version]($currentVersion -replace '[-+].*$', '')
    $prev = $all |
        Where-Object { $_ -notmatch '-' } |
        ForEach-Object { [version]($_ -replace '[-+].*$', '') } |
        Where-Object { $_ -lt $cur } |
        Sort-Object |
        Select-Object -Last 1
    return $prev
}

$breakingTotal = 0
$projects = Get-ChildItem -Path (Join-Path $repoRoot 'src') -Recurse -Filter '*.csproj'

foreach ($proj in $projects) {
    [xml]$xml = Get-Content $proj.FullName
    $pg = $xml.Project.PropertyGroup
    $packageId = ($pg.PackageId | Where-Object { $_ } | Select-Object -First 1)
    if (-not $packageId) { $packageId = [IO.Path]::GetFileNameWithoutExtension($proj.Name) }
    $version = ($pg.Version | Where-Object { $_ } | Select-Object -First 1)
    if (-not $version) { Write-Host "skip $packageId (no <Version>)"; continue }

    $prev = Get-PreviousVersion $packageId $version
    if (-not $prev) {
        Write-Host "OK  $packageId $version — first release (no previous version to compare)."
        continue
    }

    $cur = [version]($version -replace '[-+].*$', '')
    $isMajorBump = $cur.Major -gt $prev.Major
    $is0xMinorBump = ($cur.Major -eq 0 -and $prev.Major -eq 0 -and $cur.Minor -gt $prev.Minor)
    $breaksAllowed = $isMajorBump -or ($is0xMinorBump -and $Allow0xMinorBreaks)

    # Download + extract the previous package.
    $work = Join-Path ([IO.Path]::GetTempPath()) "apicompat-$packageId-$prev"
    if (Test-Path $work) { Remove-Item $work -Recurse -Force }
    New-Item -ItemType Directory -Force -Path $work | Out-Null
    $nupkg = Join-Path $work 'prev.nupkg'
    $lid = $packageId.ToLowerInvariant()
    Invoke-WebRequest -UseBasicParsing -Uri "$flat/$lid/$prev/$lid.$prev.nupkg" -OutFile $nupkg
    Expand-Archive -Path $nupkg -DestinationPath $work -Force

    $projDir = Split-Path $proj.FullName -Parent
    $curLibRoot = Join-Path $projDir "bin/$Configuration"

    # Compare every TFM present in BOTH the previous package and the current build.
    $prevTfms = Get-ChildItem (Join-Path $work 'lib') -Directory -ErrorAction SilentlyContinue
    foreach ($tfmDir in $prevTfms) {
        $tfm = $tfmDir.Name
        $prevDll = Join-Path $tfmDir.FullName "$packageId.dll"
        $curDll = Join-Path $curLibRoot "$tfm/$packageId.dll"
        if (-not (Test-Path $prevDll) -or -not (Test-Path $curDll)) { continue }

        Write-Host "--- $packageId [$tfm]: $prev -> $version ---"
        $raw = & apicompat --left $prevDll --right $curDll 2>&1

        $breaks = $raw |
            Where-Object { $_ -match 'CP[0-9]{4}' } |
            Where-Object { $line = $_; -not ($suppressions | Where-Object { $line -like "*$_*" }) }

        if ($breaks) {
            if ($breaksAllowed) {
                Write-Host "  ⚠️  $($breaks.Count) ABI break(s) — allowed for this bump (record intentional ones in compat-suppressions.txt):"
                $breaks | ForEach-Object { Write-Host "     $_" }
            }
            else {
                Write-Host "  ❌ $($breaks.Count) ABI break(s) in a non-major bump:"
                $breaks | ForEach-Object { Write-Host "     $_" }
                $breakingTotal += $breaks.Count
            }
        }
        else {
            Write-Host "  ✅ no breaking changes."
        }
    }
}

if ($breakingTotal -gt 0) {
    Write-Error "❌ $breakingTotal unsuppressed ABI break(s) in a non-major release. Bump MAJOR or record intentional breaks in compat-suppressions.txt."
    exit 1
}
Write-Host "✅ ApiCompat: no disallowed ABI breaks."
