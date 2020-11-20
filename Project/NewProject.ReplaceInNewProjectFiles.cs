using System;
using System.IO;
using System.Linq;
using Olive;

namespace MSharp.Build.Project
{
    partial class NewProject
    {
        public void ReplaceInNewProjectFiles()
        {
            Rename(Args.Name, "MY.PROJECT.NAME");
            SetRandomPortNumber();
        }

        void Rename(string value, string key)
        {
            Log("Renaming file contents ...");
            foreach (var file in Args.TempTemplate.GetFiles(includeSubDirectories: true))
            {
                var content = File.ReadAllText(file);
                if (content.Contains(key, caseSensitive: false))
                {
                    content = content.Replace(key, value);
                    content = content.Replace(key.ToLower(), value.ToLowerOrEmpty());

                    File.WriteAllText(file, content);
                }

                var fileName = Path.GetFileName(file);
                if (fileName.Contains(key))
                    File.Move(file, file.Replace(key, value));
            }
        }

        void SetRandomPortNumber()
        {
            Log("Changing port number ...");
            var file = Directory.GetFiles(Args.TempTemplate.FullName, "launchSettings.json", SearchOption.AllDirectories).SingleOrDefault();
            if (file.IsEmpty()) throw new Exception("launchSettings.json was not found!");

            var text = File.ReadAllText(file).Replace("$randomportnumber$", new Random().Next(2000, 65535).ToString());
            File.WriteAllText(file, text);
        }
    }
}