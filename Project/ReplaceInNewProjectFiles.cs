using Olive;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;

namespace MSharp.Build.Project
{
    class ReplaceInNewProjectFiles : ProjectPart
    {
        const string ProjectNameTemplateKey = "MY.PROJECT.NAME";

        public override void Install(Dictionary<string, string> inputArgs)
        {
            Rename(inputArgs["DownloadedFilesExtractPath"], inputArgs["ProjectName"], ProjectNameTemplateKey);
            SetRandomPortNumber(inputArgs["DownloadedFilesExtractPath"]);
        }

        void Rename(string destPath, string templateValue, string templateKey)
        {
            Logs.Add("Renaming file contents ...");
            foreach (var file in Directory.GetFiles(destPath, "*.*", SearchOption.AllDirectories))
            {
                var fileContent = File.ReadAllText(file);
                if (fileContent.Contains(templateKey, caseSensitive: false))
                {
                    fileContent = fileContent.Replace(templateKey, templateValue);
                    fileContent = fileContent.Replace(templateKey.ToLower(), templateValue.ToLowerOrEmpty());

                    File.WriteAllText(file, fileContent);
                }

                var fileName = Path.GetFileName(file);
                if (fileName.Contains(templateKey))
                    File.Move(file, file.Replace(templateKey, templateValue));
            }
        }

        string SerializeToString<T>(T value)
        {
            var emptyNamepsaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
            var serializer = new XmlSerializer(value.GetType());
            var settings = new XmlWriterSettings
            {
                Indent = true,
                OmitXmlDeclaration = true
            };

            using (var stream = new StringWriter())
            using (var writer = XmlWriter.Create(stream, settings))
            {
                serializer.Serialize(writer, value, emptyNamepsaces);
                return stream.ToString();
            }
        }

        void SetRandomPortNumber(string destPath)
        {
            Logs.Add("Changing port number ...");
            var file = Directory.GetFiles(destPath, "launchSettings.json", SearchOption.AllDirectories).SingleOrDefault();
            if (file.IsEmpty()) throw new Exception("launchSettings.json was not found!");

            var text = File.ReadAllText(file).Replace("$randomportnumber$", new Random().Next(2000, 65535).ToString());
            File.WriteAllText(file, text);
        }
    }
}