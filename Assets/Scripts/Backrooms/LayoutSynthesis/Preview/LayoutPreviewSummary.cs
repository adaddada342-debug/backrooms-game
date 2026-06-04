using System;
using Backrooms.LayoutSynthesis.Models;
using Backrooms.LayoutSynthesis.Routes;
using Backrooms.LayoutSynthesis.Scoring;
using Backrooms.Validation;

namespace Backrooms.LayoutSynthesis.Preview
{
    [Serializable]
    public class LayoutPreviewSummary
    {
        public string previewId;
        public string packageId;
        public string levelId;
        public int seed;
        public bool synthesisSucceeded;
        public bool assemblyValidationPassed;
        public bool readabilityPassed;
        public float readabilityScore;
        public int roomCount;
        public int connectionCount;
        public int openingCount;
        public int landmarkCount;
        public int routeLength;
        public float routeComplexity;
        public int issueCount;
        public string summaryText;

        public static LayoutPreviewSummary From(
            LayoutSynthesisResult synthesisResult,
            AssemblyValidationReport assemblyReport,
            RouteReadabilityReport readabilityReport,
            LayoutRouteAnnotation routeAnnotation)
        {
            LayoutPreviewSummary summary = new LayoutPreviewSummary();
            summary.previewId = synthesisResult == null ? "missing_preview" : synthesisResult.requestId + "_preview";
            summary.packageId = synthesisResult == null ? string.Empty : synthesisResult.packageId;
            summary.levelId = synthesisResult == null ? string.Empty : synthesisResult.levelId;
            summary.seed = synthesisResult == null ? 0 : synthesisResult.seed;
            summary.synthesisSucceeded = synthesisResult != null && synthesisResult.succeeded;
            summary.assemblyValidationPassed = assemblyReport != null && assemblyReport.passed;
            summary.readabilityPassed = readabilityReport != null && readabilityReport.passed;
            summary.readabilityScore = readabilityReport == null ? 0f : readabilityReport.totalScore;
            summary.issueCount = synthesisResult == null || synthesisResult.issues == null ? 0 : synthesisResult.issues.Count;

            if (synthesisResult != null && synthesisResult.plan != null)
            {
                summary.roomCount = synthesisResult.plan.rooms == null ? 0 : synthesisResult.plan.rooms.Count;
                summary.connectionCount = synthesisResult.plan.connections == null ? 0 : synthesisResult.plan.connections.Count;
                summary.openingCount = synthesisResult.plan.openings == null ? 0 : synthesisResult.plan.openings.Count;
                summary.landmarkCount = synthesisResult.plan.landmarks == null ? 0 : synthesisResult.plan.landmarks.Count;
            }

            if (routeAnnotation != null)
            {
                summary.routeLength = routeAnnotation.routeLength;
                summary.routeComplexity = routeAnnotation.routeComplexity;
            }

            summary.summaryText = "seed " + summary.seed +
                                  ": synthesis=" + summary.synthesisSucceeded +
                                  ", assembly=" + summary.assemblyValidationPassed +
                                  ", readability=" + summary.readabilityScore.ToString("0.00");
            return summary;
        }
    }
}
