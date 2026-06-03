using System;

namespace Backrooms.LevelPackages.Builder
{
    [Serializable]
    public class LevelPackageBuildRequest
    {
        public string requestedPackageId;
        public string levelId;
        public string displayName;
        public int seed;
        public string targetSceneName;
        public string targetSceneAddress;
        public string[] requiredTags;
        public string[] optionalTags;
        public bool localOnly;
    }
}
