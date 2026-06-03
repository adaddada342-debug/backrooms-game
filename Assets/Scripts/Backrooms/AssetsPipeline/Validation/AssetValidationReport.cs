using System;
using Backrooms.AssetsPipeline.Metadata;

namespace Backrooms.AssetsPipeline.Validation
{
    [Serializable]
    public class AssetValidationReport
    {
        public string reportId;
        public string assetId;
        public bool passed;
        public bool licensePassed;
        public bool attributionPassed;
        public bool technicalPassed;
        public bool themePassed;
        public bool performancePassed;
        public AssetValidationIssue[] issues;
        public AssetLicenseInfo licenseInfo;
        public AssetTechnicalProfile technicalProfile;
        public AssetThemeProfile themeProfile;
    }
}
