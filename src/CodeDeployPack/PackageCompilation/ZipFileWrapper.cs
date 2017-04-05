using System.IO.Compression;

namespace CodeDeployPack.PackageCompilation
{
    public class ZipFileWrapper : IZipFile
    {
        public void CreateFromDirectory(string src, string dest)
        {
            ZipFile.CreateFromDirectory(src, dest);
        }
    }
}