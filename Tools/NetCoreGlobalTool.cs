using MSharp.Build.Installers;

namespace MSharp.Build.Tools
{
    abstract class NetCoreGlobalTool : BuildTool
    {
        protected override Installer LinuxInstaller => WindowsInstaller;
        protected override Installer WindowsInstaller => new Installers.DotNet(Name);
    }
}