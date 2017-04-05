using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using CodeDeployPack.PackageCompilation;

namespace CodeDeployPack.Test.Unit.TestDoubles
{
    public class FakeZipFileWrapper : IZipFile
    {
        private readonly MockFileSystem _fs;
        public bool Called => WouldHaveCreated != null;
        public string WouldHaveCreated { get; set; }
        public List<string> FilesThatWouldHaveBeenInTheZip { get; set; } = new List<string>();

        public FakeZipFileWrapper(MockFileSystem fs)
        {
            _fs = fs;
        }

        public void CreateFromDirectory(string src, string dest)
        {
            _fs.AddFile(dest, new MockFileData(new byte[] { }));
            WouldHaveCreated = dest;

            FilesThatWouldHaveBeenInTheZip.AddRange(_fs.AllFiles.Where(x => x.StartsWith(src)).Select(x=>x.Replace(src + "\\", "")));
            FilesThatWouldHaveBeenInTheZip.RemoveAll(string.IsNullOrWhiteSpace);
        }
    }
}