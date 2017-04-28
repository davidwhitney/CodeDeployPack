using System.IO.Abstractions;
using CodeDeployPack.AppSpecCreation;
using CodeDeployPack.Logging;

namespace CodeDeployPack.PackageCompilation
{
    public static class PackageCommandFactory
    {
        public static PackageCommand Manufacture(ILog log, CreateCodeDeployTaskParameters parameters)
        {
            var fileSystem = new FileSystem();
            var envFactory = new PackingEnvironmentVariablesFactory(log, parameters, fileSystem);

            var envVariables = envFactory.GetConfig();

            var versionDiscovery = new DiscoverVersions();
            var hooksDiscovery = new DiscoverHooks(envVariables);
            var appSpecGenerator = new AppSpecGenerator(versionDiscovery, hooksDiscovery);
            var zipFileWrapper = new ZipFileWrapper();
            return new PackageCommand(log, fileSystem, parameters, appSpecGenerator, zipFileWrapper, envVariables);
        }
    }
}