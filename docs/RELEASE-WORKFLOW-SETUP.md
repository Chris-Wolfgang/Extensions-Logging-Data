# Release Workflow Setup Guide

This guide explains how to configure a repository to use the standard `release.yaml` workflow. The same checklist applies whether you are bootstrapping a new repo from `repo-template` or auditing an existing one.

## Overview

The release workflow triggers when you **publish a GitHub Release** and implements a comprehensive validation and automatic deployment process that:
- ✅ Tests all target frameworks per test project on Windows
- ✅ Enforces 90% code coverage threshold
- ✅ Validates NuGet package integrity with smoke tests
- ✅ Automatically publishes to NuGet.org after validation passes
- ✅ Eliminates duplicate build work for faster releases

## Required Configuration

Complete the following one-time setup so that the workflow can publish releases:

### Register the Trusted Publisher on NuGet.org

`release.yaml` publishes with [NuGet Trusted Publishing](https://learn.microsoft.com/nuget/nuget-org/trusted-publishing):
the `publish-nuget` job requests a GitHub OIDC token (`id-token: write`), and the
`NuGet/login` action exchanges it for a short-lived API key. There is **no `NUGET_API_KEY`
repository secret** to create or rotate.

**Location:** [NuGet.org → your account → Trusted Publishing](https://www.nuget.org/account/trustedpublishing)

1. Add a policy for the GitHub repository owner `Chris-Wolfgang`, repository `Extensions-Logging-Data`.
2. **Workflow file:** `release.yaml`.
3. Leave the environment empty (the publish job does not use a GitHub environment).
4. The policy must cover both package IDs: `Wolfgang.Extensions.Logging.Data` and
   `Wolfgang.Extensions.Logging.Data.EntityFramework6`.

**What this does:** Lets the `publish-nuget` job authenticate as the package owner without a
stored key. The `NuGet/login` step fails the job when no matching policy exists.

### Verify Branch Protection Rules

**Location:** Settings → Branches → main (or Settings → Rules → Rulesets)

> **Note:** This repository ships `scripts/Fix-BranchRuleset.ps1`, which applies the standard branch ruleset. If you prefer, configure the equivalent settings manually using the checklist below.

Ensure the following settings are enabled:

- ✅ **Require a pull request before merging**
  - **Single developer repos:** 0 approvals (default)
  - **Multi-developer repos:** 1+ approvals (recommended)
- ✅ **Require status checks to pass before merging**
  - Required checks should include the following status check contexts:
    - "Stage 1: Linux Tests (.NET 5.0-10.0) + Coverage Gate"
    - "Stage 2: Windows Tests (.NET 5.0-10.0, Framework 4.6.2-4.8.1)"
    - "Stage 3: macOS Tests (.NET 6.0-10.0)"
    - "Security Scan (DevSkim)"
    - "Security Scan (CodeQL)"
- ✅ **Require branches to be up to date before merging**
- ✅ **Require conversation resolution before merging**
- ✅ **Do not allow bypassing the above settings** (recommended, even for admins)
- ✅ **Restrict deletions**
- ✅ **Require linear history** (optional but recommended)

**What this does:** Ensures all code merged to `main` has passed comprehensive validation, preventing broken releases.

## Testing the Release Workflow

After completing the setup, test the workflow by creating a GitHub Release:

1. Go to your repository's **Releases** page
2. Click **"Draft a new release"**
3. Choose or create a tag (e.g., `v0.0.1-test`)
4. Add a title and description (optional for a test)
5. Check **"Set as a pre-release"** for test releases
6. Click **"Publish release"**

The workflow triggers automatically when the release is published.

### Expected Workflow Behavior

1. **Job 1: validate-release** (3-10 minutes)
   - Runs all framework tests with coverage
   - Enforces 90% coverage threshold
   - Uploads coverage report
   - ✅ Auto-passes if tests succeed

2. **Job 2: pack-and-validate** (2-5 minutes)
   - Packs NuGet packages
   - Performs smoke test installation
   - Uploads packages as artifacts
   - ✅ Auto-passes if packages are valid

3. **Job 3: publish-nuget** (1-2 minutes)
   - Exchanges the job's OIDC token for a short-lived NuGet API key (`NuGet/login`)
   - Publishes packages to NuGet.org automatically
   - ✅ Auto-completes if the trusted-publisher policy matches

### Monitoring the Workflow

- **Actions Tab:** Shows workflow progress in real-time
- **Artifacts:** Each job uploads artifacts (coverage reports, packages)
- **Releases:** Check the Releases page after successful completion

## Troubleshooting

### "NuGet login" step fails

**Problem:** The `publish-nuget` job fails at the `NuGet login (OIDC → temporary API key)` step.

**Solution:**
1. Check the trusted-publisher policy on NuGet.org: owner `Chris-Wolfgang`, repository
   `Extensions-Logging-Data`, workflow `release.yaml`, and both package IDs.
2. Check that the job still has `permissions: id-token: write`.
3. Re-run the workflow from the Actions tab (do not re-publish the release)

### Tests Fail on Specific Framework

**Problem:** Tests pass on some frameworks but fail on others (e.g., net462).

**Solution:**
1. Check the test logs for framework-specific issues
2. Fix compatibility issues in your code
3. Test locally: `dotnet test --framework net462`
4. Push fix, then re-publish the release (or re-run the workflow from the Actions tab)

### Coverage Below 90% Threshold

**Problem:** Workflow fails at coverage validation step.

**Solution:**
1. Review `CoverageReport/Summary.txt` artifact
2. Add tests for uncovered code paths
3. Ensure tests run on all frameworks
4. Push fix, then re-publish the release (or re-run the workflow from the Actions tab)

### Smoke Test Fails to Install Package

**Problem:** Package packs successfully but fails smoke test installation.

**Solution:**
1. Check package dependencies in `.csproj`
2. Verify framework compatibility in `<TargetFrameworks>`
3. Test locally: `dotnet pack` then try installing in a test project
4. Fix packaging issues and re-publish the release (or re-run the workflow from the Actions tab)

## Production Release Checklist

Before creating a production GitHub Release (e.g., `v1.0.0`):

- [ ] All tests pass on all platforms (pr.yaml workflow)
- [ ] Code coverage meets 90% threshold
- [ ] Security scan shows no critical issues
- [ ] Version numbers updated in `.csproj` files
- [ ] `CHANGELOG.md` updated with release notes (if applicable)
- [ ] All PRs merged to `main` branch
- [ ] Local build succeeds: `dotnet build --configuration Release`
- [ ] Local tests pass: `dotnet test --configuration Release`

**Create a production release:**
1. Go to your repository's **Releases** page
2. Click **"Draft a new release"**
3. Choose or create the version tag (e.g., `v1.0.0`) targeting `main`
4. Add a title and release notes
5. Click **"Publish release"**

**After workflow completes:**
- [ ] Verify packages appear on NuGet.org
- [ ] Test installing package from NuGet.org in a clean project
- [ ] Announce release (if applicable)

## Workflow Architecture

```
┌─────────────────────────────────────────────────────────────┐
│  Trigger: Published GitHub Release                          │
└─────────────────────────────────────────────────────────────┘
                            │
                            ▼
┌─────────────────────────────────────────────────────────────┐
│  Job 1: validate-release (Windows)                          │
│  • Restore & Build                                          │
│  • Test all frameworks (net5.0-10.0, net462-481)           │
│  • Collect coverage                                         │
│  • Enforce 90% threshold                                    │
│  • Upload coverage artifacts                                │
└─────────────────────────────────────────────────────────────┘
                            │
                            ▼ (only if tests pass)
┌─────────────────────────────────────────────────────────────┐
│  Job 2: pack-and-validate (Windows)                         │
│  • Restore & Build (fresh)                                  │
│  • Pack NuGet packages                                      │
│  • Smoke test installation                                  │
│  • Upload package artifacts                                 │
└─────────────────────────────────────────────────────────────┘
                            │
                            ▼ (only if packing succeeds)
┌─────────────────────────────────────────────────────────────┐
│  Job 3: publish-nuget (Windows)                             │
│  • Download packages                                        │
│  • NuGet login (OIDC → short-lived API key)                │
│  • Publish to NuGet.org automatically                       │
└─────────────────────────────────────────────────────────────┘
```

## Key Improvements Over Previous Workflow

| Issue | Before | After |
|-------|--------|-------|
| **Framework Coverage** | Default framework only | All frameworks (net5.0-10.0, net462-481) |
| **Code Coverage** | Not enforced | 90% threshold enforced |
| **Package Validation** | None | Smoke test installation |
| **Deployment** | Incomplete publish script | Automatic publishing after validation |
| **Secret Validation** | None | Validates before publishing |
| **GitHub Releases** | Not used as trigger | Workflow triggered by published release |
| **Build Efficiency** | Duplicate builds in each job | Build once per job with dependencies |
| **Test Logging** | No logger parameter | Console logging with verbosity |
| **Permissions** | Read-only | Write access for releases |

## Support

If you encounter issues not covered in this guide:

1. Check the Actions tab of this repository on GitHub for detailed logs
2. Review artifacts uploaded by failed jobs
3. Consult the [GitHub Actions documentation](https://docs.github.com/en/actions)
4. Open an issue in this repository with:
   - Workflow run URL
   - Error message and logs
   - Steps to reproduce
