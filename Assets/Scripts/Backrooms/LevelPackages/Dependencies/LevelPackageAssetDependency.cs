using System;

namespace Backrooms.LevelPackages.Dependencies
{
    [Serializable]
    public class LevelPackageAssetDependency
    {
        public string assetId;
        public string role;
        public bool required;
        public string localPath;
        public string prefabPath;
        public string[] tags;
    }
}
