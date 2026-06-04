using System;
using System.Collections.Generic;

namespace Backrooms.Mapping.Reports
{
    [Serializable]
    public class MappingSystemReport
    {
        public string reportId;
        public string packageId;
        public string levelId;
        public int seed;
        public bool runtimeContextCreated;
        public bool mapUiCreated;
        public bool notePersistenceEnabled;
        public bool localSavePathAvailable;
        public int roomCount;
        public int connectionCount;
        public int landmarkPlacementCount;
        public string savePath;
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
