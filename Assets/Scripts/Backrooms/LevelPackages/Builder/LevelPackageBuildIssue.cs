using System;

namespace Backrooms.LevelPackages.Builder
{
    [Serializable]
    public class LevelPackageBuildIssue
    {
        public string code;
        public string message;
        public bool blocker;
    }
}
