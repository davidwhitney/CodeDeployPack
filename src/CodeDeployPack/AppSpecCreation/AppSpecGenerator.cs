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
            var appSpec = new AppSpec();

            return new Serializer().Serialize(appSpec);
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
