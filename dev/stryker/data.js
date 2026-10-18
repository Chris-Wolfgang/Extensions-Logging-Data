window.BENCHMARK_DATA = {
  "lastUpdate": 1792305252176,
  "repoUrl": "https://github.com/Chris-Wolfgang/Extensions-Logging-Data",
  "entries": {
    "Mutation score": [
      {
        "commit": {
          "author": {
            "name": "Chris Wolfgang",
            "username": "Chris-Wolfgang",
            "email": "210299580+Chris-Wolfgang@users.noreply.github.com"
          },
          "committer": {
            "name": "GitHub",
            "username": "web-flow",
            "email": "noreply@github.com"
          },
          "id": "b85913eba97c22577d067d6ca76dfa9d9a0d0254",
          "message": "security(ci): silence zizmor self-repository on the docfx call, with the reason (#252)\n\nzizmor flags `uses: ./.github/workflows/docfx.yaml` and asks for GitHub's\ndedicated `$/` self-repository syntax. We cannot switch yet: actionlint is a\nrequired check here and rejects `$/` until rhysd/actionlint#732 ships. The\nalert has therefore been sitting open with nothing to do about it.\n\nrepo-template resolved this by keeping `./` and carrying a rule-specific inline\nignore plus the reason and the exit condition. This takes the same four lines,\nso the alert closes and the next reader learns why the preferred syntax is not\nin use and when to switch.\n\nNo behaviour change: the workflow call is byte-identical apart from the comment.\n\nCo-authored-by: Claude Opus 5 <noreply@anthropic.com>",
          "timestamp": "2026-09-23T13:24:44Z",
          "url": "https://github.com/Chris-Wolfgang/Extensions-Logging-Data/commit/b85913eba97c22577d067d6ca76dfa9d9a0d0254"
        },
        "date": 1790490946811,
        "tool": "customBiggerIsBetter",
        "benches": [
          {
            "name": "Mutation score",
            "value": 89.66,
            "unit": "%"
          }
        ]
      },
      {
        "commit": {
          "author": {
            "name": "Chris Wolfgang",
            "username": "Chris-Wolfgang",
            "email": "210299580+Chris-Wolfgang@users.noreply.github.com"
          },
          "committer": {
            "name": "GitHub",
            "username": "web-flow",
            "email": "noreply@github.com"
          },
          "id": "b85913eba97c22577d067d6ca76dfa9d9a0d0254",
          "message": "security(ci): silence zizmor self-repository on the docfx call, with the reason (#252)\n\nzizmor flags `uses: ./.github/workflows/docfx.yaml` and asks for GitHub's\ndedicated `$/` self-repository syntax. We cannot switch yet: actionlint is a\nrequired check here and rejects `$/` until rhysd/actionlint#732 ships. The\nalert has therefore been sitting open with nothing to do about it.\n\nrepo-template resolved this by keeping `./` and carrying a rule-specific inline\nignore plus the reason and the exit condition. This takes the same four lines,\nso the alert closes and the next reader learns why the preferred syntax is not\nin use and when to switch.\n\nNo behaviour change: the workflow call is byte-identical apart from the comment.\n\nCo-authored-by: Claude Opus 5 <noreply@anthropic.com>",
          "timestamp": "2026-09-23T13:24:44Z",
          "url": "https://github.com/Chris-Wolfgang/Extensions-Logging-Data/commit/b85913eba97c22577d067d6ca76dfa9d9a0d0254"
        },
        "date": 1791101762414,
        "tool": "customBiggerIsBetter",
        "benches": [
          {
            "name": "Mutation score",
            "value": 89.66,
            "unit": "%"
          }
        ]
      },
      {
        "commit": {
          "author": {
            "name": "dependabot[bot]",
            "username": "dependabot[bot]",
            "email": "49699333+dependabot[bot]@users.noreply.github.com"
          },
          "committer": {
            "name": "GitHub",
            "username": "web-flow",
            "email": "noreply@github.com"
          },
          "id": "5b3ac649efe01eda010086858f81100341a53e59",
          "message": "build(deps): bump the github-actions group with 3 updates (#260)\n\nBumps the github-actions group with 3 updates: [github/codeql-action/upload-sarif](https://github.com/github/codeql-action), [github/codeql-action/init](https://github.com/github/codeql-action) and [github/codeql-action/analyze](https://github.com/github/codeql-action).\n\n\nUpdates `github/codeql-action/upload-sarif` from 4.38.0 to 4.38.2\n- [Release notes](https://github.com/github/codeql-action/releases)\n- [Changelog](https://github.com/github/codeql-action/blob/main/CHANGELOG.md)\n- [Commits](https://github.com/github/codeql-action/compare/b96794f015dfd88f77b49b1c93e0fa7110f94c63...2892aa5e19bbd11bc0cff5427e3b750a04d9e3c2)\n\nUpdates `github/codeql-action/init` from 4.38.0 to 4.38.2\n- [Release notes](https://github.com/github/codeql-action/releases)\n- [Changelog](https://github.com/github/codeql-action/blob/main/CHANGELOG.md)\n- [Commits](https://github.com/github/codeql-action/compare/b96794f015dfd88f77b49b1c93e0fa7110f94c63...2892aa5e19bbd11bc0cff5427e3b750a04d9e3c2)\n\nUpdates `github/codeql-action/analyze` from 4.38.0 to 4.38.2\n- [Release notes](https://github.com/github/codeql-action/releases)\n- [Changelog](https://github.com/github/codeql-action/blob/main/CHANGELOG.md)\n- [Commits](https://github.com/github/codeql-action/compare/b96794f015dfd88f77b49b1c93e0fa7110f94c63...2892aa5e19bbd11bc0cff5427e3b750a04d9e3c2)\n\n---\nupdated-dependencies:\n- dependency-name: github/codeql-action/upload-sarif\n  dependency-version: 4.38.2\n  dependency-type: direct:production\n  update-type: version-update:semver-patch\n  dependency-group: github-actions\n- dependency-name: github/codeql-action/init\n  dependency-version: 4.38.2\n  dependency-type: direct:production\n  update-type: version-update:semver-patch\n  dependency-group: github-actions\n- dependency-name: github/codeql-action/analyze\n  dependency-version: 4.38.2\n  dependency-type: direct:production\n  update-type: version-update:semver-patch\n  dependency-group: github-actions\n...\n\nSigned-off-by: dependabot[bot] <support@github.com>\nCo-authored-by: dependabot[bot] <49699333+dependabot[bot]@users.noreply.github.com>\nCo-authored-by: Chris Wolfgang <210299580+Chris-Wolfgang@users.noreply.github.com>",
          "timestamp": "2026-10-11T00:16:55Z",
          "url": "https://github.com/Chris-Wolfgang/Extensions-Logging-Data/commit/5b3ac649efe01eda010086858f81100341a53e59"
        },
        "date": 1791700636761,
        "tool": "customBiggerIsBetter",
        "benches": [
          {
            "name": "Mutation score",
            "value": 89.66,
            "unit": "%"
          }
        ]
      },
      {
        "commit": {
          "author": {
            "name": "Chris Wolfgang",
            "username": "Chris-Wolfgang",
            "email": "210299580+Chris-Wolfgang@users.noreply.github.com"
          },
          "committer": {
            "name": "GitHub",
            "username": "web-flow",
            "email": "noreply@github.com"
          },
          "id": "71f31a663d09f7dea8b0bfc08f6f8f95cde5c091",
          "message": "fix(scripts): security-alerts updates the open summary issue instead of filing a new one (#280)\n\nGet-TrackedIssues keyed each marker to the LAST issue in the listing. The REST\nissues listing (state=all) is newest first, so with a closed old summary and an\nopen newer one the map kept the CLOSED issue, and every weekly run filed a\nduplicate summary issue. Prefer the OPEN issue for a marker, and between two of\nthe same state the higher number. Port of Chris-Wolfgang/repo-template#719.\n\nCo-authored-by: Claude Opus 5.5 <noreply@anthropic.com>",
          "timestamp": "2026-10-15T17:44:12Z",
          "url": "https://github.com/Chris-Wolfgang/Extensions-Logging-Data/commit/71f31a663d09f7dea8b0bfc08f6f8f95cde5c091"
        },
        "date": 1792305248271,
        "tool": "customBiggerIsBetter",
        "benches": [
          {
            "name": "Mutation score",
            "value": 89.66,
            "unit": "%"
          }
        ]
      }
    ]
  }
}