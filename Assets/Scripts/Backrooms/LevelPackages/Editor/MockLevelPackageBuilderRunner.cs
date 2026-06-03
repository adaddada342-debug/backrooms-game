#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using Backrooms.ApprovedAssets;
using Backrooms.Core;
using Backrooms.LevelPackages.Builder;
using Backrooms.LevelPackages.Dependencies;
using Backrooms.LevelPackages.Mock;
using UnityEditor;
using UnityEngine;

namespace Backrooms.LevelPackages.Editor
{
    public static class MockLevelPackageBuilderRunner
    {
        private const string PackageId = BackroomsConstants.DefaultPackageId;
        private const string ManifestPath = "Assets/Data/LevelPackages/Generated/level0_local_prototype_manifest.json";
        private const string DependencyGraphPath = "Assets/Data/LevelPackages/DependencyGraphs/level0_local_prototype_dependencies.json";
        private const string BuildReportPath = "Assets/Data/LevelPackages/BuilderReports/level0_local_prototype_build_report.json";

        [MenuItem("Backrooms/Level Packages/Generate Mock Level 0 Package")]
        public static void Generate()
        {
            ApprovedAssetLibrary library = ScriptableObject.CreateInstance<ApprovedAssetLibrary>();
            library.assets = CreateFakeApprovedAssets();

            LevelPackageBuildRequest request = new LevelPackageBuildRequest
            {
                requestedPackageId = PackageId,
                levelId = BackroomsConstants.DefaultLevelId,
                displayName = "Level 0 Local Prototype",
                seed = 1001,
                targetSceneName = "Level0_Local_Blockout",
                targetSceneAddress = string.Empty,
                requiredTags = new[] { "level0", "wall", "floor", "ceiling", "light", "carpet" },
                optionalTags = new[] { "office", "trim" },
                localOnly = true
            };

            MockLevel0PackageBuilder builder = new MockLevel0PackageBuilder();
            LevelPackageBuildResult result = builder.Build(request, library);

            WriteJson(ManifestPath, result.manifest);
            WriteJson(DependencyGraphPath, result.dependencyGraph);
            WriteJson(BuildReportPath, new BuildReportJson
            {
                generatedAtUtc = DateTime.UtcNow.ToString("o"),
                succeeded = result.succeeded,
                packageId = result.manifest == null ? PackageId : result.manifest.packageId,
                dependencyCount = result.dependencyGraph == null || result.dependencyGraph.dependencies == null
                    ? 0
                    : result.dependencyGraph.dependencies.Count,
                issues = result.issues
            });

            UnityEngine.Object.DestroyImmediate(library);
            AssetDatabase.Refresh();

            Debug.Log(
                $"Mock Level 0 package generation complete. Succeeded: {result.succeeded}, dependencies: {CountDependencies(result.dependencyGraph)}. Output package: {ManifestPath}");
        }

        private static List<ApprovedAssetRecord> CreateFakeApprovedAssets()
        {
            return new List<ApprovedAssetRecord>
            {
                CreateAsset(
                    "level0_carpet_tile",
                    "Level 0 Carpet Tile",
                    "carpet",
                    new[] { "level0", "floor", "carpet", "office" }),
                CreateAsset(
                    "level0_wallpaper_panel",
                    "Level 0 Wallpaper Panel",
                    "wall",
                    new[] { "level0", "wall", "office" }),
                CreateAsset(
                    "fluorescent_ceiling_light",
                    "Fluorescent Ceiling Light",
                    "light",
                    new[] { "level0", "ceiling", "light", "office" }),
                CreateAsset(
                    "office_ceiling_tile",
                    "Office Ceiling Tile",
                    "ceiling",
                    new[] { "level0", "ceiling", "office" }),
                CreateAsset(
                    "generic_floor_trim",
                    "Generic Floor Trim",
                    "trim",
                    new[] { "level0", "floor", "trim", "office" })
            };
        }

        private static ApprovedAssetRecord CreateAsset(
            string assetId,
            string displayName,
            string role,
            string[] tags)
        {
            return new ApprovedAssetRecord
            {
                assetId = assetId,
                displayName = displayName,
                sourceUrl = "mock://approved/" + assetId,
                creatorName = "Backrooms Mock Library",
                licenseName = "CC0",
                licenseUrl = "https://creativecommons.org/publicdomain/zero/1.0/",
                assetHash = "mock_hash_" + assetId,
                approvedLocalPath = "Assets/Data/ApprovedAssets/" + assetId + ".asset",
                prefabPath = "Assets/Data/ApprovedAssets/" + assetId + ".prefab",
                tags = tags,
                estimatedTriangleCount = role == "light" ? 1200 : 600,
                fileSizeBytes = 512L * 1024L,
                approvedForRuntime = true,
                approvalReportId = assetId + "_approval_report"
            };
        }

        private static void WriteJson(string path, object value)
        {
            string directory = Path.GetDirectoryName(path);
            if (!string.IsNullOrWhiteSpace(directory))
            {
                Directory.CreateDirectory(directory);
            }

            File.WriteAllText(path, JsonUtility.ToJson(value, true));
        }

        private static int CountDependencies(LevelPackageDependencyGraph graph)
        {
            return graph == null || graph.dependencies == null ? 0 : graph.dependencies.Count;
        }

        [Serializable]
        private class BuildReportJson
        {
            public string generatedAtUtc;
            public bool succeeded;
            public string packageId;
            public int dependencyCount;
            public List<LevelPackageBuildIssue> issues;
        }
    }
}
#endif
