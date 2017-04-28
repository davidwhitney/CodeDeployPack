using System.Collections.Generic;
using YamlDotNet.Serialization;

namespace CodeDeployPack.AppSpecCreation
{
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
}
