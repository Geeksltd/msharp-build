using System.IO;

namespace MSharp.Build.Installers
{
    class Yarn : Installer
    {
        public Yarn(string name, string installCommand = null) : base(name, installCommand ?? "global add " + name)
        {
        }

        protected override FileInfo Executable => Commands.Yarn;
    }
}