using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CodeDeployPack.AppSpecCreation;
using NUnit.Framework;

namespace CodeDeployPack.Test.Unit.AppSpecCreation
{
    [TestFixture]
    public class DiscoverHooksTests
    {
        [Test]
        public void Discover_ReturnsHooksInstance()
        {
            var discoverer = new DiscoverHooks();
            var hooks = discoverer.Discover(new string[0]);
            Assert.That(hooks, Is.Not.Null);
        }

        [Test]
        public void Discover_ThrowsOnNullArgument()
        {
            Assert.Throws<ArgumentNullException>(() => new DiscoverHooks().Discover(null));
        }

        [Test]
        [TestCase("before-install")]
        [TestCase("after-install")]
        [TestCase("application-start")]
        [TestCase("application-stop")]
        [TestCase("validate-service")]
        public void Discover_ThrowsInvalidOperationException_ForMultipleFilesOfSameType(string specialFileName)
        {
            var path1 = Path.Combine("dir", ".deploy", $"{specialFileName}.ps1");
            var path2 = Path.Combine("dir", ".deploy", $"{specialFileName}.bat");
            var files = new[]
            {
                path1,
                path2
            };
            var exception = Assert.Throws<InvalidOperationException>(() => new DiscoverHooks().Discover(files));
            Assert.That(exception.Message, Is.EqualTo($"Unable to add hook '{path2}'. The hook of the same type '{path1}' is already present."));
        }

        [Test]
        public void Discover_ReturnsPopulatedHooksInstanceWithDiscoveredHooks()
        {
            var discoverer = new DiscoverHooks();
            var beforeInstallPath = Path.Combine("target", ".deploy", "before-install.ps1");
            var afterInstallPath = Path.Combine("target", ".deploy", "after-install.ps1");
            var applicationStartPath = Path.Combine("target", ".deploy", "application-start.ps1");
            var applicationStopPath = Path.Combine("target", ".deploy", "application-stop.ps1");
            var validateServicePath = Path.Combine("target", ".deploy", "validate-service.ps1");

            var files = new[]
            {
                beforeInstallPath,
                afterInstallPath,
                applicationStartPath,
                applicationStopPath,
                validateServicePath
            };

            var hooks = discoverer.Discover(files);
            AssertHook(hooks.BeforeInstall, beforeInstallPath);
            AssertHook(hooks.AfterInstall, afterInstallPath);
            AssertHook(hooks.ApplicationStart, applicationStartPath);
            AssertHook(hooks.ApplicationStop, applicationStopPath);
            AssertHook(hooks.ValidateService, validateServicePath);
        }

        [Test]
        public void Discover_LooksForHooksOnlyInspecifiedLocation()
        {
            var discoverer = new DiscoverHooks();
            var beforeInstallPath = Path.Combine("target", "before-install.ps1");
            var afterInstallPath = Path.Combine("target", "after-install.ps1");
            var applicationStartPath = Path.Combine("target", "application-start.ps1");
            var applicationStopPath = Path.Combine("target", "application-stop.ps1");
            var validateServicePath = Path.Combine("target", "validate-service.ps1");

            var files = new[]
            {
                beforeInstallPath,
                afterInstallPath,
                applicationStartPath,
                applicationStopPath,
                validateServicePath
            };

            var hooks = discoverer.Discover(files);
            Assert.That(hooks.BeforeInstall, Is.Empty);
            Assert.That(hooks.AfterInstall, Is.Empty);
            Assert.That(hooks.ApplicationStart, Is.Empty);
            Assert.That(hooks.ApplicationStop, Is.Empty);
            Assert.That(hooks.ValidateService, Is.Empty);
        }

        private void AssertHook(List<Hook> hooks, params string[] expectedPaths)
        {
            Assert.That(hooks, Is.Not.Null);
            Assert.That(hooks.Select(h => h.location), Is.EquivalentTo(expectedPaths));
            Assert.True(hooks.All(x => x.timeout == 180), "Expected default timeout");
        }
    }
}