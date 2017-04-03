using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using CodeDeployPack.Logging;
using Microsoft.Build.Framework;

namespace CodeDeployPack.PackageCompilation.SpecialFileTypes
{
    public class TypeScriptMapper : IMapFiles
    {
        private readonly CreateCodeDeployTaskParameters _parameters;
        private readonly IFileSystem _fs;
        private readonly ILog _log;

        public TypeScriptMapper(CreateCodeDeployTaskParameters parameters, IFileSystem fs, ILog log)
        {
            _parameters = parameters;
            _fs = fs;
            _log = log;
        }

        public bool IsApplicable(string sourceFilePath, string destinationPath) => 
            string.Equals(Path.GetExtension(sourceFilePath), ".ts", StringComparison.OrdinalIgnoreCase);

        public void Process(Dictionary<string, string> indexedFiles, ITaskItem sourceFile, string sourceFilePath, string destinationPath)
        {

            var isTypeScript = string.Equals(Path.GetExtension(sourceFilePath), ".ts", StringComparison.OrdinalIgnoreCase);
            if (isTypeScript)
            {
                if (_parameters.IncludeTypeScriptSourceFiles)
                {
                    indexedFiles.Add(sourceFilePath, destinationPath);
                }
                
                var changedSource = Path.ChangeExtension(sourceFilePath, ".js");
                if (_fs.File.Exists(changedSource))
                {
                    var changedDestination = Path.ChangeExtension(destinationPath, ".js");
                    indexedFiles.Add(changedSource, changedDestination);
                }
            }
        }
    }
}
