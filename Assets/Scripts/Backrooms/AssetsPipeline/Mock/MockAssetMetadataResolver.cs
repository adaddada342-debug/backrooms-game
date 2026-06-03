using System;
using Backrooms.AssetsPipeline.Contracts;
using Backrooms.AssetsPipeline.Metadata;

namespace Backrooms.AssetsPipeline.Mock
{
    // Safe test double only. This resolver never contacts the internet or reads remote asset data.
    public class MockAssetMetadataResolver : IAssetMetadataResolver
    {
        public bool CanResolve(string sourceUrl)
        {
            return !string.IsNullOrWhiteSpace(sourceUrl) &&
                   (sourceUrl.StartsWith("mock://", StringComparison.OrdinalIgnoreCase) ||
                    sourceUrl.StartsWith("local://", StringComparison.OrdinalIgnoreCase));
        }

        public AssetSourceRecord ResolveSourceRecord(string sourceUrl)
        {
            if (!CanResolve(sourceUrl))
            {
                return new AssetSourceRecord
                {
                    assetId = "unresolved_asset",
                    sourceUrl = sourceUrl,
                    sourcePlatform = "unsupported",
                    licenseName = "unknown",
                    assetHash = string.Empty,
                    tags = new[] { "unresolved" }
                };
            }

            AssetLicenseInfo license = ResolveLicense(sourceUrl);
            string assetId = BuildAssetId(sourceUrl);
            bool dense = ContainsToken(sourceUrl, "dense");
            bool giant = ContainsToken(sourceUrl, "giant");

            return new AssetSourceRecord
            {
                assetId = assetId,
                sourcePlatform = sourceUrl.StartsWith("local://", StringComparison.OrdinalIgnoreCase) ? "local_mock" : "mock",
                sourceUrl = sourceUrl,
                creatorName = ContainsToken(sourceUrl, "anonymous") ? string.Empty : "Mock Creator",
                licenseName = license.licenseName,
                licenseUrl = license.licenseUrl,
                assetHash = BuildStableHash(sourceUrl),
                fileSizeBytes = giant ? 320L * 1024L * 1024L : 24L * 1024L * 1024L,
                estimatedTriangleCount = dense ? 260000 : 42000,
                tags = new[] { "liminal", "level_0", "environment" }
            };
        }

        public AssetLicenseInfo ResolveLicense(string sourceUrl)
        {
            string normalized = sourceUrl == null ? string.Empty : sourceUrl.ToLowerInvariant();

            if (normalized.Contains("cc0"))
            {
                return ApprovedLicense("CC0", "https://creativecommons.org/publicdomain/zero/1.0/", false);
            }

            if (normalized.Contains("cc-by"))
            {
                return ApprovedLicense("CC-BY", "https://creativecommons.org/licenses/by/4.0/", true);
            }

            if (normalized.Contains("editorial"))
            {
                return RejectedLicense("editorial", "Editorial-only assets are not approved for game use.");
            }

            if (normalized.Contains("non-commercial") || normalized.Contains("nc"))
            {
                return RejectedLicense("non-commercial", "Non-commercial licenses are not approved for this project.");
            }

            if (normalized.Contains("no-derivatives") || normalized.Contains("nd"))
            {
                return RejectedLicense("no-derivatives", "No-derivatives licenses are not approved for editable game assets.");
            }

            return RejectedLicense("unknown", "Unknown licenses must be reviewed before use.");
        }

        private static AssetLicenseInfo ApprovedLicense(
            string licenseName,
            string licenseUrl,
            bool requiresAttribution)
        {
            return new AssetLicenseInfo
            {
                licenseName = licenseName,
                licenseUrl = licenseUrl,
                allowsCommercialUse = true,
                requiresAttribution = requiresAttribution,
                requiresShareAlike = false,
                allowsDerivatives = true,
                isApprovedForUse = true,
                rejectionReason = string.Empty
            };
        }

        private static AssetLicenseInfo RejectedLicense(string licenseName, string reason)
        {
            return new AssetLicenseInfo
            {
                licenseName = licenseName,
                licenseUrl = string.Empty,
                allowsCommercialUse = false,
                requiresAttribution = true,
                requiresShareAlike = false,
                allowsDerivatives = false,
                isApprovedForUse = false,
                rejectionReason = reason
            };
        }

        private static bool ContainsToken(string value, string token)
        {
            return value != null &&
                   value.IndexOf(token, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private static string BuildAssetId(string sourceUrl)
        {
            string trimmed = sourceUrl
                .Replace("mock://", string.Empty, StringComparison.OrdinalIgnoreCase)
                .Replace("local://", string.Empty, StringComparison.OrdinalIgnoreCase);

            char[] chars = trimmed.ToCharArray();
            for (int i = 0; i < chars.Length; i++)
            {
                if (!char.IsLetterOrDigit(chars[i]) && chars[i] != '_')
                {
                    chars[i] = '_';
                }
            }

            return string.IsNullOrWhiteSpace(trimmed) ? "mock_asset" : new string(chars).Trim('_');
        }

        private static string BuildStableHash(string value)
        {
            unchecked
            {
                uint hash = 2166136261;
                for (int i = 0; i < value.Length; i++)
                {
                    hash ^= value[i];
                    hash *= 16777619;
                }

                return hash.ToString("x8");
            }
        }
    }
}
