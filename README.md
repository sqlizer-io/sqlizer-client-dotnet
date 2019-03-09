# sqlizer-client-dotnet
A .NET Client for [SQLizer](https://sqlizer.io)'s API

SQLizer lets you easily and securely convert Excel, JSON, CSV and other files into SQL databases with table definitions and multiple INSERT statements. We're free for non-commercial use on files with under 5000 rows of data and our paid plans enable you to use our powerful API to build data pipelines that handle billions of rows a day.

SQlizer's [API Help Documentation](https://sqlizer.io/help/api/)

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

await file.Convert("destination.sql");
```
