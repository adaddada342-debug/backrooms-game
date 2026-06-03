using System.Collections.Generic;
using Backrooms.AssetsPipeline.Contracts;
using Backrooms.AssetsPipeline.Metadata;
using Backrooms.AssetsPipeline.Validation;

namespace Backrooms.AssetsPipeline.Mock
{
    // Safe local validator only. It never downloads, imports, or inspects real asset files.
    public class MockAssetValidator : IAssetValidator
    {
        private const long MaxFileSizeBytes = 250L * 1024L * 1024L;
        private const int MaxTriangleCount = 200000;

        public AssetValidationReport Validate(AssetSourceRecord sourceRecord)
        {
            List<AssetValidationIssue> issues = new List<AssetValidationIssue>();
            string assetId = sourceRecord == null ? "missing_asset" : sourceRecord.assetId;

            if (sourceRecord == null)
            {
                AddBlocker(issues, "asset.missing", "Asset source record is missing.", false, true);
                return BuildReport(assetId, false, false, false, false, false, false, issues, null);
            }

            bool sourcePassed = !string.IsNullOrWhiteSpace(sourceRecord.sourceUrl);
            bool creatorPassed = !string.IsNullOrWhiteSpace(sourceRecord.creatorName);
            bool licensePassed = IsApprovedLicense(sourceRecord.licenseName);
            bool fileSizePassed = sourceRecord.fileSizeBytes > 0 && sourceRecord.fileSizeBytes < MaxFileSizeBytes;
            bool trianglesPassed = sourceRecord.estimatedTriangleCount > 0 &&
                                   sourceRecord.estimatedTriangleCount < MaxTriangleCount;
            bool hashPassed = !string.IsNullOrWhiteSpace(sourceRecord.assetHash);
            bool attributionPassed = creatorPassed && sourcePassed && licensePassed && hashPassed;
            bool technicalPassed = fileSizePassed && trianglesPassed && hashPassed;
            bool themePassed = sourceRecord.tags != null && sourceRecord.tags.Length > 0;
            bool performancePassed = fileSizePassed && trianglesPassed;

            if (!sourcePassed)
            {
                AddBlocker(issues, "source.url_missing", "Asset source URL is required.", true, true);
            }

            if (!creatorPassed)
            {
                AddBlocker(issues, "creator.missing", "Creator name is required for provenance and attribution.", true, true);
            }

            if (!licensePassed)
            {
                AddBlocker(issues, "license.rejected", "License must be CC0 or CC-BY for this mock pipeline.", true, false);
            }

            if (!fileSizePassed)
            {
                AddBlocker(issues, "technical.file_size", "Asset file size must be greater than 0 and less than 250 MB.", true, true);
            }

            if (!trianglesPassed)
            {
                AddBlocker(issues, "technical.triangle_count", "Estimated triangle count must be greater than 0 and less than 200,000.", true, true);
            }

            if (!hashPassed)
            {
                AddBlocker(issues, "source.hash_missing", "Asset hash is required before approval.", true, true);
            }

            if (!themePassed)
            {
                AddWarning(issues, "theme.tags_missing", "Asset has no theme tags. Later waves should require stronger theme analysis.");
            }

            if (sourceRecord.estimatedTriangleCount > 150000 && trianglesPassed)
            {
                AddWarning(issues, "performance.triangle_budget_high", "Asset is below the hard triangle limit but may need optimization.");
            }

            bool passed = licensePassed &&
                          attributionPassed &&
                          technicalPassed &&
                          themePassed &&
                          performancePassed &&
                          !HasBlockers(issues);

            return BuildReport(
                assetId,
                passed,
                licensePassed,
                attributionPassed,
                technicalPassed,
                themePassed,
                performancePassed,
                issues,
                sourceRecord);
        }

