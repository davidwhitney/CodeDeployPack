using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Build.Framework;

namespace CodeDeployPack.PackageCompilation.SpecialFileTypes
{
    public class AppConfigMapper : IMapFiles
    {
        public bool IsApplicable(ITaskItem sourceFile, string sourceFilePath, string destinationPath) => 
            string.Equals(Path.GetFileName(destinationPath), "app.config", StringComparison.OrdinalIgnoreCase);

        public void Process(Dictionary<string, string> indexedFiles, ITaskItem sourceFile, string destinationPath)
        {
            //var fileName = Path.GetFileName(destinationPath);
            //if (string.Equals(fileName, "app.config", StringComparison.OrdinalIgnoreCase))
            //{
            //    if (_fs.File.Exists(parameters.AppConfigFile))
            //    {
            //        var configFileName = Path.GetFileName(parameters.AppConfigFile);
            //        destinationPath = Path.GetDirectoryName(destinationPath);
            //        destinationPath = Path.Combine(destinationPath, configFileName);
            //        files.Add(new XElement("file",
            //            new XAttribute("src", parameters.AppConfigFile),
            //            new XAttribute("target", destinationPath)
            //        ));

            //        Log.LogMessage("Added file: " + destinationPath, MessageImportance.Normal);
            //    }
            //    continue;
            //}
        }
    }
}
