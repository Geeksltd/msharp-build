using System;
using System.IO;
using Olive;

namespace MSharp.Build.Project
{
    partial class NewProject
    {
        public void CopyFiles()
        {
            Log("Copying the files ...");

            var source = Args.TempTemplate.GetDirectories("Template", SearchOption.AllDirectories).WithMin(v => v.FullName.Length);

            CopyFolderContents(source.FullName, Args.Destination.FullName);
            Args.TempTemplate.Delete(recursive: true);
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