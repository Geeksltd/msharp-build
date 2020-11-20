using System;
using System.IO;
using Olive;

namespace MSharp.Build.Project
{
    class NewProjectArgs
    {
        public string Name;
        public DirectoryInfo Destination = Environment.CurrentDirectory.AsDirectory();
        public DirectoryInfo TempTemplate = Path.GetTempPath().AsDirectory().GetOrCreateSubDirectory("msharp-build\\" + Guid.NewGuid());
        public string TemplateWebAddress = "https://github.com/Geeksltd/Olive.MvcTemplate/archive/master.zip";

        public NewProjectArgs(string[] args)
        {
            foreach (var arg in args)
            {
                if (arg == "/new") continue;
                else if (arg.StartsWith("/n:")) Name = arg.Substring(3);
                else if (arg.StartsWith("/t:")) TemplateWebAddress = arg.Substring(3);
                else if (arg.StartsWith("/o:")) Destination = arg.Substring(3).AsDirectory();
                else if (arg.StartsWith("/")) throw new ArgumentException("Argument not recognised: " + arg);
            }

            if (Name.IsEmpty())
                throw new ArgumentException("Missing mandatory argument /n:MyProjectName");

            Destination = Destination.GetSubDirectory(Name).EnsureExists();
        }
    }
}