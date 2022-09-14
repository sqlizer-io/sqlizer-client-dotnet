using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SQLizerClient
{
    public class SQLizerFile
    {
        public DatabaseType DatabaseType;
        public string TableName;
        public bool HasHeaders;
        public bool CheckTableExists;
        public int InsertSpacing;
        public string SheetName;
        public string CellRange;
        private string InputFilePath;

        public SQLizerFile(string inputFilePath)
        {
            InputFilePath = inputFilePath;
        }

        public string ID { get; private set; }
        public string Status { get; private set; }

        public async Task<bool> SaveResultAsync(string outputFilePath)
        {
            var client = new HttpClient();

            if (!String.IsNullOrWhiteSpace(Settings.ApiKey))
            {
                client.DefaultRequestHeaders.Add("Authorization","Bearer " + Settings.ApiKey);
            }

            var fileProperties = new List<KeyValuePair<string, string>>() {
                new KeyValuePair<string, string>("DatabaseType", ((Enum)DatabaseType).ToString()),
                new KeyValuePair<string, string>("FileName", Path.GetFileName(InputFilePath)),
                new KeyValuePair<string, string>("TableName", TableName),
                new KeyValuePair<string, string>("FileHasHeaders", HasHeaders ? "true" : "false"),
                new KeyValuePair<string, string>("CheckTableExists", CheckTableExists ? "true" : "false"),
                new KeyValuePair<string, string>("InsertSpacing", InsertSpacing.ToString()),
            };

            switch (Path.GetExtension(InputFilePath))
            {
                case ".xlsx":
                case ".xls":
                    fileProperties.Add(new KeyValuePair<string, string>("FileType", "xlsx"));
                    fileProperties.Add(new KeyValuePair<string, string>("SheetName", SheetName));
                    fileProperties.Add(new KeyValuePair<string, string>("CellRange", CellRange));
                    break;

                default:
                    fileProperties.Add(new KeyValuePair<string, string>("FileType", Path.GetExtension(InputFilePath).Trim('.')));
                    break;
            }

            // Create a file on SQLizer.io
            var createFileResponse = await client.PostAsync("https://sqlizer.io/api/files/",  new FormUrlEncodedContent(fileProperties));

            if (createFileResponse.IsSuccessStatusCode)
            {
                var sqlizerProperties =  JsonConvert.DeserializeObject<Dictionary<string, object>>(await createFileResponse.Content.ReadAsStringAsync());

                ID = sqlizerProperties["ID"].ToString();

                // Upload the data
                using (var file = File.Open(InputFilePath, FileMode.Open))
                {
                    var bytes = new byte[10000000];
                    int numBytesToRead = (int)file.Length;
                    int numBytesRead = 0;
                    int partNumber = 0;

                    while (numBytesToRead > 0)
                    {
                        int n = file.Read(bytes, numBytesRead, numBytesToRead);

                        var form = new MultipartFormDataContent();
                        var byteContent = new ByteArrayContent(bytes, 0, n);
                        byteContent.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");
                        form.Add(byteContent, "file", Path.GetFileName(InputFilePath));
                        form.Add(new StringContent((++partNumber).ToString()), "PartNumber");
                        
                        var uploadDataResponse = await client.PostAsync($"https://sqlizer.io/api/files/{ID}/data/", form);
            
                        if (uploadDataResponse.IsSuccessStatusCode == false)
                        {
                            throw new Exception(string.Format("Failed to upload data (Part {0}): (1)", partNumber, await uploadDataResponse.Content.ReadAsStringAsync()));
                        }

                        if (n == 0)
                            break;

                        numBytesRead += n;
                        numBytesToRead -= n;
                    }
                }
                    
                var updateStatusResponse = await client.PutAsync($"https://sqlizer.io/api/files/{ID}/", new FormUrlEncodedContent(new [] {
                    new KeyValuePair<string, string>("Status", "Uploaded")
                }));

                if (updateStatusResponse.IsSuccessStatusCode)
                {
                    sqlizerProperties =  JsonConvert.DeserializeObject<Dictionary<string, object>>(await updateStatusResponse.Content.ReadAsStringAsync());

                    Status = sqlizerProperties["Status"].ToString(); 

                    // Poll for updates to the status
                    while ((Status != "Complete") && (Status != "Failed"))
                    {
                        Thread.Sleep(1000);

                        var getStatusResponse = await client.GetAsync($"https://sqlizer.io/api/files/{ID}/");

                        if (getStatusResponse.IsSuccessStatusCode)
                        {
                            sqlizerProperties =  JsonConvert.DeserializeObject<Dictionary<string, object>>(await getStatusResponse.Content.ReadAsStringAsync());
                            Status = sqlizerProperties["Status"].ToString();
                        }
                        else
                        {
                            throw new Exception(string.Format("Failed to get file state, status code: {0}", getStatusResponse.StatusCode));
                        }
                    }

                    if (Status == "Complete")
                    {
                        var awsClient = new HttpClient();

                        // Download the converted file
                        var downloadResultResponse = await awsClient.GetAsync(sqlizerProperties["ResultUrl"].ToString());
                        if (downloadResultResponse.IsSuccessStatusCode)
                        {
                            var contentStream = await downloadResultResponse.Content.ReadAsStreamAsync();
                            using (var outputFile = new FileStream(outputFilePath, FileMode.CreateNew))
                            {
                                await contentStream.CopyToAsync(outputFile);
                            }

                            return true;
                        }
                    }
                }
            }
            
            return false;
        }
    }
}
