using System;
using System.Data;
using System.Data.Common;

namespace Wolfgang.Extensions.Logging.Data.Tests.Fuzz;

/// <summary>
/// A closed <see cref="DbConnection"/> whose connection string is stored verbatim, so the
/// fuzzer can hand <c>LogDbConnection</c> any string without a provider validating it first.
/// </summary>
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



    public override string Database => "fuzz";



    public override string DataSource => "fuzz-host";



    public override string ServerVersion => "1.0";



    public override ConnectionState State => ConnectionState.Closed;



    public override void ChangeDatabase(string databaseName) => throw new NotSupportedException();



    public override void Close() => throw new NotSupportedException();



    public override void Open() => throw new NotSupportedException();



    protected override DbCommand CreateDbCommand() => throw new NotSupportedException();



    protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel) => throw new NotSupportedException();
}
