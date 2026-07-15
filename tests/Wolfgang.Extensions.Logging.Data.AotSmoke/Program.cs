// Native AOT / trimming smoke test for the AOT-safe public surface of
// Wolfgang.Extensions.Logging.Data. Published with PublishAot + PublishTrimmed
// (see the .csproj) and run by CI: a trim/AOT-unsafe regression makes the
// analyzers warn (TreatWarningsAsErrors → build fails), and a runtime break
// (MissingMethodException / NotSupportedException / silent no-op) makes this
// program exit non-zero.
//
// The anonymous-object LogCommandText overloads are DELIBERATELY not exercised
// here: they reflect over the parameter's runtime type and are marked
// [RequiresUnreferencedCode] — documented as not AOT/trim-safe. The
// IReadOnlyDictionary overloads are the AOT-safe equivalent and are covered.

using Microsoft.Extensions.Logging;
using Wolfgang.Extensions.Logging.Data;
using Wolfgang.Extensions.Logging.Data.AotSmoke;

var logger = new CountingLogger();

using var connection = new FakeDbConnection();
logger.LogDbConnection(connection);
logger.LogDbConnection(connection, LogLevel.Debug);

using var command = connection.CreateCommand();
command.CommandText = "SELECT * FROM Users WHERE Id = @id AND Pw = @password";
logger.LogDbCommand(command);
logger.LogDbCommand(command, LogLevel.Debug);
logger.LogDbCommand(command, new[] { "password" });
logger.LogDbCommand(command, new[] { "password" }, LogLevel.Debug);

var parameters = new Dictionary<string, object?> { ["@id"] = 1, ["@password"] = "secret" };
logger.LogCommandText("SELECT ...", parameters);
logger.LogCommandText("SELECT ...", parameters, LogLevel.Debug);
logger.LogCommandText("SELECT ...", parameters, new[] { "password" });
logger.LogCommandText("SELECT ...", parameters, new[] { "password" }, LogLevel.Debug);

// 10 AOT-safe calls above — each must produce exactly one log entry. A silent
// no-op (e.g. trimmed members) would drop the count.
const int expected = 10;
if (logger.Count != expected)
{
    System.Console.Error.WriteLine($"FAIL: expected {expected} log entries, got {logger.Count}.");
    return 1;
}

System.Console.WriteLine($"OK: AOT-safe surface produced {logger.Count} log entries under Native AOT.");
return 0;
