using CodeDeployPack.PackageCompilation;

namespace CodeDeployPack.Test.Unit
{
    public class FakeZipFileWrapper : IZipFile
    {
        public bool Called { get; set; }
        public void CreateFromDirectory(string src, string dest) => Called = true;
    }
}