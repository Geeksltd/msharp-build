using MSharp.Build.Installers;

namespace MSharp.Build.Tools
{
    class WebPack : BuildTool
    {
        protected override string Name => "webpack";

        protected override Installer LinuxInstaller => WindowsInstaller;
        protected override Installer WindowsInstaller => new Installers.NodeJs(Name);
    }
}