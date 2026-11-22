window.BENCHMARK_DATA = {
  "lastUpdate": 1795357820626,
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
          "id": "ce649ed1a9f250c22778f0f4d5beb3f9e9dc1e5e",
          "message": "chore: move released API entries to PublicAPI.Shipped.txt (#296)\n\nLogCommandText / LogDbCommand shipped in 0.2.0 and AddLoggingInterceptors\nshipped in EF6 0.2.0, but both projects still listed them as unshipped.\nMove every entry to Shipped.txt so Unshipped.txt holds only new surface.\n\nCloses #288\n\nCo-authored-by: Claude Fable 5.1 <noreply@anthropic.com>",
          "timestamp": "2026-11-08T01:44:38Z",
          "url": "https://github.com/Chris-Wolfgang/Extensions-Logging-Data/commit/ce649ed1a9f250c22778f0f4d5beb3f9e9dc1e5e"
        },
        "date": 1794143530179,
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
          "id": "c1e0e1b4812f5468083d7eef0335b9d8ce1d5934",
          "message": "Bump CsCheck and 11 others (#397)\n\n* Bump CsCheck and 11 others\n\nBumps CsCheck from 4.9.1 to 5.0.1\nBumps dotnet-reportgenerator-globaltool from 5.5.12 to 5.6.0\nBumps dotnet-stryker from 5.0.0 to 5.1.0\nBumps jetbrains.resharper.globaltools from 2026.2.4.1 to 2026.3.0\nBumps Meziantou.Analyzer from 3.0.322 to 3.0.340\nBumps Microsoft.CodeAnalysis.CSharp to 5.12.0\nBumps microsoft.cst.devskim.cli from 1.0.100 to 1.0.103\nBumps Microsoft.Data.Sqlite to 10.0.15, 11.0.0\nBumps Microsoft.Extensions.Logging from 10.0.14 to 11.0.0\nBumps Microsoft.Extensions.Logging.Abstractions from 10.0.14 to 11.0.0\nBumps Microsoft.SourceLink.GitHub from 10.0.403 to 11.0.100\nBumps SonarAnalyzer.CSharp from 10.35.0.4138 to 10.36.0.15337\n\n---\nupdated-dependencies:\n- dependency-name: CsCheck\n  dependency-version: 5.0.1\n  dependency-type: direct:production\n  update-type: version-update:semver-major\n  dependency-group: dotnet-dependencies\n- dependency-name: dotnet-reportgenerator-globaltool\n  dependency-version: 5.6.0\n  dependency-type: direct:production\n  update-type: version-update:semver-minor\n  dependency-group: dotnet-dependencies\n- dependency-name: dotnet-stryker\n  dependency-version: 5.1.0\n  dependency-type: direct:production\n  update-type: version-update:semver-minor\n  dependency-group: dotnet-dependencies\n- dependency-name: jetbrains.resharper.globaltools\n  dependency-version: 2026.3.0\n  dependency-type: direct:production\n  update-type: version-update:semver-minor\n  dependency-group: dotnet-dependencies\n- dependency-name: Meziantou.Analyzer\n  dependency-version: 3.0.340\n  dependency-type: direct:production\n  update-type: version-update:semver-patch\n  dependency-group: dotnet-dependencies\n- dependency-name: Microsoft.CodeAnalysis.CSharp\n  dependency-version: 5.12.0\n  dependency-type: direct:production\n  update-type: version-update:semver-major\n  dependency-group: dotnet-dependencies\n- dependency-name: Microsoft.CodeAnalysis.CSharp\n  dependency-version: 5.12.0\n  dependency-type: direct:production\n  update-type: version-update:semver-minor\n  dependency-group: dotnet-dependencies\n- dependency-name: microsoft.cst.devskim.cli\n  dependency-version: 1.0.103\n  dependency-type: direct:production\n  update-type: version-update:semver-patch\n  dependency-group: dotnet-dependencies\n- dependency-name: Microsoft.Data.Sqlite\n  dependency-version: 10.0.15\n  dependency-type: direct:production\n  update-type: version-update:semver-patch\n  dependency-group: dotnet-dependencies\n- dependency-name: Microsoft.Data.Sqlite\n  dependency-version: 10.0.15\n  dependency-type: direct:production\n  update-type: version-update:semver-patch\n  dependency-group: dotnet-dependencies\n- dependency-name: Microsoft.Extensions.Logging\n  dependency-version: 11.0.0\n  dependency-type: direct:production\n  update-type: version-update:semver-major\n  dependency-group: dotnet-dependencies\n- dependency-name: Microsoft.Extensions.Logging.Abstractions\n  dependency-version: 11.0.0\n  dependency-type: direct:production\n  update-type: version-update:semver-major\n  dependency-group: dotnet-dependencies\n- dependency-name: Microsoft.Extensions.Logging\n  dependency-version: 11.0.0\n  dependency-type: direct:production\n  update-type: version-update:semver-major\n  dependency-group: dotnet-dependencies\n- dependency-name: Microsoft.Extensions.Logging.Abstractions\n  dependency-version: 11.0.0\n  dependency-type: direct:production\n  update-type: version-update:semver-major\n  dependency-group: dotnet-dependencies\n- dependency-name: Microsoft.Extensions.Logging.Abstractions\n  dependency-version: 11.0.0\n  dependency-type: direct:production\n  update-type: version-update:semver-major\n  dependency-group: dotnet-dependencies\n- dependency-name: Microsoft.Extensions.Logging.Abstractions\n  dependency-version: 11.0.0\n  dependency-type: direct:production\n  update-type: version-update:semver-major\n  dependency-group: dotnet-dependencies\n- dependency-name: Microsoft.SourceLink.GitHub\n  dependency-version: 11.0.100\n  dependency-type: direct:production\n  update-type: version-update:semver-major\n  dependency-group: dotnet-dependencies\n- dependency-name: SonarAnalyzer.CSharp\n  dependency-version: 10.36.0.15337\n  dependency-type: direct:production\n  update-type: version-update:semver-minor\n  dependency-group: dotnet-dependencies\n- dependency-name: Microsoft.Data.Sqlite\n  dependency-version: 11.0.0\n  dependency-type: direct:production\n  update-type: version-update:semver-major\n  dependency-group: dotnet-dependencies\n...\n\nSigned-off-by: dependabot[bot] <support@github.com>\n\n* test: use Gen.OneOf for constants (CsCheck 5 removed OneOfConst)\n\nCsCheck 5.0 dropped Gen.OneOfConst; Gen.OneOf<T>(params T[] constants) is\nthe same generator. The fuzz project failed to compile on this bump.\n\nCo-Authored-By: Claude Opus 5.5 <noreply@anthropic.com>\n\n---------\n\nSigned-off-by: dependabot[bot] <support@github.com>\nCo-authored-by: dependabot[bot] <49699333+dependabot[bot]@users.noreply.github.com>\nCo-authored-by: Chris Wolfgang <210299580+Chris-Wolfgang@users.noreply.github.com>\nCo-authored-by: Claude Opus 5.5 <noreply@anthropic.com>",
          "timestamp": "2026-11-14T21:54:14Z",
          "url": "https://github.com/Chris-Wolfgang/Extensions-Logging-Data/commit/c1e0e1b4812f5468083d7eef0335b9d8ce1d5934"
        },
        "date": 1794750822290,
        "tool": "customBiggerIsBetter",
        "benches": [
          {
            "name": "Mutation score (tests/Wolfgang.Extensions.Logging.Data.EntityFramework6.Tests.Unit)",
            "value": 100,
            "unit": "%"
          },
          {
            "name": "Mutation score (tests/Wolfgang.Extensions.Logging.Data.Tests.Unit)",
            "value": 97.98,
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
          "id": "658596d63611ab3bb4f4fc4114e4ef9021aed263",
          "message": "docs(readme): state the five-leg CI test matrix and the ARM64 TFM rule (#362) (#408)\n\nCo-authored-by: Claude Opus 5.5 <noreply@anthropic.com>",
          "timestamp": "2026-11-21T20:54:42Z",
          "url": "https://github.com/Chris-Wolfgang/Extensions-Logging-Data/commit/658596d63611ab3bb4f4fc4114e4ef9021aed263"
        },
        "date": 1795357816686,
        "tool": "customBiggerIsBetter",
        "benches": [
          {
            "name": "Mutation score (tests/Wolfgang.Extensions.Logging.Data.EntityFramework6.Tests.Unit)",
            "value": 100,
            "unit": "%"
          },
          {
            "name": "Mutation score (tests/Wolfgang.Extensions.Logging.Data.Tests.Unit)",
            "value": 100,
            "unit": "%"
          }
        ]
      }
    ]
  }
}