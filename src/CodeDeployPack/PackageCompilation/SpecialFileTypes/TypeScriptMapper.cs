using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Build.Framework;

namespace CodeDeployPack.PackageCompilation.SpecialFileTypes
{
    public class TypeScriptMapper : IMapFiles
    {
        public bool IsApplicable(ITaskItem sourceFile, string sourceFilePath, string destinationPath) => 
            string.Equals(Path.GetExtension(sourceFilePath), ".ts", StringComparison.OrdinalIgnoreCase);

        public void Process(Dictionary<string, string> indexedFiles, ITaskItem sourceFile, string destinationPath)
        {

            //var isTypeScript = string.Equals(Path.GetExtension(sourceFilePath), ".ts", StringComparison.OrdinalIgnoreCase);
            //if (isTypeScript)
            //{
            //    if (parameters.IncludeTypeScriptSourceFiles)
            //    {
            //        files.Add(new XElement("file",
            //            new XAttribute("src", sourceFilePath),
            //            new XAttribute("target", destinationPath)
            //        ));

            //        Log.LogMessage("Added file: " + destinationPath, MessageImportance.Normal);
            //    }

            //    var changedSource = Path.ChangeExtension(sourceFilePath, ".js");
            //    var changedDestination = Path.ChangeExtension(destinationPath, ".js");
            //    if (_fs.FileExists(changedSource))
            //    {
            //        files.Add(new XElement("file",
            //            new XAttribute("src", changedSource),
            //            new XAttribute("target", changedDestination)
            //        ));

            //        Log.LogMessage("Added file: " + changedDestination, MessageImportance.Normal);
            //    }
            //}
        }
    }
}
