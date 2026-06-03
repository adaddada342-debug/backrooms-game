using System;
using System.Collections.Generic;
using Backrooms.ApprovedAssets;

namespace Backrooms.LevelPackages.Dependencies
{
    [Serializable]
    public class LevelPackageDependencyGraph
    {
        public string graphId;
        public string packageId;
        public string schemaVersion;
        public List<LevelPackageAssetDependency> dependencies = new List<LevelPackageAssetDependency>();

        public bool HasDependency(string assetId)
        {
            if (string.IsNullOrWhiteSpace(assetId) || dependencies == null)
            {
                return false;
            }

            foreach (LevelPackageAssetDependency dependency in dependencies)
            {
                if (dependency == null)
                {
                    continue;
                }

                if (string.Equals(dependency.assetId, assetId, StringComparison.Ordinal))
                {
                    return true;
                }
            }

            return false;
        }

        public List<LevelPackageAssetDependency> GetRequiredDependencies()
        {
            List<LevelPackageAssetDependency> requiredDependencies = new List<LevelPackageAssetDependency>();

            if (dependencies == null)
            {
                return requiredDependencies;
            }

            foreach (LevelPackageAssetDependency dependency in dependencies)
            {
                if (dependency != null && dependency.required)
                {
                    requiredDependencies.Add(dependency);
                }
            }

            return requiredDependencies;
        }

        public bool IsCompleteAgainstLibrary(ApprovedAssetLibrary library)
        {
            List<LevelPackageAssetDependency> requiredDependencies = GetRequiredDependencies();
            if (requiredDependencies.Count == 0)
            {
                return true;
            }

            if (library == null)
            {
                return false;
            }

            foreach (LevelPackageAssetDependency dependency in requiredDependencies)
            {
                if (dependency == null || string.IsNullOrWhiteSpace(dependency.assetId))
                {
                    return false;
                }

                if (!library.TryGetByAssetId(dependency.assetId, out ApprovedAssetRecord record))
                {
                    return false;
                }

                if (record == null || !record.approvedForRuntime)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
