using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using Wolfgang.Extensions.Logging.Data;

using var loggerFactory = LoggerFactory.Create(builder =>
{
    builder
        .SetMinimumLevel(LogLevel.Information)
        .AddSimpleConsole(options =>
        {
            options.SingleLine = true;
            options.IncludeScopes = false;
        });
});

ILogger logger = loggerFactory.CreateLogger("Demo");

// 1. LogDbConnection — set up a connection string that includes a Password.
//    Microsoft.Data.Sqlite with the e_sqlite3 bundle rejects Password at Open(),
//    so we log the closed connection here. That still demonstrates the redaction:
//    Password is removed from the logged ConnectionString while Data Source remains.
await using var connectionWithSecret = new SqliteConnection("Data Source=demo.db;Password=topsecret");
logger.LogDbConnection(connectionWithSecret);

// Now open a clean in-memory connection for the actual SQL operations and log it
// again — this time you can see ServerVersion, State=Open, etc.
await using var connection = new SqliteConnection("Data Source=:memory:");
await connection.OpenAsync();
logger.LogDbConnection(connection);

// 2. Set up a Users table and a parameterized INSERT.
await using (var create = connection.CreateCommand())
{
    create.CommandText = "CREATE TABLE Users (Id INTEGER PRIMARY KEY, Email TEXT, PasswordHash TEXT)";
    await create.ExecuteNonQueryAsync();
}

await using var insert = connection.CreateCommand();
insert.CommandText = "INSERT INTO Users (Email, PasswordHash) VALUES (@email, @password)";
var emailParam = insert.CreateParameter();
emailParam.ParameterName = "@email";
emailParam.Value = "alice@example.com";
insert.Parameters.Add(emailParam);

var passwordParam = insert.CreateParameter();
passwordParam.ParameterName = "@password";
passwordParam.Value = "hashed-secret-value";
insert.Parameters.Add(passwordParam);

// 3. LogDbCommand — every parameter is captured; only the @password value is redacted.
logger.LogDbCommand(insert, excludedParameterNames: new[] { "@password" });
await insert.ExecuteNonQueryAsync();

// 4. LogDbParameter — single-parameter view with explicit redaction toggle.
logger.LogDbParameter(passwordParam, redactValue: true);

// 5. LogDbParameterCollection — the whole collection in one entry, with the same
//    case-insensitive prefix-tolerant matching ("password" matches "@password").
logger.LogDbParameterCollection(insert.Parameters, excludedParameterNames: new[] { "password" });

// 6. LogCommandText — for code paths that have raw SQL + values but no DbCommand.
logger.LogCommandText(
    "UPDATE Users SET PasswordHash = @password WHERE Email = @email",
    new Dictionary<string, object?>
    {
        ["@email"] = "alice@example.com",
        ["@password"] = "new-hashed-secret"
    },
    excludedParameterNames: new[] { "@password" });
