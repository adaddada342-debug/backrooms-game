using System;
using System.Collections.Generic;
using Backrooms.SceneAssembly;

namespace Backrooms.LayoutSynthesis.Models
{
    [Serializable]
    public class LayoutSynthesisResult
    {
        public bool succeeded;
        public string requestId;
        public string packageId;
        public string levelId;
        public int seed;
        public SceneAssemblyPlan plan;
        public List<LayoutSynthesisIssue> issues = new List<LayoutSynthesisIssue>();

        public bool HasBlockers()
        {
            if (issues == null)
            {
                return false;
            }

            foreach (LayoutSynthesisIssue issue in issues)
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
