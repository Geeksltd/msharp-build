using Olive;
using System;
using System.Diagnostics;
using System.IO.Compression;
using System.Net;

namespace MSharp.Build.Project
{
    partial class NewProject
    {
        const string ZipFileName = "master.zip";

        void CloneTemplate()
        {
            string token = "";
            Args.BitbucketRepoTokens.TryGetValue(Args.Template.ToLower().Trim(), out token);
            if (string.IsNullOrEmpty(token))
            {
                throw new ArgumentException("template is not valid");
            }
            var process = Process.Start("cmd", $"/c git clone https://x-token-auth:{token}@bitbucket.org/geeks-ltd/{Args.Template}/get/master");
            process.WaitForExit();
        }

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