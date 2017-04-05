using System.Collections.Generic;
using YamlDotNet.Serialization;

namespace CodeDeployPack.AppSpecCreation
{
    public interface IAppSpecGenerator
    {
        string CreateAppSpec(Dictionary<string, string> packageContents);
    }

    public class AppSpecGenerator : IAppSpecGenerator
    {
        private readonly IDiscoverVersions _versionDiscovery;

        public AppSpecGenerator(IDiscoverVersions versionDiscovery)
        {
            _versionDiscovery = versionDiscovery;
        }

        public string CreateAppSpec(Dictionary<string, string> packageContents)
        {
            var packageId = "";

            var appSpec = new AppSpec
            {
                files = new List<FileEntry>
                {
                    new FileEntry
                    {
                        source = "app",
                        destination = $"c:\\CodeDeploy\\{packageId}"
                    }
                }
            };

            return new SerializerBuilder().EmitDefaults().Build().Serialize(appSpec);
        }
    }

    public interface IDiscoverVersions
    {
        string GetVersion();
    }

    public class DiscoverVersions : IDiscoverVersions
    {
        public string GetVersion()
        {
            return "1.0.0.0";
        }
    }
}
