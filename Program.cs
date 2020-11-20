using MSharp.Build.Project;
using Olive;
using System;
using System.Linq;

namespace MSharp.Build
{
    class Program
    {
        static int Main(string[] args)
        {
            Builder.ShouldLog = args.Contains("-log");

            if (args.Contains("/tools"))
            {
                return new BuildTools().Execute();
            }
            else if (args.Contains("/new"))
            {
                return new NewProject(new NewProjectArgs(args)).Execute();
            }
            else if (args.Contains("/update-nuget"))
            {
                return new UpdateNugetBuilder().Execute();
            }
            else
            {
                return new OliveSolution(publish: args.Contains("-publish")).Execute();
            }
        }
    }
}