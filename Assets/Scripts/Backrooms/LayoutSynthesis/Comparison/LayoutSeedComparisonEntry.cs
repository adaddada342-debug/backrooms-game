using System;

namespace Backrooms.LayoutSynthesis.Comparison
{
    [Serializable]
    public class LayoutSeedComparisonEntry
    {
        public int seed;
        public bool synthesisSucceeded;
        public bool assemblyValidationPassed;
        public bool readabilityPassed;
        public float readabilityScore;
        public int roomCount;
        public int connectionCount;
        public int openingCount;
        public int landmarkCount;
        public int issueCount;
        public string summary;
    }
}
