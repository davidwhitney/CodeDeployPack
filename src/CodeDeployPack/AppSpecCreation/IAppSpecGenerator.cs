using System.Collections.Generic;

namespace CodeDeployPack.AppSpecCreation
{
    public interface IAppSpecGenerator
    {
        string CreateAppSpec(Dictionary<string, string> packageContents, CreateCodeDeployTaskParameters parameters);
    }
}