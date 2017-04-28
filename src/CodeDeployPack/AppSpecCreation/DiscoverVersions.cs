using System.Reflection;
using CodeDeployPack.Logging;

namespace CodeDeployPack.AppSpecCreation
{
    public class DiscoverVersions : IDiscoverVersions
    {
        private readonly ILog _log;
        private readonly Assembly _assembly;

        public DiscoverVersions(ILog log, Assembly assembly)
        {
            _log = log;
            _assembly = assembly;
        }

        public string GetVersion()
        {
            var informationalVersion = _assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;
            var assemblyVersion = _assembly.GetName().Version?.ToString();
            var version = informationalVersion ?? assemblyVersion;
            _log.LogMessage($"DiscoverVersions: {version}");
            return version;
        }


    }
}