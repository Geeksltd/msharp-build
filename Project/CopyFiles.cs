using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Olive;

namespace MSharp.Build.Project
{
    class CopyFiles : ProjectPart
    {
        private const string TemplateFolderName = "Template";

        public override void Install(Dictionary<string, string> inputArgs)
        {
            Logs.Add("Copying the files ...");

            var projectFolder = Path.Combine(inputArgs["DestinationDirectory"], inputArgs["ProjectName"]);
            Directory.CreateDirectory(projectFolder);

            var extractPath = inputArgs["DownloadedFilesExtractPath"];
            var template = Directory.GetDirectories(extractPath).FirstOrDefault();
            if (template.IsEmpty()) return;
            var templateDirectory = Directory.GetDirectories(template).FirstOrDefault();
            var tempDirObj = new DirectoryInfo(templateDirectory);
            if (tempDirObj.Name != TemplateFolderName) return;
            CopyFolderContents(templateDirectory, projectFolder);
            DeleteDirectory(extractPath);
        }

        public bool CopyFolderContents(string sourcePath, string destinationPath)
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

        public void DeleteDirectory(string path)
        {
            if (!Directory.Exists(path)) return;

            // Delete all files from the Directory
            foreach (var file in Directory.GetFiles(path))
                File.Delete(file);

            // Delete all child Directories
            foreach (var directory in Directory.GetDirectories(path))
                DeleteDirectory(directory);

            // Delete a Directory
            Directory.Delete(path);
        }
    }
}
