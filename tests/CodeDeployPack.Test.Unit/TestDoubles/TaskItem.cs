using System;
using System.Collections;
using Microsoft.Build.Framework;

namespace CodeDeployPack.Test.Unit.TestDoubles
{
    public class TaskItem : ITaskItem
    {
        public string GetMetadata(string metadataName) => metadataName == "Link" ? Link : null;
        public void SetMetadata(string metadataName, string metadataValue) => throw new NotImplementedException();
        public void RemoveMetadata(string metadataName) => throw new NotImplementedException();
        public void CopyMetadataTo(ITaskItem destinationItem) => throw new NotImplementedException();
        public IDictionary CloneCustomMetadata() => throw new NotImplementedException();
        public string ItemSpec { get; set; }
        public ICollection MetadataNames { get; }
        public int MetadataCount { get; }
        public string Link { get; set; }
    }
}