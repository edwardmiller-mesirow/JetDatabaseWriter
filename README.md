# JetDatabaseReader

[![NuGet](https://img.shields.io/nuget/v/JetDatabaseReader.svg)](https://www.nuget.org/packages/JetDatabaseReader/)
[![Downloads](https://img.shields.io/nuget/dt/JetDatabaseReader.svg)](https://www.nuget.org/packages/JetDatabaseReader/)

Pure-managed .NET library for reading Microsoft Access JET databases without requiring OleDB, ODBC, or ACE drivers.

## Features

- ✅ **No Dependencies** - Pure C# implementation, no native drivers required
- ✅ **Jet3 & Jet4+** - Supports Access 97 (.mdb) through Access 2019 (.mdb/.accdb)
- ✅ **All Data Types** - Text, Integer, Date, GUID, Currency, MEMO, OLE Object
- ✅ **Streaming API** - Process millions of rows without out-of-memory errors
- ✅ **Page Caching** - 256-page LRU cache for 50%+ performance boost
- ✅ **Progress Reporting** - `IProgress<int>` callbacks for long operations
- ✅ **Multi-Language** - Auto-detects code page (Cyrillic, Japanese, etc.)
- ✅ **OLE Objects** - Auto-detects embedded images, PDFs, Office docs

## Installation

```bash
dotnet add package JetDatabaseReader
```

Or via Package Manager Console:

```powershell
Install-Package JetDatabaseReader
```

## Quick Start

```csharp
using JetDatabaseReader;

// Open database
using (var reader = new JetDatabaseReader("database.mdb"))
{
    // List all tables
    var tables = reader.ListTables();
    Console.WriteLine($"Found {tables.Count} tables");

    // Read a table (first 100 rows)
    var (headers, rows, schema) = reader.ReadTable("MyTable", maxRows: 100);
    foreach (var row in rows)
    {
        Console.WriteLine(string.Join(", ", row));
    }
}
```

## Usage Examples

### Stream Large Tables

```csharp
using (var reader = new JetDatabaseReader("large.mdb"))
{
    var progress = new Progress<int>(count => 
        Console.WriteLine($"Processed {count} rows"));

    foreach (var row in reader.StreamRows("BigTable", progress))
    {
        // Process one row at a time - no memory issues
        ProcessRow(row);
    }
}
```

### Read as DataTable

```csharp
using (var reader = new JetDatabaseReader("data.mdb"))
{
    DataTable dt = reader.ReadTableAsDataTable("Customers");
    
    // Bind to DataGridView, export to CSV, etc.
    dataGridView1.DataSource = dt;
}
```

### Get Table Statistics

```csharp
using (var reader = new JetDatabaseReader("db.mdb"))
{
    var stats = reader.GetTableStats();
    foreach (var (name, rowCount, columnCount) in stats)
    {
        Console.WriteLine($"{name}: {rowCount} rows, {columnCount} columns");
    }
}
```

### Enable Performance Features

```csharp
var reader = new JetDatabaseReader("database.mdb")
{
    PageCacheSize = 512,              // Increase cache (default: 256)
    ParallelPageReadsEnabled = true,  // Parallel I/O (default: false)
    DiagnosticsEnabled = true         // Debug output (default: false)
};
```

## Limitations

- ❌ **Encrypted databases** - Remove password in Access first (File > Info > Encrypt with Password)
- ❌ **Attachment fields** (Type 0x11) - Rare, added in Access 2007
- ❌ **Linked tables** - Only local tables are returned
- ❌ **Write operations** - Read-only library

## Compatibility

- **.NET Standard 2.0** - Compatible with:
  - .NET Framework 4.6.1+
  - .NET Core 2.0+
  - .NET 5/6/7/8+
  - Xamarin, Unity, UWP

## How It Works

Based on the [mdbtools](https://github.com/mdbtools/mdbtools) format specification, this library directly parses JET database pages without requiring Microsoft drivers. It handles:

- Database header parsing (Jet3/Jet4 detection)
- MSysObjects catalog scanning
- TDEF (table definition) page chains
- Data page traversal
- LVAL (long value) chains for MEMO/OLE fields
- Compressed Unicode text (Jet4)
- Code page detection for non-Western text

## Contributing

Contributions are welcome! Please open an issue or submit a pull request.

## License

MIT License - see [LICENSE](LICENSE) for details

## Acknowledgments

Format specification based on [mdbtools](https://github.com/mdbtools/mdbtools) by Brian Bruns and contributors.
