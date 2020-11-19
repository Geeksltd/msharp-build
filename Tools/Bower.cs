using MSharp.Build.Installers;

namespace MSharp.Build.Tools
{
    class Bower : BuildTool
    {
        protected override string Name => "bower";

        protected override Installer LinuxInstaller => new Installers.NodeJs(Name);

        protected override Installer WindowsInstaller => new Installers.Windows.Chocolaty(Name);
    }
}