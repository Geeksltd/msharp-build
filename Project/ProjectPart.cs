using System.Collections.Generic;

namespace MSharp.Build.Project
{
    abstract class ProjectPart
    {
        public readonly List<string> Logs = new List<string>();

        public abstract void Install(Dictionary<string, string> inputArgs);
    }
}