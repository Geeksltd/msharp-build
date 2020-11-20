using System;
using System.Collections.Generic;

namespace MSharp.Build.Project
{
    partial class NewProject : Builder
    {
        readonly Dictionary<string, string> Args;
        public NewProject(Dictionary<string, string> inputArgs) => Args = inputArgs;

        protected override void AddTasks()
        {
            Console.WriteLine("Help: http://learn.msharp.co.uk/#/Install/README");

            Add(() => DownloadTemplate());
            Add(() => ReplaceInNewProjectFiles());
            Add(() => CopyFiles());
        }
    }
}