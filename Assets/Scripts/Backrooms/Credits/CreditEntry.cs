using System;

namespace Backrooms.Credits
{
    [Serializable]
    public class CreditEntry
    {
        public string creditId;
        public string packageId;
        public string sourceTitle;
        public string creatorName;
        public string sourceUrl;
        public string licenseName;
        public string licenseUrl;
        public string usageType;
        public string assetHash;
        public string attributionText;
    }
}
