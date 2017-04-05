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
            _fs = new MockFileSystem();
            _fs.AddFile("c:\\abc\\file.txt", "");
            _fs.AddFile("c:\\abc\\bin\\CodeDeployPack.SampleWebApp.dll", "");
            _fs.AddFile("c:\\abc\\bin\\CodeDeployPack.SampleWebApp.pdb", "");
            _fs.AddDirectory("c:\\abc\\obj");
            _fs.AddDirectory("c:\\abc\\bin");

            _fakeZipFileWrapper = new FakeZipFileWrapper(_fs);
            _paramz = new CreateCodeDeployTaskParameters
            {
                ProjectDirectory = "c:\\abc",
                WrittenFiles = new ITaskItem[]
                {
                    new TaskItem {ItemSpec = "file.txt"},
                    new TaskItem {ItemSpec = "bin\\CodeDeployPack.SampleWebApp.dll"},
                    new TaskItem {ItemSpec = "bin\\CodeDeployPack.SampleWebApp.pdb"}
                }
            };

            _cmd = new PackageCommand(_logger, _fs, _paramz, new FakeSpecGenerator(), _fakeZipFileWrapper);
        }

        [Test]
        public void Execute_CreatesTemporaryDirectories()
        {
            _cmd.Execute();

            Assert.That(_fs.AllDirectories.Contains("c:\\abc\\obj\\packing\\"));
            Assert.That(_fs.AllDirectories.Contains("c:\\abc\\obj\\packed\\"));
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

            Assert.That(zipFilePath, Is.EqualTo("c:\\abc\\obj\\packed\\CodeDeploy.zip"));
        }

        [Test]
        public void Execute_GeneratesAppSpecFileInZipRepoRoot()
        {
            _cmd.Execute();

            Assert.That(_fakeZipFileWrapper.FilesThatWouldHaveBeenInTheZip, Does.Contain("appspec.yml"));
        }

        [Test]
        public void Execute_AnyFilesProducedByBuildExistInZip()
        {
            _cmd.Execute();

            Assert.That(_fakeZipFileWrapper.FilesThatWouldHaveBeenInTheZip, Does.Contain("file.txt"));
            Assert.That(_fakeZipFileWrapper.FilesThatWouldHaveBeenInTheZip.Count, Is.EqualTo(4));
        }
    }
}
