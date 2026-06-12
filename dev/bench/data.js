window.BENCHMARK_DATA = {
  "lastUpdate": 1781229354443,
  "repoUrl": "https://github.com/Chris-Wolfgang/Extensions-Logging-Data",
  "entries": {
    "BenchmarkDotNet": [
      {
        "commit": {
          "author": {
            "email": "210299580+Chris-Wolfgang@users.noreply.github.com",
            "name": "Chris Wolfgang",
            "username": "Chris-Wolfgang"
          },
          "committer": {
            "email": "noreply@github.com",
            "name": "GitHub",
            "username": "web-flow"
          },
          "distinct": true,
          "id": "745c6eeaec90b7e651f1fcb7c7a298c45e412207",
          "message": "Merge pull request #72 from Chris-Wolfgang/vNext\n\nRelease v0.1.1: canonical maintenance round + AssemblyVersion fix",
          "timestamp": "2026-06-11T20:22:18-04:00",
          "tree_id": "79920371d2e33abfb48ad186a2605c3204a859ec",
          "url": "https://github.com/Chris-Wolfgang/Extensions-Logging-Data/commit/745c6eeaec90b7e651f1fcb7c7a298c45e412207"
        },
        "date": 1781223830842,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "Wolfgang.Extensions.Logging.Data.Benchmarks.DbConnectionLoggerExtensionsBenchmarks.LogDbConnection_DisabledLogger_FastPath",
            "value": 0.001812749852736791,
            "unit": "ns",
            "range": "± 0.0031397748463531223"
          },
          {
            "name": "Wolfgang.Extensions.Logging.Data.Benchmarks.DbConnectionLoggerExtensionsBenchmarks.LogDbConnection_EnabledLogger_FullWork",
            "value": 3234.824961344401,
            "unit": "ns",
            "range": "± 53.99431431181469"
          },
          {
            "name": "Wolfgang.Extensions.Logging.Data.Benchmarks.DbConnectionLoggerExtensionsBenchmarks.LogDbConnection_EnabledLogger_ExplicitDebugLevel",
            "value": 3281.5537338256836,
            "unit": "ns",
            "range": "± 51.78219206885666"
          }
        ]
      },
      {
        "commit": {
          "author": {
            "email": "210299580+Chris-Wolfgang@users.noreply.github.com",
            "name": "Chris Wolfgang",
            "username": "Chris-Wolfgang"
          },
          "committer": {
            "email": "noreply@github.com",
            "name": "GitHub",
            "username": "web-flow"
          },
          "distinct": true,
          "id": "ac589db832c89a38a41eefbb2a201019f726e7c9",
          "message": "Merge pull request #127 from Chris-Wolfgang/release/v0.1.1-prep\n\nRelease v0.1.1 prep: CHANGELOG entry + PublicAPI baseline",
          "timestamp": "2026-06-11T21:54:21-04:00",
          "tree_id": "56f86f4f3fafc9170bef059c6d7f7a5a5846359a",
          "url": "https://github.com/Chris-Wolfgang/Extensions-Logging-Data/commit/ac589db832c89a38a41eefbb2a201019f726e7c9"
        },
        "date": 1781229353592,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "Wolfgang.Extensions.Logging.Data.Benchmarks.DbConnectionLoggerExtensionsBenchmarks.LogDbConnection_DisabledLogger_FastPath",
            "value": 0.0010442597170670827,
            "unit": "ns",
            "range": "± 0.0018087108862576882"
          },
          {
            "name": "Wolfgang.Extensions.Logging.Data.Benchmarks.DbConnectionLoggerExtensionsBenchmarks.LogDbConnection_EnabledLogger_FullWork",
            "value": 3258.8899167378745,
            "unit": "ns",
            "range": "± 8.83521924112885"
          },
          {
            "name": "Wolfgang.Extensions.Logging.Data.Benchmarks.DbConnectionLoggerExtensionsBenchmarks.LogDbConnection_EnabledLogger_ExplicitDebugLevel",
            "value": 3133.5257568359375,
            "unit": "ns",
            "range": "± 24.83513125600716"
          }
        ]
      }
    ]
  }
}