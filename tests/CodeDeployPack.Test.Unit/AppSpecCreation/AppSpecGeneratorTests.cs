using System.Collections.Generic;
using CodeDeployPack.AppSpecCreation;
using CodeDeployPack.Test.Unit.TestDoubles;
using NUnit.Framework;

namespace CodeDeployPack.Test.Unit.AppSpecCreation
{
    [TestFixture]
    public class AppSpecGeneratorTests
    {
        private FakeVersionDiscoverer _versionFake;
        private AppSpecGenerator _gen;
        private CreateCodeDeployTaskParameters _parameters;
        private Dictionary<string, string> _contents;
        private FakeDiscoverHooks _hooksFake;

        [SetUp]
        public void SetUp()
        {
            _versionFake = new FakeVersionDiscoverer();
            _hooksFake = new FakeDiscoverHooks();
            _gen = new AppSpecGenerator(_versionFake, _hooksFake);
            _contents = new Dictionary<string, string>();
            _parameters = new CreateCodeDeployTaskParameters();
        }

        [Test]
        public void CreateAppSpec_WithVersionDetected_ReturnsSpecWithVersion()
        {
            _versionFake.Version = "1.9.9.9";

            var spec = _gen.CreateAppSpec(_contents, _parameters);

            Assert.That(spec, Does.Contain("version: 1.9.9.9"));
        }

        [Test]
        public void CreateAppSpec_WithVersionOverridden_ReturnsSpecWithVersion()
        {
            _versionFake.Version = "1.9.9.9";
            _parameters.PackageVersion = "1.0.0";
            var spec = _gen.CreateAppSpec(_contents, _parameters);

            Assert.That(spec, Does.Contain("version: 1.0.0"));
        }

        [Test]
        public void CreateAppSpec_WithProjectName_ProjectNameOutput()
        {
            _parameters.ProjectName = "MyCoolApp";

            var spec = _gen.CreateAppSpec(_contents, _parameters);

            Assert.That(spec, Does.Contain(@"    destination: c:\app\MyCoolApp\0.0.0.0"));
        }

        [Test]
        public void CreateAppSpec_WithSomeHooks_ReturnsSpecWithHooks()
        {
            _hooksFake.Hooks.BeforeInstall.Add(new Hook { location = "loc1", timeout = 120 });
            _hooksFake.Hooks.AfterInstall.Add(new Hook { location = "loc2", timeout = 120 });
            _hooksFake.Hooks.ApplicationStart.Add(new Hook { location = "loc3", timeout = 60 });
            _hooksFake.Hooks.ApplicationStop.Add(new Hook { location = "loc4", timeout = 60 });
            _hooksFake.Hooks.ValidateService.Add(new Hook { location = "loc5", timeout = 30 });

            var spec = _gen.CreateAppSpec(_contents, _parameters);

            Assert.That(spec, Is.EqualTo(@"version: 0.0.0.0
os: windows
files:
  - source: app
    destination: c:\app\0.0.0.0\
hooks:
  BeforeInstall:
    - location: loc1
      timeout: 120
  AfterInstall:
    - location: loc2
      timeout: 120
  ApplicationStart:
    - location: loc3
      timeout: 60
  ApplicationStop:
    - location: loc4
      timeout: 60
  ValidateService:
    - location: loc5
      timeout: 30
"));
        }

        [Test]
        public void CreateAppSpec_WithSomeContents_ReturnsSpec()
        {
            var spec = _gen.CreateAppSpec(_contents, _parameters);

            Assert.That(spec, Is.EqualTo(@"version: 0.0.0.0
os: windows
files:
  - source: app
    destination: c:\app\0.0.0.0\
"));
        }
    }
}
