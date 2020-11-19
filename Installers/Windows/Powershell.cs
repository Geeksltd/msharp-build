using System.IO;

namespace MSharp.Build.Installers.Windows
{
    class Powershell : Installer
    {
        public Powershell(string name, string installCommand) : base(name, installCommand)
        {
        }

        protected override FileInfo Executable => Commands.Powershell;
    }
}