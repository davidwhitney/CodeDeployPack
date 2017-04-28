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

        [SetUp]
        public void SetUp()
        {
            _versionFake = new FakeVersionDiscoverer();
            _gen = new AppSpecGenerator(_versionFake);
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
        public void CreateAppSpec_WithProjectName_ProjectNameOutput()
        {
            _parameters.ProjectName = "MyCoolApp";

            var spec = _gen.CreateAppSpec(_contents, _parameters);

            Assert.That(spec, Does.Contain(@"    destination: c:\app\MyCoolApp\0.0.0.0"));
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
hooks:"));
        }
    }
}
