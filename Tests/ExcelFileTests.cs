using System;
using Xunit;
using System.IO;

namespace SQLizerClient.Tests
{
    public class ExcelFileTests
    {
        [Fact]
        public async void TestSimpleXlsx()
        {
            var file = new SQLizerFile("Simple.xlsx");
            
            file.DatabaseType = DatabaseType.MySQL;
            file.TableName = "testing";
            file.HasHeaders = true;
            file.SheetName = "Sheet1";
            file.CellRange = "A1:C3";
            file.CheckTableExists = true;
            file.InsertSpacing = 250;

            var outputFileName = "SimpleXlsx.sql";

            if (File.Exists(outputFileName))
            {
                File.Delete(outputFileName);
            }

            Assert.True(await file.SaveResultAsync(outputFileName), "Failed to convert Simple.xlsx");

            using (var outputFile = File.OpenText(outputFileName))
            {
                Assert.Contains("testing", outputFile.ReadToEnd());
            }
        }

        [Fact]
        public async void TestSimpleXls()
        {
            var file = new SQLizerFile("Simple.xls");
            
            file.DatabaseType = DatabaseType.MySQL;
            file.TableName = "testing";
            file.HasHeaders = true;
            file.SheetName = "Sheet1";
            file.CellRange = "A1:C3";
            file.CheckTableExists = true;
            file.InsertSpacing = 250;

            var outputFileName = "SimpleXls.sql";

            if (File.Exists(outputFileName))
            {
                File.Delete(outputFileName);
            }

            Assert.True(await file.SaveResultAsync(outputFileName), "Failed to convert Simple.xls");

            using (var outputFile = File.OpenText(outputFileName))
            {
                Assert.Contains("testing", outputFile.ReadToEnd());
            }
        }
    }
}