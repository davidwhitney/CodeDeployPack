using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CodeDeployPack.AppSpecCreation
{
    public class AppSpecGenerator : IAppSpecGenerator
    {
        private readonly IDiscoverVersions _versionDiscovery;
        private readonly IDiscoverHooks _hooksDiscovery;

        public AppSpecGenerator(IDiscoverVersions versionDiscovery, IDiscoverHooks hooksDiscovery)
        {
            _versionDiscovery = versionDiscovery;
            _hooksDiscovery = hooksDiscovery;
        }

        public string CreateAppSpec(Dictionary<string, string> packageContents, CreateCodeDeployTaskParameters parameters)
        {
            var version = parameters.PackageVersion ?? _versionDiscovery.GetVersion();
            var basePath = "c:\\app";
            var appName = parameters.ProjectName ?? "";
            var appPath = Path.Combine(basePath, appName, version);
            var hooks = _hooksDiscovery.Discover(packageContents.Values);
            var template = @"version: {version}
os: windows
files:
  - source: app
    destination: {appPath}\
{hooks}";

            return template
                .Replace("{version}", version)
                .Replace("{appPath}", appPath)
                .Replace("{hooks}", ToYaml(hooks));
        }

        private string ToYaml(Hooks hooks)
        {
            var builder = new StringBuilder();

            void ToYaml(List<Hook> list, string name)
            {
                if (!list.Any())
                    return;
                builder.AppendLine($"  {name}:");
                foreach (var hook in list)
                {
                    builder.AppendLine($"    - location: {hook.location}")
                        .AppendLine($"      timeout: {hook.timeout}");
                }
            }

            ToYaml(hooks.BeforeInstall, nameof(hooks.BeforeInstall));
            ToYaml(hooks.AfterInstall, nameof(hooks.AfterInstall));
            ToYaml(hooks.ApplicationStart, nameof(hooks.ApplicationStart));
            ToYaml(hooks.ApplicationStop, nameof(hooks.ApplicationStop));
            ToYaml(hooks.ValidateService, nameof(hooks.ValidateService));
            var content = builder.ToString();
            return content.Any() ? $"hooks:{Environment.NewLine}{content}" : "";
        }
    }
}
