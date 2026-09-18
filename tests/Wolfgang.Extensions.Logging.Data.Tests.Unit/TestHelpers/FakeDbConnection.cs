using System.Data;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;

namespace Wolfgang.Extensions.Logging.Data.Tests.Unit.TestHelpers;

// Abstract-base test double: DbCommand/DbParameter/DbParameterCollection/DbConnection
// force ~30 overrides, most of which the logging path under test never reaches. No
// product logic lives here, so the class is excluded from the instrumented test
// assembly rather than carrying an attribute per forced member.
[ExcludeFromCodeCoverage]
internal sealed class FakeDbConnection : DbConnection
{
    private string _connectionString = string.Empty;

#pragma warning disable CS8765 // Base ConnectionString uses [AllowNull] on the setter on net5+; accept null and coalesce.
    public override string ConnectionString
    {
        get => _connectionString;
        set => _connectionString = value ?? string.Empty;
    }
#pragma warning restore CS8765

    public string DatabaseValue { get; set; } = string.Empty;

    public string DataSourceValue { get; set; } = string.Empty;

    public string ServerVersionValue { get; set; } = string.Empty;

    public ConnectionState StateValue { get; set; } = ConnectionState.Closed;

    public int ConnectionTimeoutValue { get; set; } = 30;

    public Exception? ServerVersionException { get; set; }

    public override string Database => DatabaseValue;

    public override string DataSource => DataSourceValue;

    public override string ServerVersion
    {
        get
        {
            if (ServerVersionException is not null)
            {
                throw ServerVersionException;
            }

            return ServerVersionValue;
        }
    }

    public override ConnectionState State => StateValue;

    public override int ConnectionTimeout => ConnectionTimeoutValue;

    public override void ChangeDatabase(string databaseName) => DatabaseValue = databaseName;

    public override void Close() => StateValue = ConnectionState.Closed;

    public override void Open() => StateValue = ConnectionState.Open;

    protected override DbCommand CreateDbCommand() => throw new NotSupportedException();

    protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel) => throw new NotSupportedException();
}
