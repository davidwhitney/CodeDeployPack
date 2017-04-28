using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using CodeDeployPack.AppSpecCreation;
using CodeDeployPack.Logging;
using Microsoft.Build.Framework;

namespace CodeDeployPack.PackageCompilation
{
    public class PackageCommand
    {
        private readonly ILog _log;
        private readonly IFileSystem _fileSystem;
        private readonly CreateCodeDeployTaskParameters _parameters;
        private readonly IAppSpecGenerator _appSpecGenerator;
        private readonly IZipFile _zipFile;

        public PackageCommand(ILog log, IFileSystem fileSystem, CreateCodeDeployTaskParameters parameters,
                              IAppSpecGenerator appSpecGenerator, IZipFile zipFile)
        {
            _log = log;
            _fileSystem = fileSystem;
            _parameters = parameters;
            _appSpecGenerator = appSpecGenerator;
            _zipFile = zipFile;
        }

        public string Execute()
        {
            var packing = CreateEmptyOutputDirectory("packing");
            var packed = CreateEmptyOutputDirectory("packed");
            CreateEmptyOutputDirectory("packing\\app");

            var packingDirectory = Path.Combine(_parameters.ProjectDirectory, "obj", "packing");
            var appRootDirectory = Path.Combine(_parameters.ProjectDirectory, "obj", "packing", "app");

            var writtenFiles = _parameters.WrittenFiles ?? new ITaskItem[0];
            var binaries = new List<ITaskItem>(writtenFiles);

            var packagers = new List<AppPackagerBase>
            {
                new WebApplicationPackager(_log, _fileSystem),
                new ExecutableAppPackager(_log, _fileSystem)
            };

            var packager = packagers.First(x => x.IsApplicable(_parameters.ContentFiles));
            packager.Package(_parameters, _parameters.ContentFiles, binaries, _parameters.ProjectDirectory, _parameters.OutDir);

            var specFile = _appSpecGenerator.CreateAppSpec(packager.IndexedFiles, _parameters);
            _fileSystem.File.WriteAllText(Path.Combine(packingDirectory, "appspec.yml"), specFile);

            StageFiles(appRootDirectory, packager);

            var zipTargetPath = Path.Combine(packed, "CodeDeploy.zip");
            _zipFile.CreateFromDirectory(packing, zipTargetPath);

            return zipTargetPath;
        }

        private void StageFiles(string destination, AppPackagerBase packager)
        {
            foreach (var file in packager.IndexedFiles)
            {
                var target = Path.Combine(destination, file.Value);
                EnsureTargetDirectoryExists(target);

                _log.LogMessage($"Copying '{file.Key}' => '{target}'");
                _fileSystem.File.Copy(file.Key, target, true);
            }
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

        private string CreateEmptyOutputDirectory(string name)
        {
            var temp = Path.Combine(_parameters.ProjectDirectory, "obj", name);
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