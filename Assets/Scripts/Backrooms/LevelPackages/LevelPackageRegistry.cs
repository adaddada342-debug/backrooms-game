using System.Collections.Generic;
using UnityEngine;

namespace Backrooms.LevelPackages
{
    [CreateAssetMenu(
        fileName = "LevelPackageRegistry",
        menuName = "Backrooms/Level Packages/Level Package Registry")]
    public class LevelPackageRegistry : ScriptableObject
    {
        public List<LevelPackageManifest> packages = new List<LevelPackageManifest>();

        public IReadOnlyList<LevelPackageManifest> GetAll()
        {
            if (packages == null)
            {
                packages = new List<LevelPackageManifest>();
            }

            return packages;
        }

        public bool TryGetByPackageId(string packageId, out LevelPackageManifest manifest)
        {
            manifest = null;

            if (string.IsNullOrWhiteSpace(packageId) || packages == null)
            {
                return false;
            }

            foreach (LevelPackageManifest package in packages)
            {
                if (package == null)
                {
                    continue;
                }

                if (string.Equals(package.packageId, packageId, System.StringComparison.Ordinal))
                {
                    manifest = package;
                    return true;
                }
            }

            return false;
        }

        public List<LevelPackageManifest> FindByLevelId(string levelId)
        {
            List<LevelPackageManifest> matches = new List<LevelPackageManifest>();

            if (string.IsNullOrWhiteSpace(levelId) || packages == null)
            {
                return matches;
            }

            foreach (LevelPackageManifest package in packages)
            {
                if (package == null)
                {
                    continue;
                }

                if (string.Equals(package.levelId, levelId, System.StringComparison.Ordinal))
                {
                    matches.Add(package);
                }
            }

            return matches;
        }
    }
}
