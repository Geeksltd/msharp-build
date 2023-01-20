using Olive;
using System;
using System.IO;

namespace MSharp.Build.Project
{
    partial class NewProject
    {
        void CopyFiles()
        {
            Log("Copying the files ...");

            var source = Args.TempTemplate.GetDirectories("Template", SearchOption.AllDirectories).WithMin(v => v.FullName.Length);

            CopyFolderContents(source.FullName, Args.Destination.FullName);
            Args.TempTemplate.Delete(recursive: true);
        }

        bool CopyFolderContents(string sourcePath, string destinationPath)
        {
            sourcePath = sourcePath.EndsWith(Path.PathSeparator) ? sourcePath : sourcePath + Path.PathSeparator;
            destinationPath = destinationPath.EndsWith(Path.PathSeparator) ? destinationPath : destinationPath + Path.PathSeparator;

            try
            {
                if (Directory.Exists(sourcePath))
                {
                    if (Directory.Exists(destinationPath) == false)
                        Directory.CreateDirectory(destinationPath);

                    foreach (var files in Directory.GetFiles(sourcePath))
                    {
                        var fileInfo = files.AsFile();
                        fileInfo.CopyTo(Path.Combine(destinationPath, fileInfo.Name), overwrite: true);
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