#if UNITY_EDITOR
using System;
using System.IO;
using Backrooms.LayoutSynthesis.Level0;
using Backrooms.LayoutSynthesis.Models;
using UnityEditor;
using UnityEngine;

namespace Backrooms.LayoutSynthesis.Editor
{
    public static class Level0LayoutSynthesisRunner
    {
        private const string ReportPath = "Assets/Data/LayoutSynthesis/Reports/level0_synthesis_report.json";
        private const string PlanSummaryPath = "Assets/Data/LayoutSynthesis/Reports/level0_synthesized_plan_summary.json";

        [MenuItem("Backrooms/Layout Synthesis/Run Level 0 Synthesis Only")]
        public static void RunLevel0SynthesisOnly()
        {
            LayoutSynthesisRequest request = Level0LayoutSynthesisRequestFactory.CreateDefaultRequest();
            Level0LayoutSynthesizer synthesizer = new Level0LayoutSynthesizer();
            LayoutSynthesisResult result = synthesizer.Synthesize(request);

            WriteJson(ReportPath, LayoutSynthesisReport.FromResult(result, false));
            if (result != null && result.plan != null)
            {
                WriteJson(PlanSummaryPath, LayoutPlanSummary.FromResult(result));
            }

            AssetDatabase.Refresh();

            int roomCount = result == null || result.plan == null || result.plan.rooms == null ? 0 : result.plan.rooms.Count;
            int connectionCount = result == null || result.plan == null || result.plan.connections == null ? 0 : result.plan.connections.Count;
            int openingCount = result == null || result.plan == null || result.plan.openings == null ? 0 : result.plan.openings.Count;
            int issueCount = result == null || result.issues == null ? 0 : result.issues.Count;

            Debug.Log(
                $"Level 0 synthesis only complete. succeeded: {result != null && result.succeeded}, rooms: {roomCount}, connections: {connectionCount}, openings: {openingCount}, issues: {issueCount}");
        }

        private static void WriteJson(string path, object value)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            File.WriteAllText(path, JsonUtility.ToJson(value, true));
        }

        [Serializable]
        private class LayoutPlanSummary
        {
            public bool succeeded;
            public string requestId;
            public string packageId;
            public string levelId;
            public int seed;
            public string planId;
            public string sceneName;
            public int roomCount;
            public int connectionCount;
            public int openingCount;
            public int lightCount;
            public int transitionCount;
            public int landmarkCount;

            public static LayoutPlanSummary FromResult(LayoutSynthesisResult result)
            {
                LayoutPlanSummary summary = new LayoutPlanSummary
                {
                    succeeded = result.succeeded,
                    requestId = result.requestId,
                    packageId = result.packageId,
                    levelId = result.levelId,
                    seed = result.seed
                };

                if (result.plan == null)
                {
                    return summary;
                }

                summary.planId = result.plan.planId;
                summary.sceneName = result.plan.sceneName;
                summary.roomCount = result.plan.rooms == null ? 0 : result.plan.rooms.Count;
                summary.connectionCount = result.plan.connections == null ? 0 : result.plan.connections.Count;
                summary.openingCount = result.plan.openings == null ? 0 : result.plan.openings.Count;
                summary.lightCount = result.plan.lights == null ? 0 : result.plan.lights.Count;
                summary.transitionCount = result.plan.transitions == null ? 0 : result.plan.transitions.Count;
                summary.landmarkCount = result.plan.landmarks == null ? 0 : result.plan.landmarks.Count;
                return summary;
            }
        }
    }
}
#endif
