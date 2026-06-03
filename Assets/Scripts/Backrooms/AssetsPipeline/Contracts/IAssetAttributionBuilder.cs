using Backrooms.AssetsPipeline.Metadata;
using Backrooms.Credits;

namespace Backrooms.AssetsPipeline.Contracts
{
    public interface IAssetAttributionBuilder
    {
        CreditEntry BuildCredit(
            AssetSourceRecord sourceRecord,
            AssetLicenseInfo licenseInfo,
            string packageId);
    }
}
