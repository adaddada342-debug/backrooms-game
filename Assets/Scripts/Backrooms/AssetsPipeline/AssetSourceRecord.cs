using System;

namespace Backrooms.AssetsPipeline
{
    [Serializable]
    public class AssetSourceRecord
    {
        public string assetId;
        public string sourcePlatform;
        public string sourceUrl;
        public string creatorName;
        public string licenseName;
        public string licenseUrl;
        public string assetHash;
        public long fileSizeBytes;
        public int estimatedTriangleCount;
        public string[] tags;
    }
}
