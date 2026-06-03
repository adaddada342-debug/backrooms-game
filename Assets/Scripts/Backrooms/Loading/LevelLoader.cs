using System.Collections.Generic;
using Backrooms.LevelPackages;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Backrooms.Loading
{
    public class LevelLoader : MonoBehaviour
    {
        [SerializeField]
        private LevelPackageRegistry registry = null;

        public void Load(LevelLoadRequest request)
        {
            if (request == null)
            {
                Debug.LogError("Cannot load level: request is null.");
                return;
            }

            if (!string.IsNullOrWhiteSpace(request.targetPackageId))
            {
                LoadPackageById(request.targetPackageId);
                return;
            }

            if (registry == null)
            {
                Debug.LogError("Cannot load level: LevelPackageRegistry is not assigned.");
                return;
            }

            List<LevelPackageManifest> packages = registry.FindByLevelId(request.targetLevelId);
            if (packages.Count == 0)
            {
                Debug.LogError($"Cannot load level: no package found for level id '{request.targetLevelId}'.");
                return;
            }

            LoadManifest(packages[0]);
        }

        public void LoadPackageById(string packageId)
        {
            if (registry == null)
            {
                Debug.LogError("Cannot load package: LevelPackageRegistry is not assigned.");
                return;
            }

            if (!registry.TryGetByPackageId(packageId, out LevelPackageManifest manifest))
            {
                Debug.LogError($"Cannot load package: package id '{packageId}' was not found.");
                return;
            }

            LoadManifest(manifest);
        }

        private static void LoadManifest(LevelPackageManifest manifest)
        {
            if (manifest == null)
            {
                Debug.LogError("Cannot load package: manifest is null.");
                return;
            }

            if (string.IsNullOrWhiteSpace(manifest.sceneName))
            {
                Debug.LogError($"Cannot load package '{manifest.packageId}': sceneName is missing.");
                return;
            }

            // TODO: Add Addressables, remote catalog, and CDN support after local package validation exists.
            SceneManager.LoadScene(manifest.sceneName);
        }
    }
}
