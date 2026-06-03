using System;

namespace Backrooms.AssetsPipeline.Metadata
{
    [Serializable]
    public class AssetLicenseInfo
    {
        public string licenseName;
        public string licenseUrl;
        public bool allowsCommercialUse;
        public bool requiresAttribution;
        public bool requiresShareAlike;
        public bool allowsDerivatives;
        public bool isApprovedForUse;
        public string rejectionReason;
    }
}
