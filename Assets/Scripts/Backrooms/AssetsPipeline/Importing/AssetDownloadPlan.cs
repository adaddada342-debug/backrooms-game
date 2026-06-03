using System;

namespace Backrooms.AssetsPipeline.Importing
{
    [Serializable]
    public class AssetDownloadPlan
    {
        public string assetId;
        public string sourceUrl;
        public string quarantinePath;
        public string expectedHash;
        public long expectedFileSizeBytes;
        public bool requiresAuthentication;
        public string notes;
    }
}
