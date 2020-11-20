using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace MSharp.Build.Project
{
    class NewProject : Builder
    {
        private readonly Dictionary<string, string> _inputArgs;

        public NewProject(Dictionary<string, string> inputArgs)
        {
            _inputArgs = inputArgs;
        }

        protected override void AddTasks()
        {
            Console.WriteLine("Help: http://learn.msharp.co.uk/#/Install/README");

            Add(() => InstallDownloadTemplate());
            Add(() => InstallReplaceInNewProjectFiles());
            Add(() => InstallCopyFiles());
        }

        void InstallDownloadTemplate() => Install<DownloadTemplate>();

        void InstallReplaceInNewProjectFiles() => Install<ReplaceInNewProjectFiles>();

        void InstallCopyFiles() => Install<CopyFiles>();

        void Install<T>([CallerMemberName] string step = "") where T : ProjectPart, new()
        {
            var part = new T();
            try
            {
                part.Install(_inputArgs);
            }
            finally
            {
                Log(string.Join(Environment.NewLine, part.Logs), step);
            }
        }
    }
}