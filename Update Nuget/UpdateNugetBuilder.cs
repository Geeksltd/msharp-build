﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using MSharp.Build.UpdateNuget;
using Olive;

namespace MSharp.Build
{
    class UpdateNugetBuilder : Builder
    {
        FileInfo[] ProjectFiles;
        List<NugetReference> References = new List<NugetReference>();
        Dictionary<string, string> NewVersions;

        protected override void AddTasks()
        {
            Add(() => FindProjectFiles());
            Add(() => FindUsedPackages());
            Add(() => FindLatestVersions());
            Add(() => UpdateProjects());
        }

        void FindUsedPackages() => References = ProjectFiles.SelectMany(GetNugetPackages).ToList();

        void FindProjectFiles()
        {
            ProjectFiles = new[] { "Website", "Domain",Path.Combine("M#","Model"), Path.Combine("M#","UI") }
                .Select(OliveSolution.Root.GetSubDirectory)
                .Where(v => v.Exists())
                   .Select(v => v.GetFiles("*.csproj").WithMax(x => x.LastWriteTimeUtc))
                   .ExceptNull().ToArray();
        }

        static NugetReference[] GetNugetPackages(FileInfo csproj)
        {
            return csproj.ReadAllText().Trim()
                .To<XDocument>().Root.RemoveNamespaces()
                .Descendants(XName.Get("PackageReference"))
                .Select(v => new NugetReference(v.GetValue<string>("@Include"), v.GetValue<string>("@Version"), csproj))
                .Where(v => v.Name.StartsWith("Olive") || v.Name.StartsWith("MSharp") || v.Name.StartsWith("FS.Shared.Website"))
                .ToArray();
        }

        void FindLatestVersions()
        {
            NewVersions = References.Select(v => v.Name).Distinct()
                .AsParallel()
                .Select(v => new { target = v, ver = NugetReference.GetLatestVersion(v) })
                .ToArray()
                .ToDictionary(x => x.target, x => x.ver);
        }

        void UpdateProjects()
        {
            ProjectFiles.Do(Update);
            Console.WriteLine();
        }

        void Update(FileInfo project)
        {
            var xml = project.ReadAllText().To<XDocument>();

            var packageReference = xml.Root.Descendants("PackageReference");

            Console.WriteLine();
            ConsoleWrite(project.Name.PadRight(15), ConsoleColor.White);
            Console.WriteLine();

            foreach (var r in References.Where(v => v.Project.FullName == project.FullName))
            {
                var node = packageReference.Single(x => x.Attribute("Include").Value == r.Name);

                var newVersion = NewVersions[r.Name];
                var old = node.Attribute("Version").Value;

                if (newVersion == old) continue;

                node.Attribute("Version").Value = newVersion;

                ConsoleWrite(r.Name.PadRight(40), ConsoleColor.Cyan);
                Console.Write(" ");
                ConsoleWrite(old.PadRight(10), ConsoleColor.Red);
                Console.Write(" --> ");
                ConsoleWrite(newVersion, ConsoleColor.Green);
                Console.WriteLine();
            }

            xml.Save(project.FullName);
        }

        static void ConsoleWrite(string text, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.Write(text);
            Console.ResetColor();
        }

    }
}