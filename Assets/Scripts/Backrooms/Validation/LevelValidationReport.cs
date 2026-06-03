using System;
using System.Collections.Generic;

namespace Backrooms.Validation
{
    [Serializable]
    public class LevelValidationReport
    {
        public string reportId;
        public string packageId;
        public bool passed;
        public bool navmeshPassed;
        public bool licensePassed;
        public bool attributionPassed;
        public bool performancePassed;
        public float themeScore;
        public List<ValidationBlocker> blockers = new List<ValidationBlocker>();
        public List<ValidationWarning> warnings = new List<ValidationWarning>();
    }

    [Serializable]
    public class ValidationBlocker
    {
        public string code;
        public string message;
        public bool userActionRequired;
        public bool retryAllowed;
    }

    [Serializable]
    public class ValidationWarning
    {
        public string code;
        public string message;
    }
}
