using System.Data;

namespace Wolfgang.Extensions.Logging.Data;

/// <summary>
/// Immutable snapshot of a <see cref="System.Data.Common.DbParameter"/> captured for structured logging.
/// </summary>
/// <remarks>
/// <see cref="Value"/> is set to the literal string <c>[REDACTED]</c> when the parameter's name matches
/// an entry in the caller-supplied excluded-names list passed to
/// <see cref="DbCommandLoggerExtensions"/>; all other metadata is preserved.
/// </remarks>
public sealed class LoggedDbParameter
{
    /// <summary>
    /// Initializes a new <see cref="LoggedDbParameter"/>.
    /// </summary>
    /// <param name="name">The parameter name (including any provider prefix such as <c>@</c>).</param>
    /// <param name="dbType">The <see cref="System.Data.DbType"/> of the parameter.</param>
    /// <param name="direction">The <see cref="ParameterDirection"/>.</param>
    /// <param name="size">The size in bytes (or characters for string types).</param>
    /// <param name="precision">The numeric precision.</param>
    /// <param name="scale">The numeric scale.</param>
    /// <param name="isNullable">Whether the parameter accepts <see langword="null"/>.</param>
    /// <param name="value">The parameter value, or the redaction marker if the value was redacted.</param>
    public LoggedDbParameter
    (
        string name,
        DbType dbType,
        ParameterDirection direction,
        int size,
        byte precision,
        byte scale,
        bool isNullable,
        object? value
    )
    {
        Name = name;
        DbType = dbType;
        Direction = direction;
        Size = size;
        Precision = precision;
        Scale = scale;
        IsNullable = isNullable;
        Value = value;
    }

    /// <summary>The parameter name as reported by the source <c>DbParameter</c>.</summary>
    public string Name { get; }

    /// <summary>The <see cref="System.Data.DbType"/> of the parameter.</summary>
    public DbType DbType { get; }

    /// <summary>The <see cref="ParameterDirection"/> (Input, Output, etc.).</summary>
    public ParameterDirection Direction { get; }

    /// <summary>The size in bytes (or characters for string types).</summary>
    public int Size { get; }

    /// <summary>The numeric precision.</summary>
    public byte Precision { get; }

    /// <summary>The numeric scale.</summary>
    public byte Scale { get; }

    /// <summary>Whether the parameter accepts <see langword="null"/>.</summary>
    public bool IsNullable { get; }

    /// <summary>The parameter value, or the literal <c>[REDACTED]</c> marker when redacted.</summary>
    public object? Value { get; }

    /// <inheritdoc />
    public override string ToString()
        => $"{Name}={Value ?? "null"} (DbType={DbType}, Direction={Direction}, Size={Size}, Precision={Precision}, Scale={Scale}, IsNullable={IsNullable})";
}
