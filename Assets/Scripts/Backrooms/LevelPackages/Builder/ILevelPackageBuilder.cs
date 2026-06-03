using Backrooms.ApprovedAssets;

namespace Backrooms.LevelPackages.Builder
{
    public interface ILevelPackageBuilder
    {
        LevelPackageBuildResult Build(
            LevelPackageBuildRequest request,
            ApprovedAssetLibrary approvedAssets);
    }
}
