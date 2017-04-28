using System.IO.Abstractions;
using System.Reflection;
using CodeDeployPack.AppSpecCreation;
using CodeDeployPack.Logging;

namespace CodeDeployPack.PackageCompilation
{
    public static class PackageCommandFactory
    {
        public static PackageCommand Manufacture(ILog log, CreateCodeDeployTaskParameters parameters)
        {
            var fileSystem = new FileSystem();
            var assemblyFile = Assembly.LoadFile(parameters.PrimaryOutputAssembly);
            var envFactory = new PackingEnvironmentVariablesFactory(log, parameters, fileSystem);

            var envVariables = envFactory.GetConfig();

            var hooksDiscovery = new DiscoverHooks(envVariables);
            var versionDiscovery = new DiscoverVersions(log, assemblyFile);
            var appSpecGenerator = new AppSpecGenerator(versionDiscovery, hooksDiscovery);
            var zipFileWrapper = new ZipFileWrapper();
            return new PackageCommand(log, fileSystem, parameters, appSpecGenerator, zipFileWrapper, envVariables);
        }
    }
}