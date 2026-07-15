# Disaster Recovery — NuGet / GitHub account compromise

Recovery actions after a credential compromise are time-critical and easy to
fumble under stress. This runbook is the pre-written procedure. **Read it before
you need it.** Work top-to-bottom; the ordering is deliberate (contain first,
then eradicate, then communicate).

> Scope: compromise of the NuGet.org account, the GitHub account/org, or the
> machine/CI holding their credentials. Not for ordinary bugs or a bad release
> (for a bad-but-not-malicious release, just [unlist](#unlist-a-package-version)
> it and ship a fix).

## 0. Assume breach — the accounts and keys involved

| Asset | Where it lives | Blast radius if stolen |
|---|---|---|
| NuGet.org account | nuget.org login (protect with 2FA) | Publish/unlist any owned package |
| NuGet API key / trusted-publishing | GitHub Actions secret / nuget.org trusted publisher policy | Publish new package versions |
| GitHub account / PAT | github.com login, `~/.config/gh`, CI | Push code, edit workflows, releases, secrets |
| Release signing cert (if adopted) | secure key store | Sign packages as us |

## 1. Contain (minutes)

1. **NuGet:** sign in → **Account → API Keys → Regenerate/Delete** every key.
   If trusted publishing is used, remove the trusted-publisher policy for the repo.
2. **GitHub:** **Settings → Password** (rotate) and **Developer settings → Personal
   access tokens** (revoke all). **Settings → Sessions** → sign out everywhere.
3. **Repo secrets:** **Settings → Secrets and variables → Actions** → rotate/delete
   `NUGET_API_KEY` and anything else. Disable Actions if a workflow is actively
   exfiltrating (`Settings → Actions → Disable`).
4. If a **machine** was compromised, assume everything on it is exposed — rotate
   from a known-clean device.

## 2. Eradicate

- Review recent **NuGet package versions** — [unlist](#unlist-a-package-version)
  anything you didn't publish.
- Review **GitHub**: recent commits, new/edited workflows, new collaborators,
  new deploy keys, changed branch protection / rulesets, new releases. Revert
  anything unexpected; remove rogue access.
- Re-enable 2FA everywhere; require it for the org.

## 3. Recover

- Restore branch protection / rulesets from `repo-template` canonical.
- Re-issue a clean `NUGET_API_KEY` (scoped to the `Wolfgang.*` package glob only,
  "push new packages and versions") or re-establish trusted publishing.
- Rebuild and re-release a known-good version from a clean checkout.

## 4. Communicate

- If a **malicious package version shipped**: open a security advisory on the repo,
  then request a hard delete (unlisting hides it but doesn't remove it) —
  [unlist it first](#unlist-a-package-version), file via the
  [NuGet contact form](https://www.nuget.org/policies/Contact) and email
  **support@nuget.org**, and for a platform-level compromise also report to the
  Microsoft Security Response Center (**secure@microsoft.com** /
  <https://msrc.microsoft.com/report>). Note it in `CHANGELOG.md` under **Security**.
- Notify consumers via the advisory + release notes with the affected versions and
  the safe version to move to.

## Reference

### Unlist a package version
nuget.org → the package → **Manage → Listing** → uncheck the version → **Save**.
Unlisting hides it from search/restore-by-default but existing lockfiles can still
resolve it — for a malicious version, also request deletion via NuGet support.

### Who has bypass / admin
Repo admin(s) can admin-bypass branch protection (see
[ADR-0001](adr/0001-vnext-protected-branching-model.md)). Keep the admin list
minimal and 2FA-enforced.
