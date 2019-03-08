# sqlizer-client-dotnet
A .NET Client for [SQLizer](https://sqlizer.io)'s API

## Getting Started

To convert a file, use to the SQLizerFile object, like this:

```csharp
using SQLizerClient;

var file = new SQLizerFile("source.csv");

file.DatabaseType = DatabaseType.MySQL;
file.TableName = "my_table";
file.HasHeaders = true;
file.CheckTableExists = true;
file.InsertSpacing = 250;

await file.Convert("destination.sql")
```