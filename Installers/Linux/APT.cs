using System.IO;

namespace MSharp.Build.Installers.Linux
{
    class APT : Installer
    {
        public APT(string name, string installCommand) : base(name, installCommand)
        {
        }

        protected override FileInfo Executable => Commands.APT;
    }
}