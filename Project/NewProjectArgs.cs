using Olive;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MSharp.Build.Project
{
    class NewProjectArgs
    {
        public string Name;
        public string ServiceName;
        public string Template;
        public string PortNumber;
        public string TemplateWebAddress;
        public ProjectType ProjectType;
        public DirectoryInfo Destination = Environment.CurrentDirectory.AsDirectory();
        public DirectoryInfo TempTemplate = Path.GetTempPath().AsDirectory().GetOrCreateSubDirectory(Path.Combine( "msharp-build" , Guid.NewGuid().ToString()));

        public bool MicroserviceProjectFolderExists = false;
        public bool IsMicroserviceTemplate { get { return !string.IsNullOrEmpty(Template) && !string.IsNullOrEmpty(ServiceName); } }
        internal Dictionary<string, string> BitbucketRepoTokens
        {
            get
            {
                return new Dictionary<string, string>
                {
                    { "akersolutions.microservice.template", "v37AlGv7EDLy1WKM4DWD" }
                };
            }
        }

        public NewProjectArgs(string[] args)
        {
            foreach (var arg in args)
            {
                if (arg == "/new-fs")
                {
                    TemplateWebAddress = "https://github.com/Geeksltd/FitSuite.Template/archive/master.zip";
                    ProjectType = ProjectType.Microservice;
                    continue;
                }

                if (arg == "/new-ms")
                {
                    TemplateWebAddress = "https://github.com/Geeksltd/Olive.Mvc.Microservice.Template/archive/master.zip";
                    ProjectType = ProjectType.Microservice;
                    string template = args.ToList().Where(x => x.StartsWith("/template:")).FirstOrDefault();
                    if (!string.IsNullOrEmpty(template))
                    {
                        Template = template.Substring("/template:".Length);
                    }
                    string serviceName = args.ToList().Where(x => x.StartsWith("/service:")).FirstOrDefault();
                    if (!string.IsNullOrEmpty(serviceName))
                    {
                        ServiceName = serviceName.Substring("/service:".Length);
                    }
                    continue;
                }

                if (arg.StartsWith("/template:") || arg.StartsWith("/service:"))
                {
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

            if (!IsMicroserviceTemplate)
            {
                Destination = Destination.GetSubDirectory(Name).EnsureExists();
            }
            else
            {
                MicroserviceProjectFolderExists = Directory.Exists(Path.Combine(Destination.FullName, Name));
            }
        }
    }
}