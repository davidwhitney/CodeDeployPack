using CodeDeployPack.AppSpecCreation;

namespace CodeDeployPack.Test.Unit.TestDoubles
{
    public class FakeVersionDiscoverer : IDiscoverVersions
    {
        public string Version { get; set; } = "0.0.0.0";
        public string GetVersion() => Version;
    }
}