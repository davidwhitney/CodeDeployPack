using System.Collections.Generic;
using CodeDeployPack.Logging;
using Microsoft.Build.Framework;

namespace CodeDeployPack.Test.Unit
{
    public class FakeLogger : List<string>, ILog
    {
        public void LogMessage(string message, MessageImportance importance = MessageImportance.High) => Add(message);
        public void LogWarning(string code, string message) => Add($"{code}:{message}");
        public void LogError(string code, string message) => Add($"{code}:{message}");
    }
}