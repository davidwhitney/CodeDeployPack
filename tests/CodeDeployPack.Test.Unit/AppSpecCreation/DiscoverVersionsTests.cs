using System;
using System.Collections.Generic;
using System.Reflection;
using CodeDeployPack.AppSpecCreation;
using CodeDeployPack.Test.Unit.TestDoubles;
using NUnit.Framework;

namespace CodeDeployPack.Test.Unit.AppSpecCreation
{
    [TestFixture]
    public class DiscoverVersionsTests
    {
        private DiscoverVersions _versionDiscoverer;

        [Test]
        public void GetVersion_WithAssemblyVersion_ReturnVersionString()
        {
            var fakeLogger = new FakeLogger();
            var fakeAssembly = new FakeAssembly(new Version(1,0,0,0));
            _versionDiscoverer = new DiscoverVersions(fakeLogger, fakeAssembly);

            Assert.That(_versionDiscoverer.GetVersion(), Is.EqualTo("1.0.0.0"));
        }

        [Test]
        public void GetVersion_WithAssemblyInformationalVersion_ReturnVersionString()
        {
            var fakeLogger = new FakeLogger();
            var informationalVersion = "1.0.0-local+dirty";
            var fakeAssembly = new FakeAssembly(new AssemblyInformationalVersionAttribute(informationalVersion));
            _versionDiscoverer = new DiscoverVersions(fakeLogger, fakeAssembly);

            Assert.That(_versionDiscoverer.GetVersion(), Is.EqualTo(informationalVersion));
        }

        [Test]
        public void GetVersion_WithAssemblyAndInformationalVersion_ReturnInformationalVersion()
        {
            var fakeLogger = new FakeLogger();
            var informationalVersion = "1.0.0-local+dirty";
            var fakeAssembly = new FakeAssembly(new Version(1, 0, 0, 0), new AssemblyInformationalVersionAttribute(informationalVersion));
            _versionDiscoverer = new DiscoverVersions(fakeLogger, fakeAssembly);

            Assert.That(_versionDiscoverer.GetVersion(), Is.EqualTo(informationalVersion));
        }
    }
}