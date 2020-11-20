using System;
using System.Collections.Generic;
using System.IO;

namespace MSharp.Build.Project
{
    class InputParameterResolver
    {
        readonly string[] _args;
        public InputParameterResolver(string[] args) => _args = args;

        public Dictionary<string, string> Resolve()
        {
            var result = GetInputArgs();

            SetDefaults(result);

            return result;
        }

        Dictionary<string, string> GetInputArgs()
        {
            var inputArgs = new Dictionary<string, string>();
            foreach (var arg in _args)
            {
                switch (arg.Substring(0, 3).ToLower())
                {
                    case "/n:":
                        inputArgs["ProjectName"] = arg.Substring(3);
                        break;
                    case "/t:":
                        inputArgs["TemplateWebAddress"] = arg.Substring(3);
                        break;
                }
            }

            return inputArgs;
        }

        void SetDefaults(Dictionary<string, string> inputArgs)
        {
            if (!inputArgs.ContainsKey("TemplateWebAddress"))
                inputArgs["TemplateWebAddress"] = "https://github.com/Geeksltd/Olive.MvcTemplate/archive/master.zip";

            inputArgs["DownloadedFilesExtractPath"] = Path.Combine(Path.GetTempPath(), "GeeksTemplate");
            // inputArgs["DownloadedFilesExtractPath"] = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));

            inputArgs["DbType"] = "SqlServer";
            inputArgs["ConnectionString"] =
                $"Database={inputArgs["ProjectName"]}.Temp; Server=.\\SQLExpress; Integrated Security=SSPI; MultipleActiveResultSets=True;";
            inputArgs["DestinationDirectory"] = Path.GetDirectoryName(Environment.CommandLine.Split(' ')[0]);
        }
    }
}