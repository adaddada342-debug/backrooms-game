using System;
using System.Collections.Generic;

namespace Backrooms.LayoutSynthesis.Scoring
{
    [Serializable]
    public class RouteReadabilityReport
    {
        public bool passed;
        public float totalScore;
        public float routeDirectnessScore;
        public float branchClarityScore;
        public float landmarkSupportScore;
        public float deadEndPenaltyScore;
        public float transitionFindabilityScore;
        public int mainRouteRoomCount;
        public int branchCount;
        public int deadEndCount;
        public int landmarkCount;
        public int requiredLandmarkCount;
        public int missingRequiredLandmarkCount;
        public int transitionCount;
        public List<RouteReadabilityIssue> issues = new List<RouteReadabilityIssue>();

        public bool HasBlockers()
        {
            if (issues == null)
            {
                return false;
            }

            foreach (RouteReadabilityIssue issue in issues)
            {
                if (issue != null && issue.blocker)
                {
                    return true;
                }
            }

            return false;
        }
    }

    [Serializable]
    public class RouteReadabilityIssue
    {
        public string code;
        public string message;
        public bool blocker;
    }
}
