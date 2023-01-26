using Olive;
using System;
using System.Diagnostics;

namespace MSharp.Build.Project
{
    partial class NewProject : Builder
    {
        readonly NewProjectArgs Args;
        public NewProject(NewProjectArgs args) => Args = args;

        protected override void AddTasks()
        {
            Console.WriteLine("Help: http://learn.msharp.co.uk/#/Install/README");

            if (Args.IsMicroserviceTemplate)
            {
                if (Args.MicroserviceFolderExists)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"A folder with the name of {Args.ServiceName} already exists. Please try to create in another folder");
                }
                else
                {
                    Add(() => CreateProjectFolder());
                    Add(() => CloneTemplate());
                    Add(() => ReplaceInNewProjectFiles());
                }
            }
            else
            {
                Add(() => DownloadTemplate());
                Add(() => ReplaceInNewProjectFiles());
                Add(() => CopyFiles());
                Add(() => Compile());
            }
        }

        void Compile()
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = Args.Destination.GetFile("Build.bat").FullName,
                WorkingDirectory = Args.Destination.FullName
            });
        }
    }
}