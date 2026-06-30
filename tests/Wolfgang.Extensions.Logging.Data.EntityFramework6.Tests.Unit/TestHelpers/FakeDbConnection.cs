using System.Data;
using System.Data.Common;

namespace Wolfgang.Extensions.Logging.Data.EntityFramework6.Tests.Unit.TestHelpers;

/// <summary>
/// Minimal closed <see cref="DbConnection"/> test double — just enough for
/// <c>LogDbConnection</c> to read its benign properties without a live provider. It never
/// opens, so the <see cref="ServerVersion"/> path (guarded behind an Open check) is not hit.
/// </summary>
internal sealed class FakeDbConnection : DbConnection
{
#if NETCOREAPP3_0_OR_GREATER || NET5_0_OR_GREATER
    [System.Diagnostics.CodeAnalysis.AllowNull]
#endif
    public override string ConnectionString { get; set; } = "Server=localhost;Database=Test;User Id=sa;Password=secret";

    public override string Database => "Test";

    public override string DataSource => "localhost";

    public override string ServerVersion => "1.0";

    public override ConnectionState State => ConnectionState.Closed;

    public override void ChangeDatabase(string databaseName)
    {
    }

    public override void Close()
    {
    }

    public override void Open()
    {
    }

    protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel) => throw new System.NotSupportedException();

    protected override DbCommand CreateDbCommand() => throw new System.NotSupportedException();
}
