using Backrooms.AssetsPipeline.Importing;

namespace Backrooms.AssetsPipeline.Contracts
{
    public interface IAssetDownloadPlanner
    {
        AssetDownloadPlan CreatePlan(AssetSourceRecord sourceRecord);
    }
}
