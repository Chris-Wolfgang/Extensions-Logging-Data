using System;
using System.Globalization;
using System.Linq;
using CsCheck;
using Xunit;
using FakeDbCommand = Wolfgang.Extensions.Logging.Data.Tests.Unit.TestHelpers.FakeDbCommand;

namespace Wolfgang.Extensions.Logging.Data.Tests.Fuzz;

/// <summary>
/// Property-based fuzz test for <see cref="DbCommandLoggerExtensions.LogDbCommand(Microsoft.Extensions.Logging.ILogger, System.Data.Common.DbCommand, System.Collections.Generic.IEnumerable{string})"/>:
/// the value of every excluded parameter of a real <see cref="System.Data.Common.DbCommand"/>
/// is redacted, whatever ADO.NET prefix or casing the parameter or the excluded name uses, and
/// every other value is logged. The case count follows <c>FUZZ_ITER</c>, like
/// <see cref="RedactionFuzzTests"/>.
/// </summary>
public class CommandRedactionFuzzTests
{
    private static readonly Gen<string> LowerWord = Gen.String[Gen.Char['a', 'z'], 1, 10];

    private static readonly Gen<string> ParameterPrefix = Gen.OneOfConst("", "@", ":", "?");



    private static long Iterations =>
        long.TryParse(Environment.GetEnvironmentVariable("FUZZ_ITER"), NumberStyles.Integer, CultureInfo.InvariantCulture, out var n) && n > 0
            ? n
            : 500;



    [Fact]
    public void LogDbCommand_redacts_every_excluded_parameter_and_only_those()
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
                (names, excludedFlags, parameterPrefixes, excludedPrefixes, upperCase) =>
                {
                    using var command = new FakeDbCommand();
                    command.CommandText = "SELECT 1";
                    var excluded = new System.Collections.Generic.List<string>();
                    for (var i = 0; i < names.Length; i++)
                    {
                        command.AddParameter(parameterPrefixes[i] + names[i], ValueFor(i));
                        if (excludedFlags[i])
                        {
                            excluded.Add(excludedPrefixes[i] + (upperCase[i] ? names[i].ToUpperInvariant() : names[i]));
                        }
                    }

                    var logger = new CapturingLogger();

                    logger.LogDbCommand(command, excluded);

                    var rendered = logger.Value(0, "Parameters");
                    var allMatch = Enumerable
                        .Range(0, names.Length)
                        .All
                        (
                            i => excludedFlags[i]
                                ? rendered.IndexOf(parameterPrefixes[i] + names[i] + "=***", StringComparison.Ordinal) >= 0
                                    && rendered.IndexOf(ValueFor(i), StringComparison.Ordinal) < 0
                                : rendered.IndexOf(parameterPrefixes[i] + names[i] + "=" + ValueFor(i), StringComparison.Ordinal) >= 0
                        );

                    return allMatch && logger.Entries.Count == 1;
                },
                iter: Iterations
            );
    }



    // "V<index>Q": the trailing Q keeps V1Q from being a substring of V10Q.
    private static string ValueFor(int index) => "V" + index.ToString(CultureInfo.InvariantCulture) + "Q";
}
