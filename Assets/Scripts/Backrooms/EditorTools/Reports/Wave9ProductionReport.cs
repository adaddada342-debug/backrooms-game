using System;
using System.Collections.Generic;

namespace Backrooms.EditorTools.Reports
{
    [Serializable]
    public class Wave9ProductionReport
    {
        public string reportId;
        public string packageId;
        public string levelId;
        public int seed;
        public bool synthesisSucceeded;
        public bool assemblyValidationPassed;
        public bool readabilityPassed;
        public bool atmosphereApplied;
        public bool soundscapeCreated;
        public bool routeAnnotated;
        public bool landmarkPlacementsCreated;
        public bool mappingPrototypeEnabled;
        public int roomCount;
        public int routeLength;
        public int landmarkPlacementCount;
        public int materialProfileCount;
        public int soundEmitterCount;
        public float readabilityScore;
        public List<string> warnings = new List<string>();

        public void AddWarning(string warning)
        {
            if (!string.IsNullOrWhiteSpace(warning))
            {
                warnings.Add(warning);
            }
        }
    }
}
