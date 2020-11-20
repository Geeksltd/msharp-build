using System;
using System.Collections.Generic;
using System.Diagnostics;
using Olive;

namespace MSharp.Build.Project
{
    partial class NewProject : Builder
    {
        readonly NewProjectArgs Args;
        public NewProject(NewProjectArgs args) => Args = args;

        protected override void AddTasks()
        {
            Console.WriteLine("Help: http://learn.msharp.co.uk/#/Install/README");

            Add(() => DownloadTemplate());
            Add(() => ReplaceInNewProjectFiles());
            Add(() => CopyFiles());
            Add(() => Compile());
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