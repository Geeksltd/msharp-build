using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;

namespace MSharp.Build.Project
{
    class DownloadTemplate : ProjectPart
    {
        const string ZipFileName = "master.zip";

        public override void Install(Dictionary<string, string> inputArgs)
        {
            var downloadedFilesExtractPath = inputArgs["DownloadedFilesExtractPath"];
            Directory.CreateDirectory(downloadedFilesExtractPath);

            if (!DownloadAsync(inputArgs["TemplateWebAddress"], downloadedFilesExtractPath, ZipFileName)) return;

            var zipFilePath = Path.Combine(downloadedFilesExtractPath, ZipFileName);
            ZipFile.ExtractToDirectory(zipFilePath, downloadedFilesExtractPath);
            File.Delete(zipFilePath);
        }

        bool DownloadAsync(string sourceWebAddress, string destPath, string fileName)
        {
            var destFullPath = Path.Combine(destPath, fileName);
            try
            {
                Logs.Add("Downloading latest project template ...");

                var client = new WebClient();
                client.DownloadFile(sourceWebAddress, destFullPath);

                Logs.Add("Done.");
                return true;
            }
            catch (HttpRequestException ex)
            {
                Logs.Add($"Error in downloading template file: {ex.Message}");
            }
            catch (OperationCanceledException)
            {
                Logs.Add("Project template download canceled.");
            }
            catch (Exception ex)
            {
                Logs.Add("Error in downloading template file");
            }

            return false;
        }
    }
}