using System;
using Xunit;
using SQLizerClient;
using System.IO;

namespace Tests
{
    public class CSVFileTests
    {
        [Fact]
        public async void TestSimpleCSV()
        {
            var file = new SQLizerFile("Simple.csv");
            
            file.DatabaseType = DatabaseType.MySQL;
            file.TableName = "testing";
            file.HasHeaders = true;
            file.CheckTableExists = true;
            file.InsertSpacing = 250;

            var outputFileName = "Simple.sql";

            if (File.Exists(outputFileName))
            {
                File.Delete(outputFileName);
            }

            Assert.True(await file.Convert(outputFileName), "Failed to convert Simple.csv");

            using (var outputFile = File.OpenText(outputFileName))
            {
                Assert.Contains("testing", outputFile.ReadToEnd());
            }
        }
    }
}
