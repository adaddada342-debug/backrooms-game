using Backrooms.AssetsPipeline.Contracts;
using Backrooms.AssetsPipeline.Metadata;
using Backrooms.Credits;

namespace Backrooms.AssetsPipeline.Mock
{
    public class SimpleAssetAttributionBuilder : IAssetAttributionBuilder
    {
        public CreditEntry BuildCredit(
            AssetSourceRecord sourceRecord,
            AssetLicenseInfo licenseInfo,
            string packageId)
        {
            string assetId = sourceRecord == null ? "unknown_asset" : sourceRecord.assetId;
            string creatorName = sourceRecord == null ? string.Empty : sourceRecord.creatorName;
            string sourceUrl = sourceRecord == null ? string.Empty : sourceRecord.sourceUrl;
            string assetHash = sourceRecord == null ? string.Empty : sourceRecord.assetHash;
            string licenseName = licenseInfo == null ? string.Empty : licenseInfo.licenseName;
            string licenseUrl = licenseInfo == null ? string.Empty : licenseInfo.licenseUrl;

            return new CreditEntry
            {
                creditId = "credit_" + assetId,
                packageId = packageId,
                sourceTitle = string.IsNullOrWhiteSpace(assetId) ? "Untitled asset" : assetId,
                creatorName = creatorName,
                sourceUrl = sourceUrl,
                licenseName = licenseName,
                licenseUrl = licenseUrl,
                usageType = "environment_asset",
                assetHash = assetHash,
                attributionText = BuildAttributionText(assetId, creatorName, licenseName, sourceUrl)
            };
        }

        private static string BuildAttributionText(
            string assetId,
            string creatorName,
            string licenseName,
            string sourceUrl)
        {
            return "'" + assetId + "' by " + creatorName + ", licensed under " + licenseName + ". Source: " + sourceUrl;
        }
    }
}
