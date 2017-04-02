using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Abstractions;
using CodeDeployPack.Logging;
using Microsoft.Build.Framework;

namespace CodeDeployPack.PackageCompilation
{
    public class ExecutableAppPackager : AppPackagerBase
    {
        public ExecutableAppPackager(ILog log, IFileSystem fs) : base(log, fs)
        {
        }

        public override bool IsApplicable(ITaskItem[] contentFiles) => true;

        public override void Package(CreateCodeDeployTaskParameters parameters, ITaskItem[] contentFiles, List<ITaskItem> binaries, string projectDirectory, string outDir)
        {
            Log.LogMessage("Packaging a console or Window Service application (no Web.config detected)");
            Log.LogMessage("Add binary files", MessageImportance.Normal);
            IndexFilesToPackage(parameters, binaries, projectDirectory, relativeTo: outDir);
        }
    }
}
