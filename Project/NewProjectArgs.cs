using Olive;
using System;
using System.IO;

namespace MSharp.Build.Project
{
    class NewProjectArgs
    {
        public string Name;
        public string PortNumber;
        public string TemplateWebAddress;
        public ProjectType ProjectType;
        public DirectoryInfo Destination = Environment.CurrentDirectory.AsDirectory();
        public DirectoryInfo TempTemplate = Path.GetTempPath().AsDirectory().GetOrCreateSubDirectory("msharp-build\\" + Guid.NewGuid());

        public NewProjectArgs(string[] args)
        {
            foreach (var arg in args)
            {
                if (arg == "/new-ms")
                {
                    TemplateWebAddress = "https://github.com/Geeksltd/Olive.Mvc.Microservice.Template/archive/master.zip";
                    ProjectType = ProjectType.Microservice;
                    continue;
                }

                if (arg == "/new")
                {
                    TemplateWebAddress = "https://github.com/Geeksltd/Olive.MvcTemplate/archive/master.zip";
                    ProjectType = ProjectType.Mvc;
                    continue;
                }
                else if (arg.StartsWith("/n:")) Name = arg.Substring(3);
                else if (arg.StartsWith("/t:")) TemplateWebAddress = arg.Substring(3);
                else if (arg.StartsWith("/o:")) Destination = arg.Substring(3).AsDirectory();
                else if (arg.StartsWith("/p:")) PortNumber = arg.Substring(3);
                else if (arg.StartsWith("/")) throw new ArgumentException("Argument not recognised: " + arg);
            }

            if (Name.IsEmpty())
                throw new ArgumentException("Missing mandatory argument /n:MyProjectName");

            Destination = Destination.GetSubDirectory(Name).EnsureExists();
        }
    }
}