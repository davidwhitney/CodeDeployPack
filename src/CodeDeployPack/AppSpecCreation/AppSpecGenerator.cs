using System.Collections.Generic;
using System.IO;

namespace CodeDeployPack.AppSpecCreation
{
    public class AppSpecGenerator : IAppSpecGenerator
    {
        private readonly IDiscoverVersions _versionDiscovery;

        public AppSpecGenerator(IDiscoverVersions versionDiscovery)
        {
            _versionDiscovery = versionDiscovery;
        }

        public string CreateAppSpec(Dictionary<string, string> packageContents, CreateCodeDeployTaskParameters parameters)
        {
            var version = _versionDiscovery.GetVersion();
            var basePath = "c:\\app";
            var appName = parameters.ProjectName ?? "";
            var appPath = Path.Combine(basePath, appName, version);
            var template = @"version: {version}
os: windows
files:
  - source: app
    destination: {appPath}\
hooks:";

            return template
                .Replace("{version}", version)
                .Replace("{appPath}", appPath);
        }
    }
}
