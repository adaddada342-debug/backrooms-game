using System;
using System.Collections.Generic;

namespace Backrooms.Atmosphere.Reports
{
    [Serializable]
    public class AtmosphereApplicationReport
    {
        public string reportId;
        public string packageId;
        public string levelId;
        public string atmosphereId;
        public bool renderSettingsApplied;
        public bool fogApplied;
        public bool materialLibraryApplied;
        public bool soundscapePlanCreated;
        public bool soundscapeRuntimeCreated;
        public int roomAtmosphereTagCount;
        public int soundEmitterCount;
        public int materialProfileCount;
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
