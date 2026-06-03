using System;
using System.Collections.Generic;

namespace Backrooms.Validation
{
    [Serializable]
    public class AssemblyValidationReport
    {
        public bool passed;
        public float grammarScore;
        public float atmosphereScore;
        public float landmarkScore;
        public float routeScore;
        public float identityScore;
        public List<AssemblyValidationIssue> issues = new List<AssemblyValidationIssue>();
    }

    [Serializable]
    public class AssemblyValidationIssue
    {
        public string code;
        public string message;
        public bool blocker;
    }
}
