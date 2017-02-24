using System;
using Microsoft.Build.Framework;

namespace CodeDeployPack
{
    public class CreateCodeDeployPackage : ITask
    {
        public IBuildEngine BuildEngine { get; set; }
        public ITaskHost HostObject { get; set; }

        public bool Execute()
        {
            throw new NotImplementedException();
        }
    }
}
