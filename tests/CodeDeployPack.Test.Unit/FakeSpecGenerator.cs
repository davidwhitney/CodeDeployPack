using System.Collections.Generic;
using CodeDeployPack.AppSpecCreation;

namespace CodeDeployPack.Test.Unit
{
    public class FakeSpecGenerator : IAppSpecGenerator
    {
        public string CreateAppSpec(Dictionary<string, string> packageContents)
        {
            return "";
        }
    }
}