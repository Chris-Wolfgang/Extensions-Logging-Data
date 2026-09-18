using System;
using System.Data;
using System.Data.Common;
using System.Linq;
using Wolfgang.Extensions.Logging.Data.EntityFramework6.Tests.Unit.TestHelpers;

namespace Wolfgang.Extensions.Logging.Data.EntityFramework6.Tests.Unit;

/// <summary>
/// Contract tests for the <see cref="DbCommand"/> / <see cref="DbConnection"/> test doubles:
/// the members the logging path reads round-trip, the rest of the abstract surface either
/// no-ops or throws <see cref="NotSupportedException"/> so a test that wanders off the
/// supported path fails loudly instead of silently succeeding.
/// </summary>
public class FakeDbDoublesTests
{
    [Fact]
    public void FakeDbCommand_settable_members_round_trip()
    {
        using var connection = new FakeDbConnection();
        using var command = new FakeDbCommand
        {
            CommandText = "SELECT 1",
            CommandTimeout = 42,
            CommandType = CommandType.StoredProcedure,
            DesignTimeVisible = true,
            UpdatedRowSource = UpdateRowSource.OutputParameters,
            Connection = connection,
            Transaction = null,
        };

        Assert.Equal("SELECT 1", command.CommandText);
        Assert.Equal(42, command.CommandTimeout);
        Assert.Equal(CommandType.StoredProcedure, command.CommandType);
        Assert.True(command.DesignTimeVisible);
        Assert.Equal(UpdateRowSource.OutputParameters, command.UpdatedRowSource);
        Assert.Same(connection, command.Connection);
        Assert.Null(command.Transaction);
    }



    [Fact]
    public void FakeDbCommand_execution_surface_throws_NotSupportedException()
    {
        using var command = new FakeDbCommand();

        command.Cancel();
        command.Prepare();
        Assert.Throws<NotSupportedException>(() => command.ExecuteNonQuery());
        Assert.Throws<NotSupportedException>(() => command.ExecuteScalar());
        Assert.Throws<NotSupportedException>(() => command.ExecuteReader());
    }



    [Fact]
    public void FakeDbParameter_settable_members_round_trip()
    {
        using var command = new FakeDbCommand();
        var parameter = command.CreateParameter();
        parameter.ParameterName = "@p";
        parameter.Value = 1;
        parameter.DbType = DbType.Int32;
        parameter.Direction = ParameterDirection.Output;
        parameter.IsNullable = true;
        parameter.Size = 4;
        parameter.SourceColumn = "col";
        parameter.SourceColumnNullMapping = true;
        parameter.ResetDbType();

        Assert.IsType<FakeDbParameter>(parameter);
        Assert.Equal("@p", parameter.ParameterName);
        Assert.Equal(1, parameter.Value);
        Assert.Equal(DbType.Int32, parameter.DbType);
        Assert.Equal(ParameterDirection.Output, parameter.Direction);
        Assert.True(parameter.IsNullable);
        Assert.Equal(4, parameter.Size);
        Assert.Equal("col", parameter.SourceColumn);
        Assert.True(parameter.SourceColumnNullMapping);
    }



    [Fact]
    public void FakeDbParameterCollection_supports_add_count_index_enumerate_and_clear()
    {
        using var command = new FakeDbCommand();
        command.AddParameter("@a", 1);
        command.AddParameter("@b", null);
        var parameters = command.Parameters;

        Assert.Equal(2, parameters.Count);
        Assert.Equal("@a", parameters[0].ParameterName);
        Assert.NotNull(parameters.SyncRoot);
        Assert.Equal(2, parameters.Cast<DbParameter>().Count());

        parameters.Clear();

        Assert.Equal(0, parameters.Count);
    }



    [Fact]
    public void FakeDbParameterCollection_unsupported_members_throw_NotSupportedException()
    {
        using var command = new FakeDbCommand();
        command.AddParameter("@a", 1);
        var parameters = command.Parameters;
        var extra = new FakeDbParameter { ParameterName = "@z" };

        Assert.Throws<NotSupportedException>(() => parameters["@a"]);
        Assert.Throws<NotSupportedException>(() => parameters["@a"] = extra);
        Assert.Throws<NotSupportedException>(() => parameters[0] = extra);
        Assert.Throws<NotSupportedException>(() => parameters.AddRange(new[] { extra }));
        Assert.Throws<NotSupportedException>(() => parameters.Contains(extra));
        Assert.Throws<NotSupportedException>(() => parameters.Contains("@a"));
        Assert.Throws<NotSupportedException>(() => parameters.CopyTo(new DbParameter[1], 0));
        Assert.Throws<NotSupportedException>(() => parameters.IndexOf(extra));
        Assert.Throws<NotSupportedException>(() => parameters.IndexOf("@a"));
        Assert.Throws<NotSupportedException>(() => parameters.Insert(0, extra));
        Assert.Throws<NotSupportedException>(() => parameters.Remove(extra));
        Assert.Throws<NotSupportedException>(() => parameters.RemoveAt(0));
        Assert.Throws<NotSupportedException>(() => parameters.RemoveAt("@a"));
    }



    [Fact]
    public void FakeDbConnection_is_a_closed_benign_connection_and_never_opens()
    {
        using var connection = new FakeDbConnection();

        Assert.Equal("Test", connection.Database);
        Assert.Equal("localhost", connection.DataSource);
        Assert.Equal("1.0", connection.ServerVersion);
        Assert.Equal(ConnectionState.Closed, connection.State);

        connection.Open();
        connection.ChangeDatabase("Other");
        connection.Close();
        Assert.Equal(ConnectionState.Closed, connection.State);
        Assert.Equal("Test", connection.Database);

        Assert.Throws<NotSupportedException>(() => connection.CreateCommand());
        Assert.Throws<NotSupportedException>(() => connection.BeginTransaction());
    }
}
