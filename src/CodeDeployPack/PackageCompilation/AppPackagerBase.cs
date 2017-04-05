using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using CodeDeployPack.Logging;
using CodeDeployPack.PackageCompilation.SpecialFileTypes;
using Microsoft.Build.Framework;

namespace CodeDeployPack.PackageCompilation
{
    public abstract class AppPackagerBase
    {
        protected readonly ILog Log;
        private readonly IFileSystem _fs;
        public Dictionary<string, string> IndexedFiles { get; set; } = new Dictionary<string, string>();

        protected AppPackagerBase(ILog log, IFileSystem fs)
        {
            Log = log;
            _fs = fs;
        }

        public abstract bool IsApplicable(ITaskItem[] contentFiles);
        public abstract void Package(CreateCodeDeployTaskParameters parameters, ITaskItem[] contentFiles, List<ITaskItem> binaries, string projectDirectory, string outDir);

        public void IndexFilesToPackage(CreateCodeDeployTaskParameters parameters, IEnumerable<ITaskItem> sourceFiles,
            string sourceBaseDirectory, string targetDirectory = "", string relativeTo = "")
        {
            if (!string.IsNullOrWhiteSpace(relativeTo) && Path.IsPathRooted(relativeTo))
            {
                relativeTo = GetPathRelativeTo(relativeTo, sourceBaseDirectory);
            }

            foreach (var sourceFile in sourceFiles)
            {
                var sourceFilePath = Path.GetFullPath(Path.Combine(sourceBaseDirectory, sourceFile.ItemSpec));
                var destinationPath =
                    EstablishDestinationPath(sourceBaseDirectory, targetDirectory, relativeTo, sourceFile);

                if (!_fs.File.Exists(sourceFilePath))
                {
                    Log.LogWarning("ENOENT",
                        $"The source file \'{sourceFilePath}\' does not exist, so it will not be included in the package");
                    continue;
                }

                if (IndexedFiles.Keys.Any(k => k.ToLower() == sourceFilePath.ToLower()))
                {
                    continue;
                }

                var mappers = new List<IMapFiles>
                {
                    new AppConfigMapper(),
                    new TypeScriptMapper(parameters, _fs, Log)
                };

                var customFileMapper = mappers.SingleOrDefault(x => x.IsApplicable(sourceFilePath, destinationPath));
                if (customFileMapper != null)
                {
                    customFileMapper.Process(IndexedFiles, sourceFile, sourceFilePath, destinationPath);
                }
                else
                {
                    IndexedFiles.Add(sourceFilePath, destinationPath);
                }
            }
        }

        private string EstablishDestinationPath(string sourceBaseDirectory, string targetDirectory, string relativeTo,
            ITaskItem sourceFile)
        {
            var destinationPath = sourceFile.ItemSpec;
            var link = sourceFile.GetMetadata("Link");
            if (!string.IsNullOrWhiteSpace(link))
            {
                destinationPath = link;
            }

            if (!Path.IsPathRooted(destinationPath))
            {
                destinationPath = GetFullPath(Path.Combine(sourceBaseDirectory, destinationPath));
            }

            if (Path.IsPathRooted(destinationPath))
            {
                destinationPath = GetPathRelativeTo(destinationPath, sourceBaseDirectory);
            }

            if (!string.IsNullOrWhiteSpace(relativeTo))
            {
                if (destinationPath.StartsWith(relativeTo, StringComparison.OrdinalIgnoreCase))
                {
                    destinationPath = destinationPath.Substring(relativeTo.Length);
                }
            }

            destinationPath = Path.Combine(targetDirectory, destinationPath);
            return destinationPath;
        }

        public string GetPathRelativeTo(string fullPath, string relativeTo)
        {
            try
            {
                // http://stackoverflow.com/questions/703281/getting-path-relative-to-the-current-working-directory
                var file = new Uri(fullPath);
                var folder = new Uri(relativeTo + (relativeTo.EndsWith("\\") ? "" : "\\"));
                var relativePath =
                    Uri.UnescapeDataString(
                        folder.MakeRelativeUri(file)
                            .ToString()
                            .Replace('/', Path.DirectorySeparatorChar)
                    );
                return RemovePathTraversal(relativePath);
            }
            catch (Exception e)
            {
                // Provide some context for the error. This is not very helpful for example: UriFormatException: Invalid URI: The format of the URI could not be determined.
                throw new Exception($"Failed to build the path for '{fullPath}' relative to '{relativeTo}': {e.Message}. See the inner exception for more details.", e);
            }
        }

        public string RemovePathTraversal(string path)
        {
            var pathTraversalChars = ".." + Path.DirectorySeparatorChar;
            if (path.StartsWith(pathTraversalChars))
            {
                path = path.Replace(pathTraversalChars, string.Empty);
                return RemovePathTraversal(path);
            }
            return path;
        }

        public string GetFullPath(string relativeOrAbsoluteFilePath)
        {
            if (!Path.IsPathRooted(relativeOrAbsoluteFilePath))
            {
                relativeOrAbsoluteFilePath = Path.Combine(Environment.CurrentDirectory, relativeOrAbsoluteFilePath);
            }

            relativeOrAbsoluteFilePath = Path.GetFullPath(relativeOrAbsoluteFilePath);
            return relativeOrAbsoluteFilePath;
        }
    }
}