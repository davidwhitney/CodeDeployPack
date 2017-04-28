using System.IO;
using System.IO.Abstractions;
using CodeDeployPack.Logging;
using Microsoft.Build.Framework;

namespace CodeDeployPack.PackageCompilation
{
    public class PackingEnvironmentVariablesFactory
    {
        private readonly ILog _log;
        private readonly CreateCodeDeployTaskParameters _parameters;
        private readonly IFileSystem _fileSystem;

        public PackingEnvironmentVariablesFactory(ILog log, CreateCodeDeployTaskParameters parameters, IFileSystem fileSystem)
        {
            _log = log;
            _parameters = parameters;
            _fileSystem = fileSystem;
        }

        public PackingEnvironmentVariables GetConfig()
        {
            var locationOfAppInArchive = "app";
            var packing = CreateEmptyOutputDirectory("packing");
            var packed = CreateEmptyOutputDirectory("packed");
            CreateEmptyOutputDirectory("packing\\app");

            var packingDirectory = Path.Combine(_parameters.ProjectDirectory, "obj", "packing");
            var appRootDirectory = Path.Combine(_parameters.ProjectDirectory, "obj", "packing", locationOfAppInArchive);

            return new PackingEnvironmentVariables
            {
                LocationOfAppInArchive = locationOfAppInArchive,
                Packing = packing,
                Packed = packed,
                PackingDirectory = packingDirectory,
                AppRootDirectory = appRootDirectory
            };
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