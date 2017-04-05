using System.Collections.Generic;
using YamlDotNet.RepresentationModel;

namespace CodeDeployPack.AppSpecCreation
{
    public class AppSpec
    {
        public string version { get; set; } = "0.0.0.0";
        public Os os { get; set; } = Os.windows;

        public List<FileEntry> files { get; set; } = new List<FileEntry>();
        public Hooks hooks { get; set; }
    }

    public class Hooks
    {
        public List<Hook> BeforeInstall { get; set; } = new List<Hook>();
        public List<Hook> AfterInstall { get; set; } = new List<Hook>();
        public List<Hook> ApplicationStart { get; set; } = new List<Hook>();
        public List<Hook> ValidateService { get; set; } = new List<Hook>();
    }

    public class Hook
    {
        public string location { get; set; }
        public int timeout { get; set; }
        public string runas { get; set; }
    }

    public class FileEntry
    {
        public string source { get; set; }
        public string destination { get; set; }
    }

    public enum Os
    {
        windows,
        linux
    }
}
