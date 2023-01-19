﻿using System;
using System.IO;
using System.Linq;
using Olive;

namespace MSharp.Build
{
    class OliveSolution : Builder
    {
        public static DirectoryInfo Root, Lib;
        readonly bool Publish;
        public bool IsDotNetCore, IsWebForms;

        static OliveSolution()
        {
            Root = Environment.CurrentDirectory.AsDirectory();
            Lib = Root.CreateSubdirectory(@"M#\lib");
        }

        public OliveSolution(bool publish)
        {
            Console.WriteLine("Build started for: " + Root.FullName);
            Console.WriteLine();

            Publish = publish;
            IsDotNetCore = IsProjectDotNetCore();
            IsWebForms = Root.GetSubDirectory("Website").GetFiles("*.csproj").None();

            if (IsDotNetCore)
            {
                Lib = Lib.GetDirectories()
                       .Where(x => x.Name.StartsWith("net") && x.Name.Any(c => c.IsDigit()))
                       .WithMax(x => x.Name.Where(c => c.IsDigit()).ToString(""))
                       ?? throw new Exception("netcoreapp3.1 or net6.0 or similar folder is not found in " + Lib.FullName);
            }
        }

        bool IsProjectDotNetCore()
        {
            return Lib.Parent.GetSubDirectory("Model").GetFile("#Model.csproj").ReadAllText()
                 .Contains("<TargetFramework>net");
        }

        protected override void AddTasks()
        {
            Add(() => BuildRuntimeConfigJson());
            Add(() => RestoreNuget());
            Add(() => BuildMSharpModel());
            Add(() => MSharpGenerateModel());
            Add(() => BuildAppDomain());
            Add(() => BuildMSharpUI());
            Add(() => MSharpGenerateUI());
            Add(() => YarnInstall());
            Add(() => TypescriptCompile());
            Add(() => SassCompile());
            Add(() => BuildAppWebsite());
        }

        void BuildRuntimeConfigJson()
        {
            var version = Lib.Name;
            var runtime = version.SkipWhile(v => v.IsLetter()).ToString("");

            var json = $@"{{  
   ""runtimeOptions"":{{  
      ""tfm"":""{version}"",
      ""framework"":{{  
         ""name"":""Microsoft.NETCore.App"",
         ""version"":""{runtime}.0""
      }}
   }}
}}";
            File.WriteAllText(Path.Combine(Lib.FullName, "MSharp.DSL.runtimeconfig.json"), json);
        }

        void RestoreNuget()
        {
            if (!IsDotNetCore)
                Commands.FindExe("nuget").Execute("restore",
                configuration: x => x.StartInfo.WorkingDirectory = Root.FullName);
        }

        void BuildMSharpModel() => DotnetBuild(Path.Combine("M#", "Model"));

        void BuildAppDomain() => DotnetBuild("Domain");

        void BuildMSharpUI() => DotnetBuild(Path.Combine("M#", "UI"));

        void BuildAppWebsite()
        {
            if (IsWebForms)
            {
                RestorePackagesConfig("Website");
                CopyDllsToWebsite();
            }
            else DotnetBuild("Website", $"publish -o ..{Path.PathSeparator}publish".OnlyWhen(Publish));
        }

        void CopyDllsToWebsite()
        {
            var bin = "Website/bin".AsDirectory();

            var dllPaths = from file in bin.GetFiles("*.refresh")
                           let source = Root.GetSubDirectory("Website").GetFile(file.ReadAllText())
                           let item = new
                           {
                               Source = source,
                               Destination = bin.GetFile(source.Name)
                           }
                           where item.Destination.Exists == false
                           select item;

            dllPaths.Do(p => File.Copy(p.Source.FullName, p.Destination.FullName, true));
        }

        const string PACKAGES_DIRECTORY = "packages";
        void RestorePackagesConfig(string folder)
        {
            var packages = Folder(folder).AsDirectory().GetFile("packages.config");
            if (packages.Exists())
            {
                Commands.FindExe("nuget").Execute("restore " + folder + " -packagesdirectory " + Root.GetOrCreateSubDirectory(PACKAGES_DIRECTORY).FullName,
              configuration: x => x.StartInfo.WorkingDirectory = Root.FullName);
            }
        }

        FileInfo GetPackages(string folder) => Folder(folder).AsDirectory().GetFile("packages.config");

        string GetProjectSolution() => Root.GetFiles("*.sln")[0].FullName;

        void DotnetBuild(string folder, string command = null)
        {
            if (IsDotNetCore) DotnetCoreBuild(folder, command);
            else
            {
                RestorePackagesConfig(folder);

                var solution = GetProjectSolution();
                var projName = folder;
                var project = folder.AsDirectory().GetFiles("*.csproj")[0].FullName;
                if (folder.StartsWith("M#")) projName = "#" + folder.Substring(3);

                var dep = " /p:BuildProjectReferences=false".OnlyWhen(folder.StartsWith("M#"));

                Commands.FindExe("msbuild").Execute($"\"{project}\" -v:m",
                    configuration: x => x.StartInfo.EnvironmentVariables.Add("MSHARP_BUILD", "FULL"));
            }
        }

        void DotnetCoreBuild(string folder, string command = null)
        {
            if (command.IsEmpty()) command = $"build {folder.AsDirectory().GetFiles("*.csproj")[0].FullName} -v q";

            var log = Commands.DotNet.Execute(command,
                configuration: x =>
                {
                    x.StartInfo.WorkingDirectory = Folder(folder);
                    x.StartInfo.EnvironmentVariables.Add("MSHARP_BUILD", "FULL");
                });

            Log(log);
        }

        void MSharpGenerateModel() => RunMSharpBuild("/build /model /no-domain");

        void MSharpGenerateUI() => RunMSharpBuild("/build /ui");

        void RunMSharpBuild(string command)
        {
            string log;
            if (IsDotNetCore)
            {
                log = Commands.DotNet.Execute($"msharp.dsl.dll " + command,
                   configuration: x => x.StartInfo.WorkingDirectory = Lib.FullName);
            }
            else
            {
                log = Lib.GetFile("MSharp.dsl.exe").Execute(command, configuration: x => x.StartInfo.WorkingDirectory = Lib.FullName);
            }

            Log(log);
        }

        void YarnInstall()
        {
            var log = Commands.Yarn.Execute("install",
                configuration: x => x.StartInfo.WorkingDirectory = Folder("Website"));
            Log(log);
        }

        void TypescriptCompile()
        {
            var log = Commands.TypeScript.Execute("",
                configuration: x => x.StartInfo.WorkingDirectory = Folder("Website"));
            Log(log);
        }

        void SassCompile()
        {
            if (!IsDotNetCore) return;

            var exe = Folder(Path.Combine("Website", "wwwroot", "Styles", "Build", "SassCompiler.exe")).AsFile();

            var log = "SKIPPED! " + exe.FullName + " file does not exist";

            var args = "\"" + Folder(Path.Combine("Website", "CompilerConfig.json")) + "\"";
            if (exe.Exists())
                log = exe.Execute(args);

            Log(log);
        }

        string Folder(string relative) => Root.GetSubDirectory(relative).FullName;
    }
}