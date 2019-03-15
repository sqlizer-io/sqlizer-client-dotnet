[![AppVeyor](https://ci.appveyor.com/api/projects/status/github/sqlizer-io/sqlizer-client-dotnet?svg=true)](https://ci.appveyor.com/api/projects/status/github/sqlizer-io/sqlizer-client-dotnet?svg=true)

# sqlizer-client-dotnet

A .NET Client for [SQLizer](https://sqlizer.io)'s API

SQLizer lets you easily and securely convert Excel, JSON, CSV and other files into SQL databases with table definitions and multiple INSERT statements. We're free for non-commercial use on files with under 5000 rows of data and our paid plans enable you to use our powerful API to build data pipelines that handle billions of rows a day.

SQlizer's [API Help Documentation](https://sqlizer.io/help/api/)

## Getting Started

To convert a file, use to the SQLizerFile object, like this:

```csharp
SQLizerClient.Settings.ApiKey = "{your api key}"; // This can be found on https://sqlizer.io/account/

var file = new SQLizerClient.SQLizerFile("source.xlsx");

file.DatabaseType = SQLizerClient.DatabaseType.MySQL;
file.TableName = "my_table";
file.HasHeaders = true;
file.SheetName = "Sheet1";
file.CellRange = "A1:C99";
file.CheckTableExists = true;
file.InsertSpacing = 250;

await file.SaveResultAsync("destination.sql");
```
