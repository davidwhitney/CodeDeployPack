using System.Collections.Generic;
using CodeDeployPack.AppSpecCreation;

namespace CodeDeployPack.Test.Unit.TestDoubles
{
    internal class FakeDiscoverHooks : IDiscoverHooks
    {
        public Hooks Discover(IEnumerable<string> destinationPaths)
        {
            return Hooks;
        }

        public Hooks Hooks { get; set; }=new Hooks();
    }
}