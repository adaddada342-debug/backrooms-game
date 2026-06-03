using Backrooms.AssetsPipeline.Metadata;

namespace Backrooms.AssetsPipeline.Contracts
{
    public interface IAssetMetadataResolver
    {
        bool CanResolve(string sourceUrl);
        AssetSourceRecord ResolveSourceRecord(string sourceUrl);
        AssetLicenseInfo ResolveLicense(string sourceUrl);
    }
}
