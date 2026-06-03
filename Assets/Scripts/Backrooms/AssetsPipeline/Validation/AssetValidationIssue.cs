using System;

namespace Backrooms.AssetsPipeline.Validation
{
    [Serializable]
    public class AssetValidationIssue
    {
        public string code;
        public string message;
        public AssetValidationSeverity severity;
        public bool retryAllowed;
        public bool userActionRequired;
    }
}
