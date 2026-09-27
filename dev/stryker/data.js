window.BENCHMARK_DATA = {
  "lastUpdate": 1790490950455,
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
      }
    ]
  }
}