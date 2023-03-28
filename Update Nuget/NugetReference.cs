using System;
using System.IO;
using Olive;

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

        public static string GetLatestVersion(string package)
        {
            try
            {
                if(package== "FS.Shared.Website")
                {
                    var ss= $"https://nuget.app.geeks.ltd/packages/{package}/".AsUri()
                 .Download().GetAwaiter().GetResult().ToLower();
                     var kk= ss.Substring("<title>fs.shared.website ", " - baget</title>", inclusive: false);
                    return kk;
                }
                else { 
                return $"https://www.nuget.org/packages/{package}/".AsUri()
                 .Download().GetAwaiter().GetResult().ToLower()
                     .Substring($"<meta property=\"og:title\" content=\"{package.ToLower()} ", "\"", inclusive: false);
                    }
            }
            catch (Exception ex) when (ex.Message.Contains("404") || ex.Message.Contains("429"))
            {
                throw new Exception("Failed to find latest nuget version for " + package);
            }
        }
    }
}