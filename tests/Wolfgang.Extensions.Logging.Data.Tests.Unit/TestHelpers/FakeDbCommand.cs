using System;
using System.Data;
using System.Data.Common;

namespace Wolfgang.Extensions.Logging.Data.Tests.Unit.TestHelpers;

internal sealed class FakeDbCommand : DbCommand
{
    private readonly FakeDbParameterCollection _parameters = new FakeDbParameterCollection();

#pragma warning disable CS8765 // Base CommandText setter accepts null on net5+; coalesce instead.
    public override string CommandText
    {
        get => _commandText;
        set => _commandText = value ?? string.Empty;
    }
#pragma warning restore CS8765

    private string _commandText = string.Empty;

    public override int CommandTimeout { get; set; } = 30;

    public override CommandType CommandType { get; set; } = CommandType.Text;

    public override bool DesignTimeVisible { get; set; }

    public override UpdateRowSource UpdatedRowSource { get; set; }

    protected override DbConnection? DbConnection { get; set; }

    protected override DbParameterCollection DbParameterCollection => _parameters;

    protected override DbTransaction? DbTransaction { get; set; }

    public override void Cancel() => throw new NotSupportedException();

    public override int ExecuteNonQuery() => throw new NotSupportedException();

    public override object? ExecuteScalar() => throw new NotSupportedException();

    public override void Prepare() => throw new NotSupportedException();

    protected override DbParameter CreateDbParameter() => new FakeDbParameter();

    protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior) => throw new NotSupportedException();
}
