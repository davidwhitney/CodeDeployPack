using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Reflection;
using CodeDeployPack.Logging;
using Microsoft.Build.Framework;

namespace CodeDeployPack
{
    public class CreateCodeDeployPackage : ITask
    {
        public IBuildEngine BuildEngine { get; set; }
        public ITaskHost HostObject { get; set; }

        private readonly FileSystem _fileSystem;
        private readonly ILog _log;

        public CreateCodeDeployPackage()
        {
            _fileSystem = new FileSystem();
            _log = new Logger(BuildEngine);
        }

        public bool Execute()
        {
            LogDiagnostics();
            _log.LogMessage("Creating CodeDeploy package...");

            WrittenFiles = WrittenFiles ?? new ITaskItem[0];
            _log.LogMessage("Written files: " + WrittenFiles.Length);

            var packing = CreateEmptyOutputDirectory("packing");
            var packed = CreateEmptyOutputDirectory("packed");

            var content = ContentFiles.Where(file => !string.Equals(Path.GetFileName(file.ItemSpec), "packages.config", StringComparison.OrdinalIgnoreCase));
            var binaries = new List<ITaskItem>(WrittenFiles);

            if (IsWebApplication())
            {
                _log.LogMessage("Packaging an ASP.NET web application (Web.config detected)");

                _log.LogMessage("Add content files", MessageImportance.Normal);
                AddFiles(content, ProjectDirectory);

                _log.LogMessage("Add binary files to the bin folder", MessageImportance.Normal);
                AddFiles(binaries, ProjectDirectory, relativeTo: OutDir, targetDirectory: "bin");
            }
            else
            {
                _log.LogMessage("Packaging a console or Window Service application (no Web.config detected)");

                _log.LogMessage("Add binary files", MessageImportance.Normal);
                AddFiles(binaries, ProjectDirectory, relativeTo: OutDir);
            }
            return true;
        }

        private void AddFiles(IEnumerable<ITaskItem> sourceFiles, string sourceBaseDirectory, string targetDirectory = "", string relativeTo = "")
        {
        }

        private bool IsWebApplication()
        {
            var hasWebConfigFile = _fileSystem.File.Exists("web.config");
            var hasWebConfigLinkedFile = ContentFiles != null && HasLinkedWebConfigFile();
            return hasWebConfigFile || hasWebConfigLinkedFile;
        }

        private bool HasLinkedWebConfigFile()
        {
            return ContentFiles.Any(f =>
            {
                var link = f.GetMetadata("Link");
                var hasLink = !string.IsNullOrEmpty(link) && link.Equals("web.config", StringComparison.OrdinalIgnoreCase);
                return hasLink;
            });
        }

        private void LogDiagnostics()
        {
            _log.LogMessage($"CodeDeployPack version: {Assembly.GetExecutingAssembly().GetName().Version}");
            _log.LogMessage("---Arguments---", MessageImportance.Low);
            _log.LogMessage("Content files: " + (ContentFiles ?? new ITaskItem[0]).Length, MessageImportance.High);
            _log.LogMessage("ProjectDirectory: " + ProjectDirectory, MessageImportance.High);
            _log.LogMessage("OutDir: " + OutDir, MessageImportance.High);
            _log.LogMessage("PackageVersion: " + PackageVersion, MessageImportance.High);
            _log.LogMessage("ProjectName: " + ProjectName, MessageImportance.High);
            _log.LogMessage("PrimaryOutputAssembly: " + PrimaryOutputAssembly, MessageImportance.High);
            _log.LogMessage("NugetArguments: " + NuGetArguments, MessageImportance.High);
            _log.LogMessage("NugetProperties: " + NuGetProperties, MessageImportance.High);
            _log.LogMessage("---------------", MessageImportance.High);
        }

        private string CreateEmptyOutputDirectory(string name)
        {
            var temp = Path.Combine(ProjectDirectory, "obj", name);
            _log.LogMessage("Create directory: " + temp, MessageImportance.Low);
            try
            {
                _fileSystem.Directory.Delete(temp);
            }
            catch
            {
            }
            _fileSystem.Directory.CreateDirectory(temp);
            return temp;
        }

        /// <summary>
        /// Allows the name of the NuSpec file to be overridden. If empty, defaults to <see cref="ProjectName"/>.nuspec.
        /// </summary>
        public string NuSpecFileName { get; set; }

        /// <summary>
        /// Appends the value to <see cref="ProjectName"/> when generating the Id of the Nuget Package
        /// </summary>
        public string AppendToPackageId { get; set; }

        /// <summary>
        /// Appends the value to the version.
        /// </summary>
        public string AppendToVersion { get; set; }

        /// <summary>
        /// The list of content files in the project. For web applications, these files will be included in the final package.
        /// </summary>
        [Required]
        public ITaskItem[] ContentFiles { get; set; }

        /// <summary>
        /// The list of written files in the project. This should mean all binaries produced from the build.
        /// </summary>
        [Required]
        public ITaskItem[] WrittenFiles { get; set; }

        /// <summary>
        /// The projects root directory; set to <code>$(MSBuildProjectDirectory)</code> by default.
        /// </summary>
        [Required]
        public string ProjectDirectory { get; set; }

        /// <summary>
        /// The directory in which the built files were written to.
        /// </summary>
        [Required]
        public string OutDir { get; set; }

        /// <summary>
        /// Whether TypeScript (.ts) files should be included.
        /// </summary>
        public bool IncludeTypeScriptSourceFiles { get; set; }

        /// <summary>
        /// The NuGet package version. If not set via an MSBuild property, it will be empty in which case we'll use the version in the NuSpec file or 1.0.0.
        /// </summary>
        public string PackageVersion { get; set; }

        /// <summary>
        /// The name of the project; by default will be set to $(MSBuildProjectName). 
        /// </summary>
        [Required]
        public string ProjectName { get; set; }

        /// <summary>
        /// The path to the primary DLL/executable being produced by the project.
        /// </summary>
        [Required]
        public string PrimaryOutputAssembly { get; set; }

        /// <summary>
        /// Allows release notes to be attached to the NuSpec file when building.
        /// </summary>
        public string ReleaseNotesFile { get; set; }

        public string AppConfigFile { get; set; }

        /// <summary>
        /// Used to output the list of built packages.
        /// </summary>
        [Output]
        public ITaskItem[] Packages { get; set; }

        /// <summary>
        /// The path to NuGet.exe.
        /// </summary>
        [Output]
        public string NuGetExePath { get; set; }


        public bool EnforceAddingFiles { get; set; }

        public bool PublishPackagesToTeamCity { get; set; }

        /// <summary>
        /// Extra arguments to pass along to nuget.
        /// </summary>
        public string NuGetArguments { get; set; }

        /// <summary>
        /// Properties to pass along to nuget
        /// </summary>
        [Output]
        public string NuGetProperties { get; set; }

        /// <summary>
        /// Whether to suppress the warning about having scripts at the root
        /// </summary>
        public bool IgnoreNonRootScripts { get; set; }
    }
}
