using System;

namespace Backrooms.AssetsPipeline.Metadata
{
    [Serializable]
    public class AssetThemeProfile
    {
        public string assetId;
        public string[] primaryTags;
        public string[] secondaryTags;
        public string[] forbiddenTags;
        public float liminalityScore;
        public float level0FitScore;
        public float poolroomsFitScore;
        public float industrialFitScore;
        public string notes;
    }
}
