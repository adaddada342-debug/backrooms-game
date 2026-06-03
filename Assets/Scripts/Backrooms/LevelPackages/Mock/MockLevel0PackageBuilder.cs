using System;
using System.Collections.Generic;
using Backrooms.ApprovedAssets;
using Backrooms.Core;
using Backrooms.LevelPackages.Builder;
using Backrooms.LevelPackages.Dependencies;

namespace Backrooms.LevelPackages.Mock
{
    public class MockLevel0PackageBuilder : ILevelPackageBuilder
    {
        private static readonly string[] Level0Tags =
        {
            "level0",
            "office",
            "wall",
            "floor",
            "ceiling",
            "light",
            "carpet"
        };

        public LevelPackageBuildResult Build(
            LevelPackageBuildRequest request,
            ApprovedAssetLibrary approvedAssets)
        {
            LevelPackageBuildResult result = new LevelPackageBuildResult();

            if (request == null)
            {
                AddIssue(result, "request.missing", "Build request is required.", true);
                result.succeeded = false;
                return result;
            }

            if (approvedAssets == null)
            {
                AddIssue(result, "library.missing", "Approved asset library is required.", true);
            }

            if (string.IsNullOrWhiteSpace(request.requestedPackageId))
            {
                AddIssue(result, "package_id.missing", "Requested package id is required.", true);
            }

            if (string.IsNullOrWhiteSpace(request.levelId))
            {
                AddIssue(result, "level_id.missing", "Level id is required.", true);
            }

            if (string.IsNullOrWhiteSpace(request.targetSceneName) &&
                string.IsNullOrWhiteSpace(request.targetSceneAddress))
            {
                AddIssue(result, "scene_reference.missing", "A scene name or scene address is required.", true);
            }

            List<ApprovedAssetRecord> runtimeAssets = approvedAssets == null
                ? new List<ApprovedAssetRecord>()
                : approvedAssets.FindApprovedRuntimeAssets();

            if (runtimeAssets.Count == 0)
            {
                AddIssue(result, "assets.none_approved", "No approved runtime assets were found.", true);
            }

            List<ApprovedAssetRecord> selectedAssets = SelectLevel0Assets(runtimeAssets);
            AddSelectionWarnings(result, selectedAssets);

            string packageId = string.IsNullOrWhiteSpace(request.requestedPackageId)
                ? BackroomsConstants.DefaultPackageId
                : request.requestedPackageId;

            result.manifest = BuildManifest(request, packageId, selectedAssets);
            result.dependencyGraph = BuildDependencyGraph(packageId, selectedAssets);
            result.succeeded = !HasBlockers(result.issues) &&
                               result.dependencyGraph.IsCompleteAgainstLibrary(approvedAssets);

            return result;
        }

        private static LevelPackageManifest BuildManifest(
            LevelPackageBuildRequest request,
            string packageId,
            List<ApprovedAssetRecord> selectedAssets)
        {
            LevelPackageManifest manifest = new LevelPackageManifest
            {
                packageId = packageId,
                levelId = request.levelId,
                displayName = request.displayName,
                schemaVersion = BackroomsConstants.CurrentSchemaVersion,
                packageVersion = "0.1.0-local",
                seed = request.seed,
                sceneName = request.targetSceneName,
                sceneAddress = request.targetSceneAddress,
                creditsId = packageId + "_credits",
                validationReportId = packageId + "_validation",
                estimatedSizeMb = EstimateSizeMb(selectedAssets),
                checksum = packageId + "_local_mock_" + request.seed,
                requiredAssetPackIds = new List<string>(),
                tags = new List<LevelPackageTag>
                {
                    new LevelPackageTag { key = "level", value = request.levelId },
                    new LevelPackageTag { key = "localOnly", value = request.localOnly.ToString() },
                    new LevelPackageTag { key = "generator", value = "mock_level0_wave3" }
                }
            };

            foreach (ApprovedAssetRecord asset in selectedAssets)
            {
                if (asset != null && !string.IsNullOrWhiteSpace(asset.assetId))
                {
                    manifest.requiredAssetPackIds.Add(asset.assetId);
                }
            }

            return manifest;
        }

