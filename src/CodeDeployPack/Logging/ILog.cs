using Microsoft.Build.Framework;

namespace CodeDeployPack.Logging
{
    public interface ILog
    {
        void LogMessage(string message, MessageImportance importance = MessageImportance.High);
        void LogWarning(string code, string message);
        void LogError(string code, string message);
    }
}