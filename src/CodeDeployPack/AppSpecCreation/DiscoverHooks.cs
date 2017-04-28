using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CodeDeployPack.AppSpecCreation
{
    public class DiscoverHooks : IDiscoverHooks
    {
        private const string ScriptsDirectoryConvention = ".deploy";

        private readonly List<(string fileNameWithoutExt, Func<Hooks, List<Hook>> collectionProvider)> _mappers =
            new List<(string fileNameWithoutExt, Func<Hooks, List<Hook>> collectionProvider)>
            {
                ("before-install", x => x.BeforeInstall),
                ("after-install", x => x.AfterInstall),
                ("application-start", x => x.ApplicationStart),
                ("application-stop", x => x.ApplicationStop),
                ("validate-service", x => x.ValidateService)
            };

        private static void AddHook(List<Hook> list, string path)
        {
            if (list.Any())
                throw new InvalidOperationException($"Unable to add hook '{path}'. The hook of the same type '{list.First().location}' is already present.");
            list.Add(ToHook(path));
        }

        private static Hook ToHook(string path)
        {
            return new Hook { location = path };
        }

        public Hooks Discover(IEnumerable<string> destinationPaths)
        {
            if (destinationPaths == null)
                throw new ArgumentNullException(nameof(destinationPaths));
            var hooks = new Hooks();
            foreach (var destinationPath in destinationPaths)
                Aggregate(hooks, destinationPath);
            return hooks;
        }

        private void Aggregate(Hooks hooks, string destinationPath)
        {
            if (!IsScriptsDirectory(destinationPath))
                return;

            var targetCollection = _mappers
                .Where(mapper => DoesFileNameMatch(mapper.fileNameWithoutExt, destinationPath))
                .Select(mapper => mapper.collectionProvider(hooks))
                .FirstOrDefault();

            if (targetCollection != null)
                AddHook(targetCollection, destinationPath);
        }

        private bool DoesFileNameMatch(string expectedFileNameWithoutExtension, string destinationPath)
        {
            return string.Equals(
                Path.GetFileNameWithoutExtension(destinationPath),
                expectedFileNameWithoutExtension,
                StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsScriptsDirectory(string destinationPath)
        {
            var directoryName = Path.GetFileName(Path.GetDirectoryName(destinationPath));
            return string.Equals(
                directoryName,
                ScriptsDirectoryConvention,
                StringComparison.OrdinalIgnoreCase);
        }
    }
}