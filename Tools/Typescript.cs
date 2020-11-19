using MSharp.Build.Installers;

namespace MSharp.Build.Tools
{
    class Typescript : BuildTool
    {
        protected override string Name => "typescript";
        protected override Installer LinuxInstaller => WindowsInstaller;
        protected override Installer WindowsInstaller => new Installers.NodeJs(Name);
    }
}