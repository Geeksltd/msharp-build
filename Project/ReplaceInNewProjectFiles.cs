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
            ApplyDbType(inputArgs["DownloadedFilesExtractPath"], DBType.FromString(inputArgs["DbType"]), inputArgs["ConnectionString"]);
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

        void ApplyDbType(string destPath, DBType selectedDbType, string constr)
        {
            Logs.Add("Applying database ...");
            ChangeConnectionString(destPath, constr);

            if (selectedDbType != DBType.SqlServer)
            {
                FixDomainCsProjectReference(destPath, selectedDbType);
                FixSqlDialect(destPath, selectedDbType);
                FixStartUp(destPath, selectedDbType);
            }
        }

        void ChangeConnectionString(string destPath, string constr)
        {
            var jsonkey = @"""AppDatabase""";
            var appsettings = Directory.GetFiles(destPath, "appsettings.json", SearchOption.AllDirectories)
                .Single(f => f.AsFile().DirectoryName?.ToLower().EndsWith("\\website") == true);
            var allLines = File.ReadAllLines(appsettings);
            for (var index = 0; index < allLines.Length; index++)
            {
                var line = allLines[index];
                if (line.Lacks(jsonkey)) continue;
                var hasComma = line.Trim().EndsWith(",");

                allLines[index] = $"\t{jsonkey}: \"{constr}\" {(hasComma ? "," : "")}";
                break;
            }

            File.WriteAllLines(appsettings, allLines);
        }

        void FixSqlDialect(string destPath, DBType selectedDbType)
        {
            var file = Directory.GetFiles(destPath, "Project.cs", SearchOption.AllDirectories).FirstOrDefault(x => x.Contains("M#\\Model\\"));
            if (file.IsEmpty()) throw new Exception("M#\\Model\\Project.cs was not found!");

            // var file = Path.Combine(destPath, "Model", "Project.cs");

            var text = File.ReadAllText(file);
            var index = text.IndexOf("\n", text.IndexOf("Name(\"", StringComparison.Ordinal), StringComparison.Ordinal) + 1;
            text = text.Insert(index, $"SqlDialect(MSharp.SqlDialect.{selectedDbType.Dialect});");

            File.WriteAllText(file, text);
        }

        void FixStartUp(string destPath, DBType selectedDbType)
        {
            var file = Directory.GetFiles(destPath, "StartUp.cs", SearchOption.AllDirectories).SingleOrDefault();
            if (file.IsEmpty()) throw new Exception("StartUp.cs was not found!");

            var text = File.ReadAllText(file).ReplaceWholeWord("SqlServerManager", selectedDbType.Manager);
            File.WriteAllText(file, text);
        }

        void FixDomainCsProjectReference(string destPath, DBType selectedDbType)
        {
            var domainFile = Directory.GetFiles(destPath, "Domain.csproj", SearchOption.AllDirectories).FirstOrDefault();
            if (domainFile.IsEmpty()) return;

            var serializer = new XmlSerializer(typeof(Project));

            Project proj;
            using (var fileStream = File.OpenRead(domainFile))
                proj = (Project)serializer.Deserialize(fileStream);

            var sqlServerReference = proj.ItemGroup.Single(x => x.Include == "Olive.Entities.Data.SqlServer");
            sqlServerReference.Include = "Olive.Entities.Data." + selectedDbType.Provider;
            sqlServerReference.Version = selectedDbType.OliveVersion;

            var newProjStruct = SerializeToString(proj);
            File.WriteAllText(domainFile, newProjStruct);
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