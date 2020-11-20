using System;
using System.IO;
using System.Linq;
using Olive;

namespace MSharp.Build.Project
{
    partial class NewProject
    {
        const string ProjectNameTemplateKey = "MY.PROJECT.NAME";

        public void ReplaceInNewProjectFiles()
        {
            Rename(Args["DownloadedFilesExtractPath"], Args["ProjectName"], ProjectNameTemplateKey);
            SetRandomPortNumber(Args["DownloadedFilesExtractPath"]);
        }

        void Rename(string destPath, string value, string key)
        {
            Log("Renaming file contents ...");
            foreach (var file in Directory.GetFiles(destPath, "*.*", SearchOption.AllDirectories))
            {
                var fileContent = File.ReadAllText(file);
                if (fileContent.Contains(key, caseSensitive: false))
                {
                    fileContent = fileContent.Replace(key, value);
                    fileContent = fileContent.Replace(key.ToLower(), value.ToLowerOrEmpty());

                    File.WriteAllText(file, fileContent);
                }

                var fileName = Path.GetFileName(file);
                if (fileName.Contains(key))
                    File.Move(file, file.Replace(key, value));
            }
        }

        void SetRandomPortNumber(string destPath)
        {
            Log("Changing port number ...");
            var file = Directory.GetFiles(destPath, "launchSettings.json", SearchOption.AllDirectories).SingleOrDefault();
            if (file.IsEmpty()) throw new Exception("launchSettings.json was not found!");

            var text = File.ReadAllText(file).Replace("$randomportnumber$", new Random().Next(2000, 65535).ToString());
            File.WriteAllText(file, text);
        }
    }
}