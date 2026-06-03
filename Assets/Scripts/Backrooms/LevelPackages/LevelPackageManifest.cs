using System;
using System.Collections.Generic;

namespace Backrooms.LevelPackages
{
    [Serializable]
    public class LevelPackageManifest
    {
        public string packageId;
        public string levelId;
        public string displayName;
        public string schemaVersion;
        public string packageVersion;
        public int seed;
        public string sceneName;
        public string sceneAddress;
        public string creditsId;
        public string validationReportId;
        public float estimatedSizeMb;
        public string checksum;
        public List<string> requiredAssetPackIds = new List<string>();
        public List<LevelPackageTag> tags = new List<LevelPackageTag>();

        public bool HasSceneReference()
        {
            return !string.IsNullOrWhiteSpace(sceneName) ||
                   !string.IsNullOrWhiteSpace(sceneAddress);
        }

        public bool HasValidationReference()
        {
            return !string.IsNullOrWhiteSpace(validationReportId);
        }

        public bool HasCreditsReference()
        {
            return !string.IsNullOrWhiteSpace(creditsId);
        }
    }

    [Serializable]
    public class LevelPackageTag
    {
        public string key;
        public string value;
    }
}
