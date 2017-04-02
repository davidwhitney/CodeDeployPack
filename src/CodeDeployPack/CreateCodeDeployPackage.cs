using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
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

        private ILog _log;
        private readonly FileSystem _fileSystem;

        public CreateCodeDeployPackage()
        {
            _fileSystem = new FileSystem();
        }

        public bool Execute()
        {
            _log = new Logger(BuildEngine);

            LogDiagnostics();
            _log.LogMessage("Creating CodeDeploy package...");

            WrittenFiles = WrittenFiles ?? new ITaskItem[0];
            _log.LogMessage("Written files: " + WrittenFiles.Length);

            var packing = CreateEmptyOutputDirectory("packing");
            var packed = CreateEmptyOutputDirectory("packed");

            var binaries = new List<ITaskItem>(WrittenFiles);

            var packagers = new List<AppPackagerBase>
            {
                new WebApplicationPackager(_log, _fileSystem),
                new ExecutableAppPackager(_log, _fileSystem)
            };

            var packager = packagers.First(x => x.IsApplicable(ContentFiles));
            packager.Package(this, ContentFiles, binaries, ProjectDirectory, OutDir);

            var filesToMap = packager.IndexedFiles;

            foreach (var file in filesToMap)
            {
                _log.LogMessage($"{file.Key} => {file.Value}");

                var packingDirectory = Path.Combine(ProjectDirectory, "obj", "packing");
                var target = Path.Combine(packingDirectory, file.Value);
                EnsureTargetDirectoryExists(target);

                _log.LogMessage($"Copying '{file.Key}' => '{target}'");
                _fileSystem.File.Copy(file.Key, target, true);
            }

            return true;
        }

        private void EnsureTargetDirectoryExists(string target)
        {
            var directoryParts = Path.GetDirectoryName(target).Split('\\').ToList();
            directoryParts[0] = directoryParts[0] + "\\";

            var current = "";
            foreach (var part in directoryParts)
            {
                current = Path.Combine(current, part);
                if (!_fileSystem.Directory.Exists(current))
                {
                    _fileSystem.Directory.CreateDirectory(current);
                }
            }
        }

        private void LogDiagnostics()
        {
            _log.LogMessage($"CodeDeployPack version: {Assembly.GetExecutingAssembly().GetName().Version}");
            _log.LogMessage("---Arguments---", MessageImportance.Low);
            _log.LogMessage("Content files: " + (ContentFiles ?? new ITaskItem[0]).Length, MessageImportance.High);
            _log.LogMessage("ProjectDirectory: " + ProjectDirectory, MessageImportance.High);
            _log.LogMessage("OutDir: " + OutDir, MessageImportance.High);
            _log.LogMessage("PackageVersion: " + PackageVersion, MessageImportance.High);
            _log.LogMessage("ProjectName: " + ProjectName, MessageImportance.High);
            _log.LogMessage("PrimaryOutputAssembly: " + PrimaryOutputAssembly, MessageImportance.High);
            _log.LogMessage("AppConfigFile: " + AppConfigFile, MessageImportance.High);
            _log.LogMessage("---------------", MessageImportance.High);
        }

        private string CreateEmptyOutputDirectory(string name)
        {
            var temp = Path.Combine(ProjectDirectory, "obj", name);
            _log.LogMessage("Create directory: " + temp, MessageImportance.Low);

            if (_fileSystem.Directory.Exists(temp))
            {
                _fileSystem.Directory.Delete(temp, true);
            }
            
            _fileSystem.Directory.CreateDirectory(temp);
            return temp;
        }
    }
}
