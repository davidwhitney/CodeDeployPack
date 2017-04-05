using Microsoft.Build.Framework;

namespace CodeDeployPack
{
    public class CreateCodeDeployTaskParameters
    {
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

        public string AppConfigFile { get; set; }

        /// <summary>
        /// Used to output the list of built packages.
        /// </summary>
        [Output]
        public ITaskItem[] Packages { get; set; }

        public bool PublishPackagesToTeamCity { get; set; }
    }
}