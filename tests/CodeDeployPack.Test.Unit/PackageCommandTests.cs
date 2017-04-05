using System;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeDeployPack.Logging;
using CodeDeployPack.PackageCompilation;
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
            _fs = new MockFileSystem();
            _fs.AddDirectory("c:\\abc\\obj");

            _logger = new FakeLogger();
            _fakeZipFileWrapper = new FakeZipFileWrapper();
            _paramz = new CreateCodeDeployTaskParameters
            {
                ProjectDirectory = "c:\\abc"
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
        public void Execute_()
        {
            
        }
    }
}
