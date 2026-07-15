window.BENCHMARK_DATA = {
  "lastUpdate": 1784157869877,
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
          "id": "10b174b501b007ad5f2efa448e6c2ba23532920c",
          "message": "Merge pull request #137 from Chris-Wolfgang/fix/publicapi-nullability-markers\n\nNormalize PublicAPI.Shipped.txt to canonical nullable-annotated format",
          "timestamp": "2026-06-27T13:37:34-04:00",
          "tree_id": "3182d29478e684f421e2f0e9752d20bb4bce97c6",
          "url": "https://github.com/Chris-Wolfgang/Extensions-Logging-Data/commit/10b174b501b007ad5f2efa448e6c2ba23532920c"
        },
        "date": 1782581995044,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "Wolfgang.Extensions.Logging.Data.Benchmarks.DbCommandLoggerExtensionsBenchmarks.Dictionary_EnabledLogger",
            "value": 356.4296743075053,
            "unit": "ns",
            "range": "± 2.654285275938107"
          },
          {
            "name": "Wolfgang.Extensions.Logging.Data.Benchmarks.DbCommandLoggerExtensionsBenchmarks.AnonymousObject_EnabledLogger",
            "value": 496.4018910725911,
            "unit": "ns",
            "range": "± 3.7612346167994994"
          },
          {
            "name": "Wolfgang.Extensions.Logging.Data.Benchmarks.DbCommandLoggerExtensionsBenchmarks.Dictionary_DisabledLogger_FastPath",
            "value": 0.0016317994644244511,
            "unit": "ns",
            "range": "± 0.002826359580146832"
          },
          {
            "name": "Wolfgang.Extensions.Logging.Data.Benchmarks.DbCommandLoggerExtensionsBenchmarks.AnonymousObject_DisabledLogger_FastPath",
            "value": 114.55776145060857,
            "unit": "ns",
            "range": "± 1.387085974163986"
          },
          {
            "name": "Wolfgang.Extensions.Logging.Data.Benchmarks.DbConnectionLoggerExtensionsBenchmarks.LogDbConnection_DisabledLogger_FastPath",
            "value": 0.0038162656128406525,
            "unit": "ns",
            "range": "± 0.005795059915733007"
          },
          {
            "name": "Wolfgang.Extensions.Logging.Data.Benchmarks.DbConnectionLoggerExtensionsBenchmarks.LogDbConnection_EnabledLogger_FullWork",
            "value": 3290.002187093099,
            "unit": "ns",
            "range": "± 44.8591589485181"
          },
          {
            "name": "Wolfgang.Extensions.Logging.Data.Benchmarks.DbConnectionLoggerExtensionsBenchmarks.LogDbConnection_EnabledLogger_ExplicitDebugLevel",
            "value": 3207.898770650228,
            "unit": "ns",
            "range": "± 21.807538345734105"
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
          "id": "66ebbba890e0597f060db8970df845b7126c9b79",
          "message": "Merge pull request #139 from Chris-Wolfgang/feature/66-logdbcommand-core\n\nAdd LogDbCommand(DbCommand) core overloads (#66)",
          "timestamp": "2026-06-29T21:16:25-04:00",
          "tree_id": "eaf2ad462733c24e15fb71778fb63ffd8cc21a41",
          "url": "https://github.com/Chris-Wolfgang/Extensions-Logging-Data/commit/66ebbba890e0597f060db8970df845b7126c9b79"
        },
        "date": 1782782327358,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "Wolfgang.Extensions.Logging.Data.Benchmarks.DbCommandLoggerExtensionsBenchmarks.Dictionary_EnabledLogger",
            "value": 397.4641450246175,
            "unit": "ns",
            "range": "± 5.4862121100694425"
          },
          {
            "name": "Wolfgang.Extensions.Logging.Data.Benchmarks.DbCommandLoggerExtensionsBenchmarks.AnonymousObject_EnabledLogger",
            "value": 603.9086303710938,
            "unit": "ns",
            "range": "± 9.373534930529804"
          },
          {
            "name": "Wolfgang.Extensions.Logging.Data.Benchmarks.DbCommandLoggerExtensionsBenchmarks.Dictionary_DisabledLogger_FastPath",
            "value": 0.0032913188139597573,
            "unit": "ns",
            "range": "± 0.004117804391206589"
          },
          {
            "name": "Wolfgang.Extensions.Logging.Data.Benchmarks.DbCommandLoggerExtensionsBenchmarks.AnonymousObject_DisabledLogger_FastPath",
            "value": 119.64937241872151,
            "unit": "ns",
            "range": "± 0.5722000905252596"
          },
          {
            "name": "Wolfgang.Extensions.Logging.Data.Benchmarks.DbConnectionLoggerExtensionsBenchmarks.LogDbConnection_DisabledLogger_FastPath",
            "value": 0.005893714105089505,
            "unit": "ns",
            "range": "± 0.009820080819897499"
          },
          {
            "name": "Wolfgang.Extensions.Logging.Data.Benchmarks.DbConnectionLoggerExtensionsBenchmarks.LogDbConnection_EnabledLogger_FullWork",
            "value": 3394.521251678467,
            "unit": "ns",
            "range": "± 14.652784499000141"
          },
          {
            "name": "Wolfgang.Extensions.Logging.Data.Benchmarks.DbConnectionLoggerExtensionsBenchmarks.LogDbConnection_EnabledLogger_ExplicitDebugLevel",
            "value": 3371.2649129231772,
            "unit": "ns",
            "range": "± 47.62641354298808"
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
          "id": "eff04a70e2e4501d3f59ab7158a0353da9a8e0d6",
          "message": "Merge pull request #144 from Chris-Wolfgang/feature/66-ef6-companion-package\n\nAdd EntityFramework6 companion package (#66)",
          "timestamp": "2026-06-29T21:54:45-04:00",
          "tree_id": "45bf220629800d52d821443ff72284b33123915b",
          "url": "https://github.com/Chris-Wolfgang/Extensions-Logging-Data/commit/eff04a70e2e4501d3f59ab7158a0353da9a8e0d6"
        },
        "date": 1782784630277,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "Wolfgang.Extensions.Logging.Data.Benchmarks.DbCommandLoggerExtensionsBenchmarks.Dictionary_EnabledLogger",
            "value": 318.5572196642558,
            "unit": "ns",
            "range": "± 0.8709735277266544"
          },
          {
            "name": "Wolfgang.Extensions.Logging.Data.Benchmarks.DbCommandLoggerExtensionsBenchmarks.AnonymousObject_EnabledLogger",
            "value": 451.171391805013,
            "unit": "ns",
            "range": "± 3.9478320621481484"
          },
          {
            "name": "Wolfgang.Extensions.Logging.Data.Benchmarks.DbCommandLoggerExtensionsBenchmarks.Dictionary_DisabledLogger_FastPath",
            "value": 0,
            "unit": "ns",
            "range": "± 0"
          },
          {
            "name": "Wolfgang.Extensions.Logging.Data.Benchmarks.DbCommandLoggerExtensionsBenchmarks.AnonymousObject_DisabledLogger_FastPath",
            "value": 99.30735635757446,
            "unit": "ns",
            "range": "± 0.18953708564391464"
          },
          {
            "name": "Wolfgang.Extensions.Logging.Data.Benchmarks.DbConnectionLoggerExtensionsBenchmarks.LogDbConnection_DisabledLogger_FastPath",
            "value": 0.0008524972945451736,
            "unit": "ns",
            "range": "± 0.001476568627467251"
          },
          {
            "name": "Wolfgang.Extensions.Logging.Data.Benchmarks.DbConnectionLoggerExtensionsBenchmarks.LogDbConnection_EnabledLogger_FullWork",
            "value": 2626.714157104492,
            "unit": "ns",
            "range": "± 15.849884100001495"
          },
          {
            "name": "Wolfgang.Extensions.Logging.Data.Benchmarks.DbConnectionLoggerExtensionsBenchmarks.LogDbConnection_EnabledLogger_ExplicitDebugLevel",
            "value": 2766.455046335856,
            "unit": "ns",
            "range": "± 56.38406128104131"
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
          "id": "ad652de429d56a302cac54d08b3e21b67a10b8a9",
          "message": "Merge pull request #148 from Chris-Wolfgang/chore/release-v0.2.0\n\nchore: release v0.2.0 (MINOR)",
          "timestamp": "2026-07-11T21:06:11-04:00",
          "tree_id": "154a9b442cd68da3d0bacae0fa33d820447185be",
          "url": "https://github.com/Chris-Wolfgang/Extensions-Logging-Data/commit/ad652de429d56a302cac54d08b3e21b67a10b8a9"
        },
        "date": 1783818484861,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "Wolfgang.Extensions.Logging.Data.Benchmarks.DbCommandLoggerExtensionsBenchmarks.Dictionary_EnabledLogger",
            "value": 378.01774470011395,
            "unit": "ns",
            "range": "± 3.871914076607768"
          },
          {
            "name": "Wolfgang.Extensions.Logging.Data.Benchmarks.DbCommandLoggerExtensionsBenchmarks.AnonymousObject_EnabledLogger",
            "value": 580.3050012588501,
            "unit": "ns",
            "range": "± 3.316498060160203"
          },
          {
            "name": "Wolfgang.Extensions.Logging.Data.Benchmarks.DbCommandLoggerExtensionsBenchmarks.Dictionary_DisabledLogger_FastPath",
            "value": 0,
            "unit": "ns",
            "range": "± 0"
          },
          {
            "name": "Wolfgang.Extensions.Logging.Data.Benchmarks.DbCommandLoggerExtensionsBenchmarks.AnonymousObject_DisabledLogger_FastPath",
            "value": 111.6205021540324,
            "unit": "ns",
            "range": "± 0.5552590212953594"
          },
          {
            "name": "Wolfgang.Extensions.Logging.Data.Benchmarks.DbConnectionLoggerExtensionsBenchmarks.LogDbConnection_DisabledLogger_FastPath",
            "value": 2.182315415392319,
            "unit": "ns",
            "range": "± 0.002236261294612084"
          },
          {
            "name": "Wolfgang.Extensions.Logging.Data.Benchmarks.DbConnectionLoggerExtensionsBenchmarks.LogDbConnection_EnabledLogger_FullWork",
            "value": 3323.369041442871,
            "unit": "ns",
            "range": "± 13.002670619668844"
          },
          {
            "name": "Wolfgang.Extensions.Logging.Data.Benchmarks.DbConnectionLoggerExtensionsBenchmarks.LogDbConnection_EnabledLogger_ExplicitDebugLevel",
            "value": 3319.399656931559,
            "unit": "ns",
            "range": "± 11.786671376180793"
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
          "id": "43af6a178f3a9446ddb7f14c3c93bf1f7429d86b",
          "message": "Merge pull request #166 from Chris-Wolfgang/release/0.3.0-nonprotected\n\n0.3.0: Trim/AOT (#94) + maintenance docs + version bump (non-protected)",
          "timestamp": "2026-07-14T20:48:12-04:00",
          "tree_id": "ea6230e33efdfd9c8f4c7241dd3483eecd55bad5",
          "url": "https://github.com/Chris-Wolfgang/Extensions-Logging-Data/commit/43af6a178f3a9446ddb7f14c3c93bf1f7429d86b"
        },
        "date": 1784076634084,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "Wolfgang.Extensions.Logging.Data.Benchmarks.DbCommandLoggerExtensionsBenchmarks.Dictionary_EnabledLogger",
            "value": 348.24335257212323,
            "unit": "ns",
            "range": "± 4.353644880305523"
          },
          {
            "name": "Wolfgang.Extensions.Logging.Data.Benchmarks.DbCommandLoggerExtensionsBenchmarks.AnonymousObject_EnabledLogger",
            "value": 491.0420509974162,
            "unit": "ns",
            "range": "± 4.655838676988648"
          },
          {
            "name": "Wolfgang.Extensions.Logging.Data.Benchmarks.DbCommandLoggerExtensionsBenchmarks.Dictionary_DisabledLogger_FastPath",
            "value": 0.0005734767764806747,
            "unit": "ns",
            "range": "± 0.0009932909138253491"
          },
          {
            "name": "Wolfgang.Extensions.Logging.Data.Benchmarks.DbCommandLoggerExtensionsBenchmarks.AnonymousObject_DisabledLogger_FastPath",
            "value": 110.29211346308391,
            "unit": "ns",
            "range": "± 1.1893787489236889"
          },
          {
            "name": "Wolfgang.Extensions.Logging.Data.Benchmarks.DbConnectionLoggerExtensionsBenchmarks.LogDbConnection_DisabledLogger_FastPath",
            "value": 0.00008095862964789073,
            "unit": "ns",
            "range": "± 0.00014022445986129877"
          },
          {
            "name": "Wolfgang.Extensions.Logging.Data.Benchmarks.DbConnectionLoggerExtensionsBenchmarks.LogDbConnection_EnabledLogger_FullWork",
            "value": 3241.0740826924643,
            "unit": "ns",
            "range": "± 11.998071736190154"
          },
          {
            "name": "Wolfgang.Extensions.Logging.Data.Benchmarks.DbConnectionLoggerExtensionsBenchmarks.LogDbConnection_EnabledLogger_ExplicitDebugLevel",
            "value": 3283.3917503356934,
            "unit": "ns",
            "range": "± 18.139511169180793"
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
          "id": "e47e699e51990c242b2694840fc8818da9966452",
          "message": "Merge pull request #169 from Chris-Wolfgang/maint/inspectcode-noise-floor\n\nci(inspectcode): .DotSettings noise-floor + remove redundant usings (#136)",
          "timestamp": "2026-07-15T19:22:24-04:00",
          "tree_id": "875b13eb10ac699732ecbf848532be888458b3ac",
          "url": "https://github.com/Chris-Wolfgang/Extensions-Logging-Data/commit/e47e699e51990c242b2694840fc8818da9966452"
        },
        "date": 1784157867994,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "Wolfgang.Extensions.Logging.Data.Benchmarks.DbCommandLoggerExtensionsBenchmarks.Dictionary_EnabledLogger",
            "value": 248.88143030802408,
            "unit": "ns",
            "range": "± 1.6784459975786297"
          },
          {
            "name": "Wolfgang.Extensions.Logging.Data.Benchmarks.DbCommandLoggerExtensionsBenchmarks.AnonymousObject_EnabledLogger",
            "value": 346.5148623784383,
            "unit": "ns",
            "range": "± 1.7838670265199825"
          },
          {
            "name": "Wolfgang.Extensions.Logging.Data.Benchmarks.DbCommandLoggerExtensionsBenchmarks.Dictionary_DisabledLogger_FastPath",
            "value": 0.005593659356236458,
            "unit": "ns",
            "range": "± 0.008978847601137242"
          },
          {
            "name": "Wolfgang.Extensions.Logging.Data.Benchmarks.DbCommandLoggerExtensionsBenchmarks.AnonymousObject_DisabledLogger_FastPath",
            "value": 80.92288784186046,
            "unit": "ns",
            "range": "± 0.6105164127015656"
          },
          {
            "name": "Wolfgang.Extensions.Logging.Data.Benchmarks.DbConnectionLoggerExtensionsBenchmarks.LogDbConnection_DisabledLogger_FastPath",
            "value": 0,
            "unit": "ns",
            "range": "± 0"
          },
          {
            "name": "Wolfgang.Extensions.Logging.Data.Benchmarks.DbConnectionLoggerExtensionsBenchmarks.LogDbConnection_EnabledLogger_FullWork",
            "value": 2015.7692540486653,
            "unit": "ns",
            "range": "± 13.338181315191754"
          },
          {
            "name": "Wolfgang.Extensions.Logging.Data.Benchmarks.DbConnectionLoggerExtensionsBenchmarks.LogDbConnection_EnabledLogger_ExplicitDebugLevel",
            "value": 2132.038159688314,
            "unit": "ns",
            "range": "± 12.3738940279496"
          }
        ]
      }
    ]
  }
}