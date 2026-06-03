using System;

namespace Backrooms.AssetsPipeline.Importing
{
    [Serializable]
    public class AssetImportPlan
    {
        public string assetId;
        public string sourceFilePath;
        public string targetFolder;
        public bool generatePrefab;
        public bool generateCollider;
        public bool optimizeMeshes;
        public bool compressTextures;
        public bool stripAnimations;
        public string[] importTags;
    }
}
