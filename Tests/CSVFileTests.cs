using System;
using Xunit;
using System.IO;

namespace SQLizerClient.Tests
{
    public class CSVFileTests
    {
        [Fact]
        public async void TestSimpleCSV()
        {
            var file = new SQLizerClient.SQLizerFile("Simple.csv");
            
            file.DatabaseType = SQLizerClient.DatabaseType.MySQL;
            file.TableName = "testing";
            file.HasHeaders = true;
            file.CheckTableExists = true;
            file.InsertSpacing = 250;

            var outputFileName = "SimpleCsv.sql";

            if (File.Exists(outputFileName))
            {
                File.Delete(outputFileName);
            }

            Assert.True(await file.SaveResultAsync(outputFileName), "Failed to convert Simple.csv");

            using (var outputFile = File.OpenText(outputFileName))
            {
                Assert.Contains("testing", outputFile.ReadToEnd());
            }
        }
    }
}
