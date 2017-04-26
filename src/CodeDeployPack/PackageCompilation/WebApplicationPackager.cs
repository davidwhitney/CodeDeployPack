using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using CodeDeployPack.Logging;
using Microsoft.Build.Framework;

namespace CodeDeployPack.PackageCompilation
{
    public class WebApplicationPackager : AppPackagerBase
    {
        private readonly IFileSystem _fs;
        public WebApplicationPackager(ILog log, IFileSystem fs) : base(log, fs) => _fs = fs;

        public override bool IsApplicable(ITaskItem[] contentFiles) =>
            _fs.File.Exists("web.config") || contentFiles != null && HasLinkedWebConfigFile(contentFiles);

        public override void Package(CreateCodeDeployTaskParameters parameters, ITaskItem[] contentFiles, List<ITaskItem> binaries, string projectDirectory, string outDir)
        {
            Log.LogMessage("Packaging an ASP.NET web application (Web.config detected)");

            var content = contentFiles.Where(file => !string.Equals(Path.GetFileName(file.ItemSpec),
                "packages.config", StringComparison.OrdinalIgnoreCase));

            Log.LogMessage("Add content files", MessageImportance.Normal);
            IndexFilesToPackage(parameters, content, projectDirectory);

            Log.LogMessage("Add binary files to the bin folder", MessageImportance.Normal);
            IndexFilesToPackage(parameters, binaries, projectDirectory, relativeTo: outDir, targetDirectory: "bin");
        }

        private static bool HasLinkedWebConfigFile(IEnumerable<ITaskItem> contentFiles) => contentFiles.Any(f =>
        {
            var link = f.GetMetadata("Link");
            return !string.IsNullOrEmpty(link) && link.Equals("web.config", StringComparison.OrdinalIgnoreCase);
        });
    }
}
