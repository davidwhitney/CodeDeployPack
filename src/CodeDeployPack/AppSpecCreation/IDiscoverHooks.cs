using System.Collections.Generic;

namespace CodeDeployPack.AppSpecCreation
{
    public interface IDiscoverHooks
    {
        Hooks Discover(IEnumerable<string> destinationPaths);
    }
}