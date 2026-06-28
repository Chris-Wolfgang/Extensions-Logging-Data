using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;

namespace Wolfgang.Extensions.Logging.Data.Tests.Unit.TestHelpers;

/// <summary>
/// Minimal <see cref="DbCommand"/> test double for exercising <c>LogDbCommand</c> on every
/// target framework (no live provider needed). Only the members <c>LogDbCommand</c> reads —
/// <see cref="CommandText"/> and <see cref="DbParameterCollection"/> — are functional; the
/// execution surface throws.
/// </summary>
internal sealed class FakeDbCommand : DbCommand
{
    private readonly FakeDbParameterCollection _parameters = new();

    [AllowNull]
    public override string CommandText { get; set; } = string.Empty;

    public override int CommandTimeout { get; set; }

    public override CommandType CommandType { get; set; }

    public override bool DesignTimeVisible { get; set; }

    public override UpdateRowSource UpdatedRowSource { get; set; }

    protected override DbConnection? DbConnection { get; set; }

    protected override DbParameterCollection DbParameterCollection => _parameters;

    protected override DbTransaction? DbTransaction { get; set; }

    public void AddParameter(string name, object? value)
    {
        _parameters.Add(new FakeDbParameter { ParameterName = name, Value = value });
    }

    public override void Cancel()
    {
    }

    public override int ExecuteNonQuery() => throw new NotSupportedException();

    public override object? ExecuteScalar() => throw new NotSupportedException();

    public override void Prepare()
    {
    }

    protected override DbParameter CreateDbParameter() => new FakeDbParameter();

    protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior) => throw new NotSupportedException();
}


internal sealed class FakeDbParameter : DbParameter
{
    public override DbType DbType { get; set; }

    public override ParameterDirection Direction { get; set; }

    public override bool IsNullable { get; set; }

    [AllowNull]
    public override string ParameterName { get; set; } = string.Empty;

    public override int Size { get; set; }

    [AllowNull]
    public override string SourceColumn { get; set; } = string.Empty;

    public override bool SourceColumnNullMapping { get; set; }

    [AllowNull]
    public override object? Value { get; set; }

    public override void ResetDbType()
    {
    }
}


/// <summary>
/// Backing collection for <see cref="FakeDbCommand"/>. Implements only <see cref="Count"/> and
/// enumeration (what <c>ToDictionary(DbParameterCollection)</c> uses) plus <c>Add</c>; the rest
/// throw, as they are never reached by the logging path under test.
/// </summary>
internal sealed class FakeDbParameterCollection : DbParameterCollection
{
    private readonly List<DbParameter> _items = new();

    public override int Count => _items.Count;

    public override object SyncRoot => _items;

    public override int Add(object value)
    {
        _items.Add((DbParameter)value);
        return _items.Count - 1;
    }

    public override IEnumerator GetEnumerator() => _items.GetEnumerator();

    protected override DbParameter GetParameter(int index) => _items[index];

    protected override DbParameter GetParameter(string parameterName) => throw new NotSupportedException();

    public override void AddRange(Array values) => throw new NotSupportedException();

    public override void Clear() => _items.Clear();

    public override bool Contains(object value) => throw new NotSupportedException();

    public override bool Contains(string value) => throw new NotSupportedException();

    public override void CopyTo(Array array, int index) => throw new NotSupportedException();

    public override int IndexOf(object value) => throw new NotSupportedException();

    public override int IndexOf(string parameterName) => throw new NotSupportedException();

    public override void Insert(int index, object value) => throw new NotSupportedException();

    public override void Remove(object value) => throw new NotSupportedException();

    public override void RemoveAt(int index) => throw new NotSupportedException();

    public override void RemoveAt(string parameterName) => throw new NotSupportedException();

    protected override void SetParameter(int index, DbParameter value) => throw new NotSupportedException();

    protected override void SetParameter(string parameterName, DbParameter value) => throw new NotSupportedException();
}
