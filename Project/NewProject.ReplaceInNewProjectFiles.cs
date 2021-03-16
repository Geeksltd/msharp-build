using Newtonsoft.Json.Linq;
using Olive;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace MSharp.Build.Project
{
    partial class NewProject
    {
        void ReplaceInNewProjectFiles()
        {
            var replacements = GetReplacements();
            ReplaceFilesContents(replacements);

            if (Args.ProjectType == ProjectType.Microservice)
                AddMicroserviceToHubServices();
        }

        Dictionary<string, string> GetReplacements()
        {
            if (Args.ProjectType == ProjectType.Microservice)
            {
                var domain = GetDomain();
                var dic = new Dictionary<string, string>
                {
                    {"MY.MICROSERVICE.NAME", Args.Name},
                    {"MY.SOLUTION", Args.Destination.Name},
                    {"9012", GetPortNumberForMicroservice()}
                };
                if (domain.HasValue())
                {
                    dic.Add("my-solution-domain", domain);
                    dic.Add("mysolution", domain.Remove("."));
                }

                return dic;
            }
            else if (Args.ProjectType == ProjectType.Mvc)
            {
                return new Dictionary<string, string>
                {
                    {"MY.PROJECT.NAME", Args.Name},
                    {"$randomportnumber$", GetPortNumberForMvc()}
                };
            }

            return new Dictionary<string, string>();
        }

        void ReplaceFilesContents(Dictionary<string, string> dic)
        {
            Log("Renaming file contents ...");
            foreach (var file in Args.TempTemplate.GetFiles(includeSubDirectories: true))
            {
                if (Path.GetExtension(file).EndsWith("exe", StringComparison.OrdinalIgnoreCase)) continue;

                var content = File.ReadAllText(file);
                foreach (var d in dic)
                {
                    if (content.Contains(d.Key, caseSensitive: false))
                    {
                        content = content.Replace(d.Key, d.Value);
                        content = content.Replace(d.Key.ToLower(), d.Value.ToLowerOrEmpty());
                    }
                }

                File.WriteAllText(file, content);

                var fileName = Path.GetFileName(file);
                foreach (var d in dic)
                {
                    if (fileName.Contains(d.Key))
                        File.Move(file, file.Replace(d.Key, d.Value));
                }
            }
        }

        string GetPortNumberForMvc() => Args.PortNumber.HasValue() ? Args.PortNumber : GetRandomPortNumber();

        string GetRandomPortNumber() => new Random().Next(2000, 65535).ToString();

        string GetPortNumberForMicroservice()
        {
            if (Args.PortNumber.HasValue())
                return Args.PortNumber;
            var servicePath = GetServicesPath();
            if (servicePath.IsEmpty() || !File.Exists(servicePath))
                return GetRandomPortNumber();

            var services = XElement.Load(servicePath);
            var nextPortNumber = (from srv in services.FirstNode.ElementsAfterSelf()
                                  where srv.Attribute("url") != null && srv.Attribute("url").Value.Replace("://", "").Contains(":")
                                  let y = srv.Attribute("url")?.Value.Replace("://", "").RemoveBefore(":", caseSensitive: true)
                                  where int.TryParse(y, out _)
                                  select Convert.ToInt32(y)).Max() + 1;
            return nextPortNumber.ToString();
        }

        string GetServicesPath()
        {
            var hubDir = Args.Destination;
            while (hubDir?.Parent != null && !Directory.Exists(Path.Combine(hubDir.FullName, "hub")))
                hubDir = hubDir.Parent;

            return Path.Combine(hubDir.FullName, "hub", "website", "services.xml");
        }

        void AddMicroserviceToHubServices()
        {
            var servicePath = GetServicesPath();
            if (servicePath.IsEmpty() || !File.Exists(servicePath))
            {
                ShowWarning("Couldn't find hub services file.");
                return;
            }

            try
            {
                var services = XDocument.Load(servicePath);
                var node = new XElement(Args.Name,
                new XAttribute("url", $"http://localhost:{GetPortNumberForMicroservice()}"),
                new XAttribute("production", $"https://{Args.Name}.{GetDomain()}"));

                services.Root?.AddFirst(node);
                services.Save(servicePath);
            }
            catch
            {
                ShowWarning("Couldn't add microservice to hub services");
            }
        }

        string GetDomain()
        {
            var hubAddress = Path.Combine(Args.Destination.FullName, "hub");

            try
            {
                var appSettingsProductionAllText = File.ReadAllText(Path.Combine(hubAddress, "website", "appsettings.Production.json"));
                var appSettingsProductionJObject = JObject.Parse(appSettingsProductionAllText);
                return appSettingsProductionJObject["Authentication"]["Cookie"]["Domain"].ToString().TrimStart('*').TrimStart('.');
            }
            catch
            {
                ShowWarning("Couldn't find appsettings file in hub");
            }

            return null;
        }
    }
}