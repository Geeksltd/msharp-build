using System.IO;

namespace MSharp.Build.UpdateNuget
{
    public class NugetReference
    {
        public string Name { get; set; }
        public string Version { get; set; }
        public FileInfo Project { get; }

        public NugetReference(string package, string version, FileInfo project)
        {
            Name = package;
            Version = version;
            Project = project;
        }
    }
}