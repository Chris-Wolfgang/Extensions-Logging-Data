using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;

namespace Wolfgang.Extensions.Logging.Data.AotSmoke;

/// <summary>Counts log entries so the smoke can assert no call silently no-ops.</summary>
internal sealed class CountingLogger : ILogger
{
    public int Count { get; private set; }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

    public bool IsEnabled(LogLevel logLevel) => true;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        _ = formatter(state, exception);   // render the template so any format break surfaces
        Count++;
    }
}


/// <summary>Minimal in-memory <see cref="DbConnection"/> — no provider, so the smoke's trim/AOT result is attributable to the library alone.</summary>
internal sealed class FakeDbConnection : DbConnection
{
    [AllowNull]
    [SuppressMessage("Major Code Smell", "S2068:Hard-coded credentials are security-sensitive",
        Justification = "Fake connection string in an AOT smoke harness — not a real credential; it deliberately includes Password= so LogDbConnection's redaction is exercised.")]
    public override string ConnectionString { get; set; } = "Server=localhost;Database=Test;User Id=sa;Password=secret";

    public override string Database => "Test";

    public override string DataSource => "localhost";

    public override string ServerVersion => "1.0";

    public override ConnectionState State => ConnectionState.Closed;

    public override void ChangeDatabase(string databaseName) { }

    public override void Close() { }

    public override void Open() { }

    protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel) => throw new NotSupportedException();

    protected override DbCommand CreateDbCommand() => new FakeDbCommand { Connection = this };
}


internal sealed class FakeDbCommand : DbCommand
{
    private readonly FakeParameterCollection _parameters = new();

    [AllowNull]
    public override string CommandText { get; set; } = string.Empty;

    public override int CommandTimeout { get; set; }

    public override CommandType CommandType { get; set; }

    public override bool DesignTimeVisible { get; set; }

    public override UpdateRowSource UpdatedRowSource { get; set; }

    protected override DbConnection? DbConnection { get; set; }

    protected override DbParameterCollection DbParameterCollection => _parameters;

    protected override DbTransaction? DbTransaction { get; set; }

    public override void Cancel() { }

    public override int ExecuteNonQuery() => throw new NotSupportedException();

    public override object? ExecuteScalar() => throw new NotSupportedException();

    public override void Prepare() { }

    protected override DbParameter CreateDbParameter() => new FakeParameter();

    protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior) => throw new NotSupportedException();
}


internal sealed class FakeParameter : DbParameter
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
    public override object Value { get; set; }

    public override void ResetDbType() { }
}


internal sealed class FakeParameterCollection : DbParameterCollection
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
