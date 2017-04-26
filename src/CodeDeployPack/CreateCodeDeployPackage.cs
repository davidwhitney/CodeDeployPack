using System.Reflection;
using CodeDeployPack.Logging;
using CodeDeployPack.PackageCompilation;
using Microsoft.Build.Framework;

namespace CodeDeployPack
{
    public class CreateCodeDeployPackage : CreateCodeDeployTaskParameters, ITask
    {
        public IBuildEngine BuildEngine { get; set; }
        public ITaskHost HostObject { get; set; }

        public bool Execute()
        {
            var log = new Logger(BuildEngine);
            log.LogMessage($"CodeDeployPack version: {Assembly.GetExecutingAssembly().GetName().Version}");
            log.LogMessage("---Arguments---", MessageImportance.Low);
            log.LogMessage("Content files: " + (ContentFiles ?? new ITaskItem[0]).Length);
            log.LogMessage("ProjectDirectory: " + ProjectDirectory);
            log.LogMessage("OutDir: " + OutDir);
            log.LogMessage("PackageVersion: " + PackageVersion);
            log.LogMessage("ProjectName: " + ProjectName);
            log.LogMessage("PrimaryOutputAssembly: " + PrimaryOutputAssembly);
            log.LogMessage("AppConfigFile: " + AppConfigFile);
            log.LogMessage("---------------");
            log.LogMessage("Creating CodeDeploy package...");

            WrittenFiles = WrittenFiles ?? new ITaskItem[0];
            log.LogMessage("Written files: " + WrittenFiles.Length);

            log.LogMessage("Written files:");
            foreach (var file in WrittenFiles)
            {
                log.LogMessage($"ItemSpec: {file.ItemSpec}, {file.GetMetadata("Link")}");
            }

            log.LogMessage("Content files:");
            foreach (var file in ContentFiles ?? new ITaskItem[0])
            {
                log.LogMessage($"ItemSpec: {file.ItemSpec}, {file.GetMetadata("Link")}");
            }

            PackageCommandFactory.Manufacture(log, this).Execute();
            return true;
        }
    }
}
