using System.Data;
using System.Data.Common;

namespace Wolfgang.Extensions.Logging.Data.Tests.Unit.TestHelpers;

internal sealed class FakeDbParameter : DbParameter
{
    public override DbType DbType { get; set; }

    public override ParameterDirection Direction { get; set; } = ParameterDirection.Input;

    public override bool IsNullable { get; set; }

#pragma warning disable CS8765 // Base ParameterName setter accepts null on net5+; coalesce instead.
    public override string ParameterName
    {
        get => _parameterName;
        set => _parameterName = value ?? string.Empty;
    }
#pragma warning restore CS8765

    private string _parameterName = string.Empty;

    public override int Size { get; set; }

#pragma warning disable CS8765 // Base SourceColumn setter accepts null on net5+; coalesce instead.
    public override string SourceColumn
    {
        get => _sourceColumn;
        set => _sourceColumn = value ?? string.Empty;
    }
#pragma warning restore CS8765

    private string _sourceColumn = string.Empty;

    public override bool SourceColumnNullMapping { get; set; }

    public override DataRowVersion SourceVersion { get; set; } = DataRowVersion.Current;

    public override object? Value { get; set; }

    public override byte Precision { get; set; }

    public override byte Scale { get; set; }

    public override void ResetDbType() => DbType = DbType.Object;
}
