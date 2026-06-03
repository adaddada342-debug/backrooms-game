using System;
using System.Collections.Generic;
using Backrooms.LevelPackages.Dependencies;

namespace Backrooms.LevelPackages.Builder
{
    [Serializable]
    public class LevelPackageBuildResult
    {
        public bool succeeded;
        public LevelPackageManifest manifest;
        public LevelPackageDependencyGraph dependencyGraph;
        public List<LevelPackageBuildIssue> issues = new List<LevelPackageBuildIssue>();
    }
}
