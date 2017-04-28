using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CodeDeployPack.AppSpecCreation
{
    public class DiscoverHooks : IDiscoverHooks
    {
        private const string ScriptsDirectoryConvention = ".deploy";

        private readonly List<(string fileNameWithoutExt, Action<Hooks, string> aggregateFn)> _mappers =
            new List<(string fileNameWithoutExt, Action<Hooks, string> aggregateFn)>
            {
                ("before-install", AddBeforeInstallHook),
                ("after-install", AddAfterInstallHook),
                ("application-start", AddApplicationStartHook),
                ("application-stop", AddApplicationStopHook),
                ("validate-service", AddValidateServiceHook),
            };

        private static void AddHook(List<Hook> list, string path)
        {
            if (list.Any())
                throw new InvalidOperationException($"Unable to add hook '{path}'. The hook of the same type '{list.First().location}' is already present.");
            list.Add(ToHook(path));
        }

        private static void AddValidateServiceHook(Hooks hooks, string path)
        {
            AddHook(hooks.ValidateService, path);
        }

        private static void AddApplicationStopHook(Hooks hooks, string path)
        {
            AddHook(hooks.ApplicationStop, path);
        }

        private static void AddApplicationStartHook(Hooks hooks, string path)
        {
            AddHook(hooks.ApplicationStart, path);
        }

        private static void AddAfterInstallHook(Hooks hooks, string path)
        {
            AddHook(hooks.AfterInstall, path);
        }

        private static void AddBeforeInstallHook(Hooks hooks, string path)
        {
            AddHook(hooks.BeforeInstall, path);
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

            var aggregator = _mappers
                .Where(mapper => DoesFileNameMatch(mapper.fileNameWithoutExt, destinationPath))
                .Select(mapper => mapper.aggregateFn)
                .FirstOrDefault();

            aggregator?.Invoke(hooks, destinationPath);
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