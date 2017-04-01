using Microsoft.Build.Framework;

namespace CodeDeployPack.Logging
{
    public class Logger : ILog
    {
        private readonly IBuildEngine _engine;
        public Logger(IBuildEngine engine) => _engine = engine;

        public void LogMessage(string message, MessageImportance importance = MessageImportance.High) => _engine
            .LogMessageEvent(new BuildMessageEventArgs("CodeDeployPack: " + message, "CodeDeployPack",
                "CodeDeployPack", importance));

        public void LogWarning(string code, string message) => _engine.LogWarningEvent(new BuildWarningEventArgs(
            "CodeDeployPack", code, null, 0, 0, 0, 0, message,
            "CodeDeployPack", "CodeDeployPack"));

        public void LogError(string code, string message) => _engine.LogErrorEvent(new BuildErrorEventArgs(
            "CodeDeployPack", code, null, 0, 0, 0, 0, message,
            "CodeDeployPack", "CodeDeployPack"));
    }
}