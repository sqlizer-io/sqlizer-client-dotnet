# sqlizer-client-dotnet

A .NET Client for [SQLizer](https://sqlizer.io)'s API

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