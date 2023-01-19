using Olive;
using System;
using System.IO.Compression;
using System.Net;

namespace MSharp.Build.Project
{
    partial class NewProject
    {
        const string ZipFileName = "master.zip";

        void DownloadTemplate()
        {
            if (!Download(Args.TemplateWebAddress, ZipFileName)) return;

            var zip = Args.TempTemplate.GetFile(ZipFileName);
            ZipFile.ExtractToDirectory(zip.FullName, Args.TempTemplate.FullName);
            zip.Delete();
        }

        bool Download(string sourceWebAddress, string fileName)
        {
            var destFullPath = Args.TempTemplate.GetFile(fileName);
            try
            {
                Log("Downloading latest project template ...");
                var client = new WebClient();
                client.DownloadFile(sourceWebAddress, destFullPath.FullName);
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