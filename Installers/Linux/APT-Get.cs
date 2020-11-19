using System.IO;

namespace MSharp.Build.Installers.Linux
{
    class APTGet : Installer
    {
        public APTGet(string name, string installCommand) : base(name, installCommand)
        {
        }

        protected override FileInfo Executable => Commands.APT_GET;
    }
}