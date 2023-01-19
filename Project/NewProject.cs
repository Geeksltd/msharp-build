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
                Add(() => CloneTemplate());
                Add(() => ReplaceInNewProjectFiles());
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