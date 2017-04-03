using System.IO.Abstractions;
using System.Reflection;
using CodeDeployPack.AppSpecCreation;
using CodeDeployPack.Logging;
using CodeDeployPack.PackageCompilation;
using Microsoft.Build.Framework;

namespace CodeDeployPack
{
    public class CreateCodeDeployPackage : CreateCodeDeployTaskParameters, ITask
    {
        public IBuildEngine BuildEngine { get; set; }
        public ITaskHost HostObject { get; set; }

        private ILog _log;
        private readonly IFileSystem _fileSystem;

        public CreateCodeDeployPackage()
        {
            _fileSystem = new FileSystem();
        }

        public bool Execute()
        {
            _log = new Logger(BuildEngine);
            _log.LogMessage($"CodeDeployPack version: {Assembly.GetExecutingAssembly().GetName().Version}");
            _log.LogMessage("---Arguments---", MessageImportance.Low);
            _log.LogMessage("Content files: " + (ContentFiles ?? new ITaskItem[0]).Length);
            _log.LogMessage("ProjectDirectory: " + ProjectDirectory);
            _log.LogMessage("OutDir: " + OutDir);
            _log.LogMessage("PackageVersion: " + PackageVersion);
            _log.LogMessage("ProjectName: " + ProjectName);
            _log.LogMessage("PrimaryOutputAssembly: " + PrimaryOutputAssembly);
            _log.LogMessage("AppConfigFile: " + AppConfigFile);
            _log.LogMessage("---------------");
            _log.LogMessage("Creating CodeDeploy package...");

            WrittenFiles = WrittenFiles ?? new ITaskItem[0];
            _log.LogMessage("Written files: " + WrittenFiles.Length);

            var appSpecGenerator = new AppSpecGenerator(new DiscoverVersions());
            new PackageCommand(_log, _fileSystem, this, appSpecGenerator).Execute();

            return true;
        }

    }
}
