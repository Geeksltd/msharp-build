using System;
using System.IO;
using System.Linq;
using Olive;

namespace MSharp.Build.Project
{
    partial class NewProject
    {
        const string TemplateFolderName = "Template";

        public void CopyFiles()
        {
            Log("Copying the files ...");

            var projectFolder = Path.Combine(Args["DestinationDirectory"], Args["ProjectName"]);
            Directory.CreateDirectory(projectFolder);

            var extractPath = Args["DownloadedFilesExtractPath"];
            var template = Directory.GetDirectories(extractPath).FirstOrDefault();
            if (template.IsEmpty()) return;
            var templateDirectory = Directory.GetDirectories(template).FirstOrDefault();
            var tempDirObj = new DirectoryInfo(templateDirectory);
            if (tempDirObj.Name != TemplateFolderName) return;
            CopyFolderContents(templateDirectory, projectFolder);
            extractPath.AsDirectory().Delete(recursive: true);
        }

        bool CopyFolderContents(string sourcePath, string destinationPath)
        {
            sourcePath = sourcePath.EndsWith(@"\") ? sourcePath : sourcePath + @"\";
            destinationPath = destinationPath.EndsWith(@"\") ? destinationPath : destinationPath + @"\";

            try
            {
                if (Directory.Exists(sourcePath))
                {
                    if (Directory.Exists(destinationPath) == false)
                        Directory.CreateDirectory(destinationPath);

                    foreach (var files in Directory.GetFiles(sourcePath))
                    {
                        var fileInfo = files.AsFile();
                        fileInfo.CopyTo($@"{destinationPath}\{fileInfo.Name}", overwrite: true);
                    }

                    foreach (var drs in Directory.GetDirectories(sourcePath))
                    {
                        var directoryInfo = new DirectoryInfo(drs);
                        if (CopyFolderContents(drs, destinationPath + directoryInfo.Name) == false)
                            return false;
                    }
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}