using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using CodeDeployPack.PackageCompilation;
using CodeDeployPack.Test.Unit.TestDoubles;
using Microsoft.Build.Framework;
using NUnit.Framework;

namespace CodeDeployPack.Test.Unit
{
    [TestFixture]
    public class PackageCommandTests
    {
        private PackageCommand _cmd;
        private MockFileSystem _fs;
        private CreateCodeDeployTaskParameters _paramz;
        private FakeLogger _logger;
        private FakeZipFileWrapper _fakeZipFileWrapper;

        [SetUp]
        public void SetUp()
        {
            _logger = new FakeLogger();

            WebAppInFileSystem();
            _fakeZipFileWrapper = new FakeZipFileWrapper(_fs);
            _cmd = new PackageCommand(_logger, _fs, _paramz, new FakeSpecGenerator(), _fakeZipFileWrapper);
        }

        [Test]
        public void Execute_CreatesTemporaryDirectories()
        {
            _cmd.Execute();

            Assert.That(_fs.AllDirectories.Contains("c:\\WebApp\\obj\\packing\\"));
            Assert.That(_fs.AllDirectories.Contains("c:\\WebApp\\obj\\packed\\"));
        }

        [Test]
        public void Execute_ReturnsCreatedZipPath()
        {
            var zipFilePath = _cmd.Execute();

            Assert.That(_fakeZipFileWrapper.WouldHaveCreated, Is.EqualTo(zipFilePath));
        }

        [Test]
        public void Execute_CreatesPackageInObjPackingDirectory()
        {
            var zipFilePath = _cmd.Execute();

            Assert.That(zipFilePath, Is.EqualTo("c:\\WebApp\\obj\\packed\\CodeDeploy.zip"));
        }

        [Test]
        public void Execute_GeneratesAppSpecFileInZipRepoRoot()
        {
            _cmd.Execute();

            Assert.That(_fakeZipFileWrapper.FilesThatWouldHaveBeenInTheZip, Does.Contain("appspec.yml"));
        }

        [Test]
        public void Execute_AnyFilesProducedByBuildExistInZipInAppFolder()
        {
            _cmd.Execute();

            Assert.That(_fakeZipFileWrapper.FilesThatWouldHaveBeenInTheZip, Does.Contain("app\\HtmlPage1.html"));
            Assert.That(_fakeZipFileWrapper.FilesThatWouldHaveBeenInTheZip, Does.Contain("app\\web.config"));
            Assert.That(_fakeZipFileWrapper.FilesThatWouldHaveBeenInTheZip, Does.Contain("app\\bin\\WebApp.dll"));
            Assert.That(_fakeZipFileWrapper.FilesThatWouldHaveBeenInTheZip, Does.Contain("app\\bin\\WebApp.pdb"));
            Assert.That(_fakeZipFileWrapper.FilesThatWouldHaveBeenInTheZip.Count, Is.EqualTo(5));
        }

        private void WebAppInFileSystem()
        {
            _fs = new MockFileSystem(new Dictionary<string, MockFileData>(), "c:\\WebApp");
            _fs.AddFile("c:\\WebApp\\HtmlPage1.html", "");
            _fs.AddFile("c:\\WebApp\\web.config", "");
            _fs.AddFile("c:\\WebApp\\bin\\WebApp.dll", "");
            _fs.AddFile("c:\\WebApp\\bin\\WebApp.pdb", "");

            _paramz = new CreateCodeDeployTaskParameters
            {
                ProjectDirectory = "c:\\WebApp",
                ContentFiles = new ITaskItem[]
                {
                    new TaskItem {ItemSpec = "HtmlPage1.html"},
                    new TaskItem {ItemSpec = "web.config"}
                },
                WrittenFiles = new ITaskItem[]
                {
                    new TaskItem {ItemSpec = "bin\\WebApp.dll"},
                    new TaskItem {ItemSpec = "bin\\WebApp.pdb"}
                },
                OutDir = "bin\\"
            };
        }
    }
}
