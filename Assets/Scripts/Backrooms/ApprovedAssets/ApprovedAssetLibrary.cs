using System.Collections.Generic;
using UnityEngine;

namespace Backrooms.ApprovedAssets
{
    [CreateAssetMenu(
        fileName = "ApprovedAssetLibrary",
        menuName = "Backrooms/Approved Assets/Approved Asset Library")]
    public class ApprovedAssetLibrary : ScriptableObject
    {
        public List<ApprovedAssetRecord> assets = new List<ApprovedAssetRecord>();

        public IReadOnlyList<ApprovedAssetRecord> GetAll()
        {
            if (assets == null)
            {
                assets = new List<ApprovedAssetRecord>();
            }

            return assets;
        }

        public bool TryGetByAssetId(string assetId, out ApprovedAssetRecord record)
        {
            record = null;

            if (string.IsNullOrWhiteSpace(assetId) || assets == null)
            {
                return false;
            }

            foreach (ApprovedAssetRecord asset in assets)
            {
                if (asset == null)
                {
                    continue;
                }

                if (string.Equals(asset.assetId, assetId, System.StringComparison.Ordinal))
                {
                    record = asset;
                    return true;
                }
            }

            return false;
        }

        public List<ApprovedAssetRecord> FindByTag(string tag)
        {
            List<ApprovedAssetRecord> matches = new List<ApprovedAssetRecord>();

            if (string.IsNullOrWhiteSpace(tag) || assets == null)
            {
                return matches;
            }

            foreach (ApprovedAssetRecord asset in assets)
            {
                if (asset == null || asset.tags == null)
                {
                    continue;
                }

                foreach (string assetTag in asset.tags)
                {
                    if (string.Equals(assetTag, tag, System.StringComparison.OrdinalIgnoreCase))
                    {
                        matches.Add(asset);
                        break;
                    }
                }
            }

            return matches;
        }

        public List<ApprovedAssetRecord> FindApprovedRuntimeAssets()
        {
            List<ApprovedAssetRecord> matches = new List<ApprovedAssetRecord>();

            if (assets == null)
            {
                return matches;
            }

            foreach (ApprovedAssetRecord asset in assets)
            {
                if (asset != null && asset.approvedForRuntime)
                {
                    matches.Add(asset);
                }
            }

            return matches;
        }
    }
}
