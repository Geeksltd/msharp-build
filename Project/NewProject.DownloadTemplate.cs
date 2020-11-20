using System;
using System.IO;
using System.IO.Compression;
using System.Net;

namespace MSharp.Build.Project
{
    partial class NewProject
    {
        const string ZipFileName = "master.zip";

        public void DownloadTemplate()
        {
            var downloadedFilesExtractPath = Args["DownloadedFilesExtractPath"];
            Directory.CreateDirectory(downloadedFilesExtractPath);

            if (!DownloadAsync(Args["TemplateWebAddress"], downloadedFilesExtractPath, ZipFileName)) return;

            var zipFilePath = Path.Combine(downloadedFilesExtractPath, ZipFileName);
            ZipFile.ExtractToDirectory(zipFilePath, downloadedFilesExtractPath);
            File.Delete(zipFilePath);
        }

        bool DownloadAsync(string sourceWebAddress, string destPath, string fileName)
        {
            var destFullPath = Path.Combine(destPath, fileName);
            try
            {
                Log("Downloading latest project template ...");

                var client = new WebClient();
                client.DownloadFile(sourceWebAddress, destFullPath);

                Log("Done.");
                return true;
            }
            catch (Exception ex)
            {
                Log("Error in downloading template file: " + ex.Message);
            }

            return false;
        }
    }
}