using System;

namespace Backrooms.ApprovedAssets
{
    [Serializable]
    public class ApprovedAssetRecord
    {
        public string assetId;
        public string displayName;
        public string sourceUrl;
        public string creatorName;
        public string licenseName;
        public string licenseUrl;
        public string assetHash;
        public string approvedLocalPath;
        public string prefabPath;
        public string[] tags;
        public int estimatedTriangleCount;
        public long fileSizeBytes;
        public bool approvedForRuntime;
        public string approvalReportId;
    }
}
