using System;
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
            ProjectFiles = new[] { "Website", "Domain", "M#\\Model", "M#\\UI" }
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
                .ToArray();
        }

        void FindLatestVersions()
        {
            string findLatestVersion(string package)
            {
                try
                {
                    return $"https://www.nuget.org/packages/{package}/".AsUri()
                     .Download().GetAwaiter().GetResult().ToLower()
                         .Substring($"<meta property=\"og:title\" content=\"{package.ToLower()} ", "\"", inclusive: false);
                }
                catch (Exception ex) when (ex.Message.Contains("404") || ex.Message.Contains("429"))
                {
                    throw new Exception("Failed to find latest nuget version for " + package);
                }
            }

            NewVersions = References.Select(v => v.Name).Distinct()
                .ToDictionary(x => x, findLatestVersion);
        }

        void UpdateProjects() => ProjectFiles.Do(Update);

        void Update(FileInfo project)
        {
            var projectXML = XElement.Load(project.ReadAllText());
            var packageReference = projectXML.Descendants("PackageReference");

            foreach (var r in References.Where(v => v.Project.FullName == project.FullName))
            {
                packageReference
                   .Single(x => x.Attribute("Include").Value == r.Name)
                   .Attribute("Version").Value = NewVersions[r.Name];
            }

            projectXML.Save(project.FullName);
        }
    }
}