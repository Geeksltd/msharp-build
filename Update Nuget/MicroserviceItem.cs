using Olive;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MSharp.Build.UpdateNuget
{
    public sealed class MicroserviceItem
    {
        public List<NugetReference> References = new List<NugetReference>();

        static IEnumerable<SolutionProject> StandardProjects
           => Enum.GetValues(typeof(SolutionProject)).OfType<SolutionProject>();

        public void RefreshPackages()
        {
            References = new List<NugetReference>();

            foreach (var project in StandardProjects)
            {
                var packages = project.GetNugetPackages(SolutionFolder)
                    .Where(x => !x.name.StartsWith("Microsoft.AspNetCore."));

                foreach (var p in packages)
                    References.Add(new NugetReference(p.name, p.ver, this, project));
            }
        }

        public void UpdatePackages()
        {
            References.Do(x => x.ShouldUpdate = true);
            UpdateSelectedPackages();
        }

        public string SolutionFolder { get; set; }
        public IUpdateNugetBuilder Builder { get; set; }

        void UpdateSelectedPackages()
        {
            var toUpdate = References.Where(x => x.ShouldUpdate && !x.IsUpToDate);

            foreach (var item in toUpdate)
                item.Update();
        }

        public void LogMessage(string message, string desc = null) => Builder.Log(message);
    }
}