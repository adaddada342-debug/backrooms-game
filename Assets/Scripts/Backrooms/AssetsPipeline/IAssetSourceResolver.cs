namespace Backrooms.AssetsPipeline
{
    // Wave 1 simple resolver. Wave 2 contracts under AssetsPipeline.Contracts define the fuller ingestion shape.
    // Future editor/backend interface only. Runtime Unity code must not download or scrape source assets.
    public interface IAssetSourceResolver
    {
        bool CanResolve(string sourceUrl);
        AssetSourceRecord ResolveMetadata(string sourceUrl);
    }
}
