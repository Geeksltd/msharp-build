using System;
using System.Linq;
using Olive;

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
                Console.WriteLine("TODO: Create a new project. See readme.md");
                return 0;
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