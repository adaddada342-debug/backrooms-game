namespace Backrooms.AssetsPipeline
{
    // Future editor/backend interface only. Runtime Unity code must not download or scrape source assets.
    public interface IAssetSourceResolver
    {
        bool CanResolve(string sourceUrl);
        AssetSourceRecord ResolveMetadata(string sourceUrl);
    }
}
