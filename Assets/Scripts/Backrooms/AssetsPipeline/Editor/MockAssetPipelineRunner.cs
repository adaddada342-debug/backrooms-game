#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using Backrooms.AssetsPipeline.Metadata;
using Backrooms.AssetsPipeline.Mock;
using Backrooms.AssetsPipeline.Validation;
using Backrooms.Credits;
using UnityEditor;
using UnityEngine;

namespace Backrooms.AssetsPipeline.Editor
{
    public static class MockAssetPipelineRunner
    {
        private const string ReportPath = "Assets/Data/AssetPipeline/Reports/mock_asset_pipeline_report.json";
        private const string MockPackageId = "level0_local_prototype";

        [MenuItem("Backrooms/Asset Pipeline/Run Mock Asset Pipeline")]
        public static void Run()
        {
            MockAssetMetadataResolver resolver = new MockAssetMetadataResolver();
            MockAssetValidator validator = new MockAssetValidator();
            SimpleAssetAttributionBuilder attributionBuilder = new SimpleAssetAttributionBuilder();

            List<AssetSourceRecord> sourceRecords = CreateMockSources(resolver);
            List<AssetValidationReport> reports = new List<AssetValidationReport>();
            List<CreditEntry> credits = new List<CreditEntry>();

            foreach (AssetSourceRecord sourceRecord in sourceRecords)
            {
                AssetValidationReport report = validator.Validate(sourceRecord);
                reports.Add(report);

                if (report.passed)
                {
                    AssetLicenseInfo licenseInfo = resolver.ResolveLicense(sourceRecord.sourceUrl);
                    credits.Add(attributionBuilder.BuildCredit(sourceRecord, licenseInfo, MockPackageId));
                }
            }

            MockAssetPipelineReport output = new MockAssetPipelineReport
            {
                generatedAtUtc = DateTime.UtcNow.ToString("o"),
                totalSources = sourceRecords.Count,
                approvedCount = credits.Count,
                rejectedCount = sourceRecords.Count - credits.Count,
                sources = sourceRecords.ToArray(),
                validationReports = reports.ToArray(),
                credits = credits.ToArray()
            };

            Directory.CreateDirectory(Path.GetDirectoryName(ReportPath));
            File.WriteAllText(ReportPath, JsonUtility.ToJson(output, true));
            AssetDatabase.Refresh();

            Debug.Log(
                $"Mock asset pipeline complete. Sources: {output.totalSources}, approved: {output.approvedCount}, rejected: {output.rejectedCount}. Report: {ReportPath}");
        }

        private static List<AssetSourceRecord> CreateMockSources(MockAssetMetadataResolver resolver)
        {
            return new List<AssetSourceRecord>
            {
                resolver.ResolveSourceRecord("mock://level0_wall_panel_cc0"),
                resolver.ResolveSourceRecord("mock://fluorescent_light_cc-by"),
                resolver.ResolveSourceRecord("mock://editorial_mascot_editorial"),
                resolver.ResolveSourceRecord("local://giant_machine_cc0_giant"),
                resolver.ResolveSourceRecord("mock://anonymous_dense_pipe_cc-by_dense")
            };
        }

        [Serializable]
        private class MockAssetPipelineReport
        {
            public string generatedAtUtc;
            public int totalSources;
            public int approvedCount;
            public int rejectedCount;
            public AssetSourceRecord[] sources;
            public AssetValidationReport[] validationReports;
            public CreditEntry[] credits;
        }
    }
}
#endif
