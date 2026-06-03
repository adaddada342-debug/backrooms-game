using Backrooms.AssetsPipeline.Validation;

namespace Backrooms.AssetsPipeline.Contracts
{
    public interface IAssetValidator
    {
        AssetValidationReport Validate(AssetSourceRecord sourceRecord);
    }
}
