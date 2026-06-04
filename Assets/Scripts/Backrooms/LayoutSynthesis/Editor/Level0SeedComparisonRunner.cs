#if UNITY_EDITOR
using System.IO;
using Backrooms.LayoutSynthesis.Comparison;
using Backrooms.LayoutSynthesis.Level0;
using Backrooms.LayoutSynthesis.Models;
using Backrooms.LayoutSynthesis.Scoring;
using Backrooms.Validation;
using UnityEditor;
using UnityEngine;

namespace Backrooms.LayoutSynthesis.Editor
{
    public static class Level0SeedComparisonRunner
    {
        private const int SeedStart = 1001;
        private const int SeedEnd = 1020;
        private const string ReportPath = "Assets/Data/LayoutSynthesis/Comparisons/level0_seed_comparison_1001_1020.json";

        [MenuItem("Backrooms/Layout Synthesis/Compare Level 0 Seeds")]
        public static void CompareLevel0Seeds()
        {
            LayoutSeedComparisonReport report = new LayoutSeedComparisonReport
            {
                reportId = "level0_seed_comparison_1001_1020",
                levelId = "level_0",
                seedStart = SeedStart,
                seedCount = SeedEnd - SeedStart + 1
            };

            Level0LayoutSynthesizer synthesizer = new Level0LayoutSynthesizer();
            for (int seed = SeedStart; seed <= SeedEnd; seed++)
            {
                LayoutSynthesisRequest request = Level0LayoutSynthesisRequestFactory.CreateDefaultRequest();
                request.seed = seed;
                request.requestId = "level0_seed_" + seed;

                LayoutSynthesisResult synthesisResult = synthesizer.Synthesize(request);
                AssemblyValidationReport assemblyReport = synthesisResult == null || synthesisResult.plan == null
                    ? null
                    : AssemblyValidator.Validate(synthesisResult.plan);
                RouteReadabilityReport readabilityReport = synthesisResult == null || synthesisResult.plan == null
                    ? null
                    : RouteReadabilityScorer.Score(synthesisResult.plan);

                report.entries.Add(CreateEntry(seed, synthesisResult, assemblyReport, readabilityReport));
            }

            report.RecalculateSummary();
            Directory.CreateDirectory(Path.GetDirectoryName(ReportPath));
            File.WriteAllText(ReportPath, JsonUtility.ToJson(report, true));
            AssetDatabase.Refresh();

            Debug.Log(
                $"Level 0 seed comparison complete. success: {report.successCount}, failure: {report.failureCount}, average readability: {report.averageReadabilityScore:0.00}, best seed: {report.bestSeed}, worst seed: {report.worstSeed}");
        }

        private static LayoutSeedComparisonEntry CreateEntry(
            int seed,
            LayoutSynthesisResult synthesisResult,
            AssemblyValidationReport assemblyReport,
            RouteReadabilityReport readabilityReport)
        {
            LayoutSeedComparisonEntry entry = new LayoutSeedComparisonEntry
            {
                seed = seed,
                synthesisSucceeded = synthesisResult != null && synthesisResult.succeeded,
                assemblyValidationPassed = assemblyReport != null && assemblyReport.passed,
                readabilityPassed = readabilityReport != null && readabilityReport.passed,
                readabilityScore = readabilityReport == null ? 0f : readabilityReport.totalScore,
                issueCount = synthesisResult == null || synthesisResult.issues == null ? 0 : synthesisResult.issues.Count
            };

            if (synthesisResult != null && synthesisResult.plan != null)
            {
                entry.roomCount = synthesisResult.plan.rooms == null ? 0 : synthesisResult.plan.rooms.Count;
                entry.connectionCount = synthesisResult.plan.connections == null ? 0 : synthesisResult.plan.connections.Count;
                entry.openingCount = synthesisResult.plan.openings == null ? 0 : synthesisResult.plan.openings.Count;
                entry.landmarkCount = synthesisResult.plan.landmarks == null ? 0 : synthesisResult.plan.landmarks.Count;
            }

            entry.summary = $"seed {seed}: synthesis={entry.synthesisSucceeded}, assembly={entry.assemblyValidationPassed}, readability={entry.readabilityScore:0.00}";
            return entry;
        }
    }
}
#endif
