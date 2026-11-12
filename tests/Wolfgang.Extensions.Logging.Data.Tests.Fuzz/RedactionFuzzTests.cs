using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using CsCheck;
using Xunit;

namespace Wolfgang.Extensions.Logging.Data.Tests.Fuzz;

/// <summary>
/// Property-based fuzz tests over the library's redaction guarantees, using CsCheck. The
/// load-bearing invariant is that a secret never reaches the log: a connection-string password
/// and the value of an excluded command parameter must be redacted whatever the surrounding
/// input looks like, and a logging call must never throw on the caller's data.
/// The case count is <c>FUZZ_ITER</c> (default 500 for the per-PR run); the scheduled
/// fuzz.yaml sets it far higher for a deep sweep. On failure CsCheck shrinks to a minimal
/// input and prints a replayable seed.
/// </summary>
public class RedactionFuzzTests
{
    // Ordinary keys and values are lowercase letters only, and a secret is "ZQX" followed by
    // uppercase letters and digits, so a secret cannot occur anywhere else in the input by
    // accident: finding it in the output means it leaked.
    private static readonly Gen<string> LowerWord = Gen.String[Gen.Char['a', 'z'], 1, 10];

    private static readonly Gen<string> Secret =
        Gen.String[Gen.OneOf(Gen.Char['A', 'Z'], Gen.Char['0', '9']), 6, 20]
            .Select(s => "ZQX" + s);

    private static readonly Gen<string> PasswordKey =
        Gen.OneOfConst("Password", "password", "PASSWORD", "pAsSwOrD", "Pwd", "pwd", "PWD");

    private static readonly Gen<(string Key, string Value)> OrdinaryPair =
        Gen.Select
        (
            LowerWord.Where(k => !string.Equals(k, "password", StringComparison.Ordinal) && !string.Equals(k, "pwd", StringComparison.Ordinal)),
            Gen.String[Gen.Char['a', 'z'], 0, 10],
            (key, value) => (key, value)
        );

    private static readonly Gen<string> ParameterPrefix = Gen.OneOfConst("", "@", ":", "?");



    private static long Iterations =>
        long.TryParse(Environment.GetEnvironmentVariable("FUZZ_ITER"), NumberStyles.Integer, CultureInfo.InvariantCulture, out var n) && n > 0
            ? n
            : 500;



    [Fact]
    public void LogDbConnection_never_logs_the_password_of_a_well_formed_connection_string()
    {
        Gen
            .Select
            (
                OrdinaryPair.Array[0, 8],
                PasswordKey,
                Secret,
                Gen.Int[0, 8]
            )
            .Sample
            (
                (pairs, passwordKey, secret, position) =>
                {
                    var parts = pairs
                        .Select(p => p.Key + "=" + p.Value)
                        .ToList();
                    parts.Insert(Math.Min(position, parts.Count), passwordKey + "=" + secret);

                    var logger = new CapturingLogger();
                    using var connection = new FakeDbConnection { ConnectionString = string.Join(";", parts) };

                    logger.LogDbConnection(connection);

                    return logger.Entries.Count == 1
                        && logger.Value(0, "ConnectionString").IndexOf(secret, StringComparison.Ordinal) < 0;
                },
                iter: Iterations
            );
    }



    [Fact]
    public void LogDbConnection_never_throws_on_an_arbitrary_connection_string()
    {
        Gen.String[0, 200]
            .Sample
            (
                connectionString =>
                {
                    var logger = new CapturingLogger();
                    using var connection = new FakeDbConnection { ConnectionString = connectionString };

                    logger.LogDbConnection(connection);

                    return logger.Entries.Count == 1;
                },
                iter: Iterations
            );
    }



    [Fact]
    public void LogCommandText_redacts_every_excluded_parameter_and_only_those()
    {
        Gen
            .Select
            (
                LowerWord.Array[1, 10].Select(names => names.Distinct(StringComparer.Ordinal).ToArray()),
                Gen.Bool.Array[10],
                ParameterPrefix.Array[10],
                ParameterPrefix.Array[10],
                Gen.Bool.Array[10]
            )
            .Sample
            (
                (names, excludedFlags, keyPrefixes, excludedPrefixes, upperCase) =>
                {
                    var parameters = new Dictionary<string, object?>(StringComparer.Ordinal);
                    var excluded = new List<string>();
                    for (var i = 0; i < names.Length; i++)
                    {
                        parameters[keyPrefixes[i] + names[i]] = ValueFor(i);
                        if (excludedFlags[i])
                        {
                            excluded.Add(excludedPrefixes[i] + (upperCase[i] ? names[i].ToUpperInvariant() : names[i]));
                        }
                    }

                    var logger = new CapturingLogger();

                    logger.LogCommandText("SELECT 1", parameters, excluded);

                    var rendered = logger.Value(0, "Parameters");
                    var allMatch = Enumerable
                        .Range(0, names.Length)
                        .All
                        (
                            i => excludedFlags[i]
                                ? rendered.IndexOf(keyPrefixes[i] + names[i] + "=***", StringComparison.Ordinal) >= 0
                                    && rendered.IndexOf("=" + ValueFor(i) + ",", StringComparison.Ordinal) < 0
                                    && rendered.IndexOf("=" + ValueFor(i) + "}", StringComparison.Ordinal) < 0
                                : rendered.IndexOf(keyPrefixes[i] + names[i] + "=" + ValueFor(i), StringComparison.Ordinal) >= 0
                        );

                    return allMatch && logger.Entries.Count == 1;
                },
                iter: Iterations
            );
    }



    private static string ValueFor(int index) => "V" + index.ToString(CultureInfo.InvariantCulture) + "Q";
}