        private static LevelPackageDependencyGraph BuildDependencyGraph(
            string packageId,
            List<ApprovedAssetRecord> selectedAssets)
        {
            LevelPackageDependencyGraph graph = new LevelPackageDependencyGraph
            {
                graphId = packageId + "_dependency_graph",
                packageId = packageId,
                schemaVersion = BackroomsConstants.CurrentSchemaVersion,
                dependencies = new List<LevelPackageAssetDependency>()
            };

            foreach (ApprovedAssetRecord asset in selectedAssets)
            {
                if (asset == null)
                {
                    continue;
                }

                graph.dependencies.Add(new LevelPackageAssetDependency
                {
                    assetId = asset.assetId,
                    role = DetermineRole(asset),
                    required = true,
                    localPath = asset.approvedLocalPath,
                    prefabPath = asset.prefabPath,
                    tags = asset.tags
                });
            }

            return graph;
        }

        private static List<ApprovedAssetRecord> SelectLevel0Assets(List<ApprovedAssetRecord> runtimeAssets)
        {
            List<ApprovedAssetRecord> selected = new List<ApprovedAssetRecord>();

            foreach (ApprovedAssetRecord asset in runtimeAssets)
            {
                if (asset == null || !asset.approvedForRuntime)
                {
                    continue;
                }

                if (HasAnyTag(asset, Level0Tags) && !ContainsAsset(selected, asset.assetId))
                {
                    selected.Add(asset);
                }
            }

            return selected;
        }

        private static void AddSelectionWarnings(
            LevelPackageBuildResult result,
            List<ApprovedAssetRecord> selectedAssets)
        {
            foreach (string tag in Level0Tags)
            {
                if (!AnyAssetHasTag(selectedAssets, tag))
                {
                    AddIssue(result, "asset_tag.missing_" + tag, "No approved runtime asset matched important Level 0 tag '" + tag + "'.", false);
                }
            }

            if (selectedAssets.Count < 3)
            {
                AddIssue(result, "dependency_count.low", "Estimated dependency count is very low for a Level 0 package.", false);
            }
        }

        private static string DetermineRole(ApprovedAssetRecord asset)
        {
            if (HasTag(asset, "wall"))
            {
                return "wall";
            }

            if (HasTag(asset, "floor") || HasTag(asset, "carpet"))
            {
                return "floor";
            }

            if (HasTag(asset, "ceiling"))
            {
                return "ceiling";
            }

            if (HasTag(asset, "light"))
            {
                return "lighting";
            }

            return "environment";
        }

        private static float EstimateSizeMb(List<ApprovedAssetRecord> selectedAssets)
        {
            long bytes = 0;
            foreach (ApprovedAssetRecord asset in selectedAssets)
            {
                if (asset != null && asset.fileSizeBytes > 0)
                {
                    bytes += asset.fileSizeBytes;
                }
            }

            return bytes / (1024f * 1024f);
        }

        private static bool HasAnyTag(ApprovedAssetRecord asset, string[] tags)
        {
            foreach (string tag in tags)
            {
                if (HasTag(asset, tag))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool AnyAssetHasTag(List<ApprovedAssetRecord> assets, string tag)
        {
            foreach (ApprovedAssetRecord asset in assets)
            {
                if (HasTag(asset, tag))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool HasTag(ApprovedAssetRecord asset, string tag)
        {
            if (asset == null || asset.tags == null || string.IsNullOrWhiteSpace(tag))
            {
                return false;
            }

            foreach (string assetTag in asset.tags)
            {
                if (string.Equals(assetTag, tag, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool ContainsAsset(List<ApprovedAssetRecord> assets, string assetId)
        {
            foreach (ApprovedAssetRecord asset in assets)
            {
                if (asset != null && string.Equals(asset.assetId, assetId, StringComparison.Ordinal))
                {
                    return true;
                }
            }

            return false;
        }

        private static void AddIssue(
            LevelPackageBuildResult result,
            string code,
            string message,
            bool blocker)
        {
            result.issues.Add(new LevelPackageBuildIssue
            {
                code = code,
                message = message,
                blocker = blocker
            });
        }

        private static bool HasBlockers(List<LevelPackageBuildIssue> issues)
        {
            foreach (LevelPackageBuildIssue issue in issues)
            {
                if (issue != null && issue.blocker)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
