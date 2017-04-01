using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Build.Framework;

namespace CodeDeployPack
{
    public class GetAssemblyVersionInfo : ITask
    {
        public IBuildEngine BuildEngine { get; set; }
        public ITaskHost HostObject { get; set; }

        public bool UseFileVersion { get; set; }
        public bool UseProductVersion { get; set; }

        [Required]
        public ITaskItem[] AssemblyFiles { get; set; }

        [Output]
        public ITaskItem[] AssemblyVersionInfo { get; set; }

        public bool Execute()
        {
            return true;
        }
    }
}