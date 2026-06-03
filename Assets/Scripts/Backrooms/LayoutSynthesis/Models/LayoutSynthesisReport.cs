using System;
using System.Collections.Generic;

namespace Backrooms.LayoutSynthesis.Models
{
    [Serializable]
    public class LayoutSynthesisReport
    {
        public bool succeeded;
        public bool fallbackUsed;
        public string requestId;
        public string packageId;
        public string levelId;
        public int seed;
        public int issueCount;
        public List<LayoutSynthesisIssue> issues = new List<LayoutSynthesisIssue>();
        public int generatedRoomCount;
        public int generatedConnectionCount;
        public int generatedOpeningCount;
        public int generatedLandmarkCount;

        public static LayoutSynthesisReport FromResult(LayoutSynthesisResult result, bool fallbackUsed)
        {
            LayoutSynthesisReport report = new LayoutSynthesisReport
            {
                fallbackUsed = fallbackUsed
            };

            if (result == null)
            {
                report.succeeded = false;
                report.issueCount = 1;
                report.issues.Add(new LayoutSynthesisIssue
                {
                    code = "synthesis.result_missing",
                    message = "No layout synthesis result was available.",
                    blocker = true
                });
                return report;
            }

            report.succeeded = result.succeeded;
            report.requestId = result.requestId;
            report.packageId = result.packageId;
            report.levelId = result.levelId;
            report.seed = result.seed;
            report.issues = result.issues ?? new List<LayoutSynthesisIssue>();
            report.issueCount = report.issues.Count;

            if (result.plan != null)
            {
                report.generatedRoomCount = result.plan.rooms == null ? 0 : result.plan.rooms.Count;
                report.generatedConnectionCount = result.plan.connections == null ? 0 : result.plan.connections.Count;
                report.generatedOpeningCount = result.plan.openings == null ? 0 : result.plan.openings.Count;
                report.generatedLandmarkCount = result.plan.landmarks == null ? 0 : result.plan.landmarks.Count;
            }

            return report;
        }
    }
}
