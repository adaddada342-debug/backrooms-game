using System;

namespace Backrooms.AssetsPipeline.Metadata
{
    [Serializable]
    public class AssetTechnicalProfile
    {
        public string assetId;
        public long fileSizeBytes;
        public int estimatedTriangleCount;
        public int estimatedVertexCount;
        public int materialCount;
        public int textureCount;
        public int maxTextureResolution;
        public bool hasAnimations;
        public bool hasEmbeddedTextures;
        public bool hasMissingTextures;
        public bool hasInvalidScale;
        public bool hasInvalidPivot;
        public bool hasBrokenNormals;
        public string[] detectedFileExtensions;
    }
}
