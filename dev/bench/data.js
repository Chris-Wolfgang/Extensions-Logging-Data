window.BENCHMARK_DATA = {
  "lastUpdate": 1782517278155,
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
          "id": "a6026ad780a469b6b84ea97a765b625baa9bd634",
          "message": "Merge pull request #131 from Chris-Wolfgang/feature/3-log-command-text\n\nAdd LogCommandText with Dapper-style anonymous-object overloads (closes #3)",
          "timestamp": "2026-06-19T12:17:16-04:00",
          "tree_id": "ee69fc8a05e62a2be47ab5990e01a82bfdb28e5a",
          "url": "https://github.com/Chris-Wolfgang/Extensions-Logging-Data/commit/a6026ad780a469b6b84ea97a765b625baa9bd634"
        },
        "date": 1781885984141,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "Wolfgang.Extensions.Logging.Data.Benchmarks.DbCommandLoggerExtensionsBenchmarks.Dictionary_EnabledLogger",
            "value": 324.3123927116394,
            "unit": "ns",
            "range": "± 1.6370779383730016"
          },
          {
            "name": "Wolfgang.Extensions.Logging.Data.Benchmarks.DbCommandLoggerExtensionsBenchmarks.AnonymousObject_EnabledLogger",
            "value": 433.0786301294963,
            "unit": "ns",
            "range": "± 1.7572285247442316"
          },
          {
            "name": "Wolfgang.Extensions.Logging.Data.Benchmarks.DbCommandLoggerExtensionsBenchmarks.Dictionary_DisabledLogger_FastPath",
            "value": 0.0008657922347386678,
            "unit": "ns",
            "range": "± 0.0014995961393659725"
          },
          {
            "name": "Wolfgang.Extensions.Logging.Data.Benchmarks.DbCommandLoggerExtensionsBenchmarks.AnonymousObject_DisabledLogger_FastPath",
            "value": 100.81561787923177,
            "unit": "ns",
            "range": "± 1.2437760541224967"
          },
          {
            "name": "Wolfgang.Extensions.Logging.Data.Benchmarks.DbConnectionLoggerExtensionsBenchmarks.LogDbConnection_DisabledLogger_FastPath",
            "value": 0,
            "unit": "ns",
            "range": "± 0"
          },
          {
            "name": "Wolfgang.Extensions.Logging.Data.Benchmarks.DbConnectionLoggerExtensionsBenchmarks.LogDbConnection_EnabledLogger_FullWork",
            "value": 2643.7482732137046,
            "unit": "ns",
            "range": "± 2.933350833986856"
          },
          {
            "name": "Wolfgang.Extensions.Logging.Data.Benchmarks.DbConnectionLoggerExtensionsBenchmarks.LogDbConnection_EnabledLogger_ExplicitDebugLevel",
            "value": 2654.855298360189,
            "unit": "ns",
            "range": "± 18.134692459398796"
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
          "id": "97a86cb7d5c93224620774c03ec258cbc8f57cdb",
          "message": "Merge pull request #134 from Chris-Wolfgang/dependabot/github_actions/github-actions-39b8605068\n\nBump the github-actions group with 2 updates",
          "timestamp": "2026-06-26T19:38:54-04:00",
          "tree_id": "aedc2e39f6489203b2f0be8c36e6af82ed322bd2",
          "url": "https://github.com/Chris-Wolfgang/Extensions-Logging-Data/commit/97a86cb7d5c93224620774c03ec258cbc8f57cdb"
        },
        "date": 1782517276682,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "Wolfgang.Extensions.Logging.Data.Benchmarks.DbCommandLoggerExtensionsBenchmarks.Dictionary_EnabledLogger",
            "value": 313.0269281069438,
            "unit": "ns",
            "range": "± 0.3115122390856967"
          },
          {
            "name": "Wolfgang.Extensions.Logging.Data.Benchmarks.DbCommandLoggerExtensionsBenchmarks.AnonymousObject_EnabledLogger",
            "value": 472.41882038116455,
            "unit": "ns",
            "range": "± 2.312216034460389"
          },
          {
            "name": "Wolfgang.Extensions.Logging.Data.Benchmarks.DbCommandLoggerExtensionsBenchmarks.Dictionary_DisabledLogger_FastPath",
            "value": 0,
            "unit": "ns",
            "range": "± 0"
          },
          {
            "name": "Wolfgang.Extensions.Logging.Data.Benchmarks.DbCommandLoggerExtensionsBenchmarks.AnonymousObject_DisabledLogger_FastPath",
            "value": 99.04685320456822,
            "unit": "ns",
            "range": "± 0.24453938973433587"
          },
          {
            "name": "Wolfgang.Extensions.Logging.Data.Benchmarks.DbConnectionLoggerExtensionsBenchmarks.LogDbConnection_DisabledLogger_FastPath",
            "value": 0.0003577092041571935,
            "unit": "ns",
            "range": "± 0.0004992603861511299"
          },
          {
            "name": "Wolfgang.Extensions.Logging.Data.Benchmarks.DbConnectionLoggerExtensionsBenchmarks.LogDbConnection_EnabledLogger_FullWork",
            "value": 2634.7170766194663,
            "unit": "ns",
            "range": "± 2.8997047827051388"
          },
          {
            "name": "Wolfgang.Extensions.Logging.Data.Benchmarks.DbConnectionLoggerExtensionsBenchmarks.LogDbConnection_EnabledLogger_ExplicitDebugLevel",
            "value": 2621.0166651407876,
            "unit": "ns",
            "range": "± 9.860726193499469"
          }
        ]
      }
    ]
  }
}