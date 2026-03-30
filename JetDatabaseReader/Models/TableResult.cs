using System;
using System.Collections.Generic;

namespace JetDatabaseReader
{
    /// <summary>Unit of measurement for a <see cref="ColumnSize"/> value.</summary>
    public enum ColumnSizeUnit
    {
        /// <summary>Size in bits (e.g., Yes/No stores 1 bit in the null mask).</summary>
        Bits,
        /// <summary>Size in bytes.</summary>
        Bytes,
        /// <summary>Maximum character count for text columns.</summary>
        Chars,
        /// <summary>Variable-length with no declared maximum.</summary>
        Variable,
        /// <summary>Large-value data stored on LVAL pages (MEMO / OLE).</summary>
        Lval
    }

    /// <summary>
    /// Structured column size: a numeric <see cref="Value"/> paired with a <see cref="ColumnSizeUnit"/>.
    /// Use <see cref="ToString"/> for a human-readable description.
    /// </summary>
    public readonly struct ColumnSize
    {
        /// <summary>Numeric count; <c>null</c> for <see cref="ColumnSizeUnit.Variable"/> and <see cref="ColumnSizeUnit.Lval"/>.</summary>
        public int? Value { get; }

        /// <summary>The unit in which <see cref="Value"/> is expressed.</summary>
        public ColumnSizeUnit Unit { get; }

        private ColumnSize(int? value, ColumnSizeUnit unit) { Value = value; Unit = unit; }

        /// <summary>Creates a fixed size expressed in bits.</summary>
        public static ColumnSize FromBits(int count)  => new ColumnSize(count, ColumnSizeUnit.Bits);

        /// <summary>Creates a fixed size expressed in bytes.</summary>
        public static ColumnSize FromBytes(int count) => new ColumnSize(count, ColumnSizeUnit.Bytes);

        /// <summary>Creates a maximum character count for a text column.</summary>
        public static ColumnSize FromChars(int count) => new ColumnSize(count, ColumnSizeUnit.Chars);

        /// <summary>Variable-length with no declared maximum.</summary>
        public static readonly ColumnSize Variable = new ColumnSize(null, ColumnSizeUnit.Variable);

        /// <summary>Large-value data stored on LVAL pages (MEMO / OLE).</summary>
        public static readonly ColumnSize Lval = new ColumnSize(null, ColumnSizeUnit.Lval);

        /// <inheritdoc/>
        public override string ToString()
        {
            switch (Unit)
            {
                case ColumnSizeUnit.Bits:     return Value == 1 ? "1 bit"  : $"{Value} bits";
                case ColumnSizeUnit.Bytes:    return Value == 1 ? "1 byte" : $"{Value} bytes";
                case ColumnSizeUnit.Chars:    return $"{Value} chars";
                case ColumnSizeUnit.Variable: return "variable";
                case ColumnSizeUnit.Lval:     return "LVAL";
                default:                      return string.Empty;
            }
        }
    }

    /// <summary>
    /// Schema entry for a single column in a <see cref="TableResult"/>.
    /// </summary>
    public sealed class TableColumn
    {
        /// <summary>Column name.</summary>
        public string Name { get; set; }

        /// <summary>CLR type that best represents this column (e.g., <see cref="string"/>, <see cref="int"/>, <see cref="DateTime"/>).</summary>
        public Type Type { get; set; }

        /// <summary>Structured size — use <see cref="ColumnSize.ToString"/> for a human-readable description.</summary>
        public ColumnSize Size { get; set; }
    }

    /// <summary>
    /// Base result type for table read operations.
    /// Contains column headers, sampled rows (as strings), per-column schema,
    /// and the name of the table that was read.
    /// </summary>
    public class TableResult
    {
        /// <summary>Ordered list of column names.</summary>
        public List<string> Headers { get; set; }

        /// <summary>Up to <c>maxRows</c> rows, each row a list of string values (one per column).</summary>
        public List<List<string>> Rows { get; set; }

        /// <summary>Per-column schema information in the same order as <see cref="Headers"/>.</summary>
        public List<TableColumn> Schema { get; set; }

        /// <summary>Name of the table this result was read from.</summary>
        public string TableName { get; set; }

        /// <summary>Total number of rows in the result.</summary>
        public int RowCount => Rows?.Count ?? 0;
    }
}
