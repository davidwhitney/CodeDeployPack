using System.Collections.Generic;
using Microsoft.Build.Framework;

namespace CodeDeployPack.PackageCompilation.SpecialFileTypes
{
    public interface IMapFiles
    {
        bool IsApplicable(string sourceFilePath, string destinationPath);
        void Process(Dictionary<string, string> fileMap, ITaskItem sourceFile, string sourceFilePath, string destinationPath);
    }
}