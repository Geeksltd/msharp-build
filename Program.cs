using Olive;
using System;
using System.Linq;

namespace MSharp.Build
{
    class Program
    {
        static bool Log;

        static int Main(string[] args)
        {
            Log = args.Contains("-log");

            if (args.Contains("/tools"))
            {
                var buildTools = new BuildTools();
                return Run(() => buildTools.Build(), buildTools.PrintLog);
            }
            else if (args.Contains("/new"))
            {
                Console.WriteLine("TODO: Create a new project. See readme.md");
                return 0;
            }
            else
            {
                var solution = new OliveSolution(publish: args.Contains("-publish"));
                return Run(() => solution.Build(), solution.PrintLog);
            }
        }

        static int Run(Action work, Action printLog)
        {
            try
            {
                work();
                return 0;
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.ToLogString());
                Console.ResetColor();
                return -1;
            }
            finally
            {
                if (Log) printLog();
            }
        }
    }
}