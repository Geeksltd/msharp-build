using System.IO;
using Olive;

namespace MSharp.Build.Installers.Windows
{
    class Chocolaty : Installer
    {
        public Chocolaty(string name, string installCommand = null) : base(name, installCommand ?? "install " + name)
        {
        }

        protected override FileInfo Executable => Commands.Chocolaty;

        internal override string Install()
        {
            var wrappedCommand = $"-noprofile -command \"&{{ start-process -FilePath '{Executable.FullName}' -ArgumentList '{InstallCommand} -y' -Wait -Verb RunAs}}\"";

            return Commands.Powershell.Execute(wrappedCommand);
        }
    }
}