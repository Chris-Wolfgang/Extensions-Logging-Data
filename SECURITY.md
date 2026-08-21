# Security Policy

## Reporting a Vulnerability

If you discover a security vulnerability, please follow these steps:

1. **Do not** create a public issue on this repository.
2. In the top navigation of this repository, click the **Security** tab.
3. In the top right, click the **Report a vulnerability** button.
4. Fill out the provided form with:
   - A description of the vulnerability
   - Steps to reproduce the issue
   - Potential impact
   - Suggested fix (if you have one)

## Response Timeline

We will acknowledge your report within 48 hours and provide an estimated timeline for a fix.

## Thank You

Your help is greatly appreciated!
Responsible disclosure of security vulnerabilities helps protect our entire community.

## Supply-Chain Verification

This section documents how consumers can verify that a published NuGet
package was genuinely built from this repository by the `release.yaml`
workflow.

### SBOM (Software Bill of Materials)

Every release attaches a CycloneDX SBOM per shipped package to the GitHub
Release assets:

- `Wolfgang.Extensions.Logging.Data.bom.json`
- `Wolfgang.Extensions.Logging.Data.EntityFramework6.bom.json`

Each SBOM lists every NuGet dependency and its version.

To audit the dependency graph:

1. Download the `.bom.json` file(s) from the GitHub Release page.
2. Open in any CycloneDX-compatible tool (e.g.,
   [CycloneDX CLI](https://github.com/CycloneDX/cyclonedx-cli),
   [OWASP Dependency-Track](https://dependencytrack.org/)).
3. Cross-reference component licenses and versions against your own policy.

### SLSA Build Provenance Attestation

Every release generates a **SLSA Build Level 2** provenance attestation
signed via [Sigstore](https://sigstore.dev/) keyless signing through
GitHub's OIDC identity. The attestation proves that the `.nupkg` / `.snupkg`
files were produced by the `release.yaml` workflow at a specific commit in
this repository — with no opportunity for an attacker to inject artifacts
without leaving a verifiable audit trail.

**To verify a package:**

1. Install the [GitHub CLI](https://cli.github.com/) (v2.49.0+).
2. Download the `.nupkg` from NuGet or the GitHub Release page.
3. Run:

   ```sh
   gh attestation verify Wolfgang.Extensions.Logging.Data.<version>.nupkg \
     --owner Chris-Wolfgang \
     --repo Extensions-Logging-Data
   ```

   Or for the EF6 companion package:

   ```sh
   gh attestation verify Wolfgang.Extensions.Logging.Data.EntityFramework6.<version>.nupkg \
     --owner Chris-Wolfgang \
     --repo Extensions-Logging-Data
   ```

4. A successful verification prints the signing workflow, commit SHA, and
   Sigstore transparency log entry. Failure means the artifact cannot be
   traced to a legitimate release run.

### Package Signing (deferred)

NuGet package signing via a code-signing certificate (or Sigstore `cosign`)
is **not yet implemented** — tracked as
[#181](https://github.com/Chris-Wolfgang/Extensions-Logging-Data/issues/181),
blocked on obtaining a code-signing certificate. SLSA attestation via
`gh attestation verify` provides an equivalent supply-chain integrity
guarantee for most scenarios.

Once implemented, consumers will be able to run `dotnet nuget verify` to
check the embedded signature independently of the GitHub CLI.

## Release path & compromise scope

Facts a maintainer would need at 2am if the release identity is compromised. Generic incident-response steps (rotating credentials, revoking OAuth apps, publishing advisories, unlisting NuGet packages) are not duplicated here — GitHub's and NuGet's own docs update faster than a checked-in runbook.

- **Release path**: OIDC / NuGet Trusted Publishing via `NuGet/login@v1` in `.github/workflows/release.yaml`. The workflow mints an ephemeral push token per run via OIDC — the release path does not depend on a long-lived API key stored in GitHub secrets or on the NuGet account. During an incident, check the NuGet account for any long-lived API keys anyway (they can be created outside of CI) and delete anything you don't recognize.
- **Fallback**: none. If Trusted Publishing is compromised, the incident is at the GitHub-account level (the OIDC identity is `Chris-Wolfgang/Extensions-Logging-Data`).
- **Owner**: @Chris-Wolfgang.
- **Downstream consumers**: none known within the Wolfgang.* fleet; the in-repo `Wolfgang.Extensions.Logging.Data.EntityFramework6` companion package depends on the core package. Unknown external consumers may exist on nuget.org.
- **Package coordinates for unlisting** (this repo ships two packages):
  - `Wolfgang.Extensions.Logging.Data` — https://www.nuget.org/packages/Wolfgang.Extensions.Logging.Data/
  - `Wolfgang.Extensions.Logging.Data.EntityFramework6` — https://www.nuget.org/packages/Wolfgang.Extensions.Logging.Data.EntityFramework6/
