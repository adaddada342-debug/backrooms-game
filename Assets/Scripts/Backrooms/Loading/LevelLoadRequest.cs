using System;

namespace Backrooms.Loading
{
    [Serializable]
    public class LevelLoadRequest
    {
        public string currentPackageId;
        public string targetLevelId;
        public string targetPackageId;
        public string transitionType;
        public int seed;
        public bool hasExplicitSeed;
    }
}