        private static AssetValidationReport BuildReport(
            string assetId,
            bool passed,
            bool licensePassed,
            bool attributionPassed,
            bool technicalPassed,
            bool themePassed,
            bool performancePassed,
            List<AssetValidationIssue> issues,
            AssetSourceRecord sourceRecord)
        {
            return new AssetValidationReport
            {
                reportId = "mock_report_" + (string.IsNullOrWhiteSpace(assetId) ? "unknown" : assetId),
                assetId = assetId,
                passed = passed,
                licensePassed = licensePassed,
                attributionPassed = attributionPassed,
                technicalPassed = technicalPassed,
                themePassed = themePassed,
                performancePassed = performancePassed,
                issues = issues.ToArray(),
                licenseInfo = BuildLicenseInfo(sourceRecord),
                technicalProfile = BuildTechnicalProfile(sourceRecord),
                themeProfile = BuildThemeProfile(sourceRecord)
            };
        }

        private static AssetLicenseInfo BuildLicenseInfo(AssetSourceRecord sourceRecord)
        {
            if (sourceRecord == null)
            {
                return null;
            }

            bool approved = IsApprovedLicense(sourceRecord.licenseName);
            return new AssetLicenseInfo
            {
                licenseName = sourceRecord.licenseName,
                licenseUrl = sourceRecord.licenseUrl,
                allowsCommercialUse = approved,
                requiresAttribution = sourceRecord.licenseName == "CC-BY",
                requiresShareAlike = false,
                allowsDerivatives = approved,
                isApprovedForUse = approved,
                rejectionReason = approved ? string.Empty : "License must be CC0 or CC-BY."
            };
        }

        private static AssetTechnicalProfile BuildTechnicalProfile(AssetSourceRecord sourceRecord)
        {
            if (sourceRecord == null)
            {
                return null;
            }

            return new AssetTechnicalProfile
            {
                assetId = sourceRecord.assetId,
                fileSizeBytes = sourceRecord.fileSizeBytes,
                estimatedTriangleCount = sourceRecord.estimatedTriangleCount,
                estimatedVertexCount = sourceRecord.estimatedTriangleCount / 2,
                materialCount = 1,
                textureCount = 1,
                maxTextureResolution = 2048,
                hasAnimations = false,
                hasEmbeddedTextures = false,
                hasMissingTextures = false,
                hasInvalidScale = false,
                hasInvalidPivot = false,
                hasBrokenNormals = false,
                detectedFileExtensions = new[] { ".fbx", ".png" }
            };
        }

        private static AssetThemeProfile BuildThemeProfile(AssetSourceRecord sourceRecord)
        {
            if (sourceRecord == null)
            {
                return null;
            }

            return new AssetThemeProfile
            {
                assetId = sourceRecord.assetId,
                primaryTags = sourceRecord.tags,
                secondaryTags = new[] { "mock_validated" },
                forbiddenTags = new string[0],
                liminalityScore = 0.75f,
                level0FitScore = 0.8f,
                poolroomsFitScore = 0.2f,
                industrialFitScore = 0.35f,
                notes = "Mock theme profile for local contract testing only."
            };
        }

        private static bool IsApprovedLicense(string licenseName)
        {
            return licenseName == "CC0" || licenseName == "CC-BY";
        }

        private static void AddBlocker(
            List<AssetValidationIssue> issues,
            string code,
            string message,
            bool userActionRequired,
            bool retryAllowed)
        {
            issues.Add(new AssetValidationIssue
            {
                code = code,
                message = message,
                severity = AssetValidationSeverity.Blocker,
                userActionRequired = userActionRequired,
                retryAllowed = retryAllowed
            });
        }

        private static void AddWarning(List<AssetValidationIssue> issues, string code, string message)
        {
            issues.Add(new AssetValidationIssue
            {
                code = code,
                message = message,
                severity = AssetValidationSeverity.Warning,
                userActionRequired = false,
                retryAllowed = true
            });
        }

        private static bool HasBlockers(List<AssetValidationIssue> issues)
        {
            foreach (AssetValidationIssue issue in issues)
            {
                if (issue != null && issue.severity == AssetValidationSeverity.Blocker)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
