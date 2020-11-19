using MSharp.Build.UpdateNuget;
using Olive;
using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml.Linq;

namespace MSharp.Build
{
    public interface IUpdateNugetBuilder { void Log(string message, [CallerMemberName] string step = ""); }

    class UpdateNugetBuilder : Builder, IUpdateNugetBuilder
    {
        MicroserviceItem Solution;

        protected override void AddTasks()
        {
            Add(() => PrepareSolution());
            Add(() => RefreshNugets());
            Add(() => UpdateNugets());
        }
        void PrepareSolution()
        {
            var root = Environment.CurrentDirectory.AsDirectory();

            Solution = new MicroserviceItem { SolutionFolder = root.FullName, Builder = this };
        }

        void RefreshNugets()
        {
            Solution.RefreshPackages();
        }

        void UpdateNugets()
        {
            Solution.UpdatePackages();
        }

        void IUpdateNugetBuilder.Log(string message, [CallerMemberName] string step = "") => base.Log(message);
    }
}