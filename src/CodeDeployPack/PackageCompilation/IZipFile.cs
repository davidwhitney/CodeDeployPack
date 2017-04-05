namespace CodeDeployPack.PackageCompilation
{
    public interface IZipFile
    {
        void CreateFromDirectory(string src, string dest);
    }
}