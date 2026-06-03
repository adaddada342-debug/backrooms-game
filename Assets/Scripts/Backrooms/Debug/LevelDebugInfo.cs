using Backrooms.Atmosphere;
using Backrooms.Grammar;
using Backrooms.Validation;
using UnityEngine;

namespace Backrooms.Debugging
{
    public class LevelDebugInfo : MonoBehaviour
    {
        [Header("Profiles")]
        public LevelIdentityProfile identity;
        public RoomGrammarProfile grammar;
        public AtmosphereProfile atmosphere;
        public AssemblyValidationReport validationReport;

        [Header("Validation Summary")]
        public string levelId;
        public string identityName;
        public string grammarId;
        public string atmosphereId;
        public bool validationPassed;
        public float grammarScore;
        public float atmosphereScore;
        public float landmarkScore;
        public float identityScore;
        public float routeScore;
        public int validationIssueCount;

        public void Configure(
            LevelIdentityProfile newIdentity,
            RoomGrammarProfile newGrammar,
            AtmosphereProfile newAtmosphere,
            AssemblyValidationReport newValidationReport)
        {
            identity = newIdentity;
            grammar = newGrammar;
            atmosphere = newAtmosphere;
            validationReport = newValidationReport;

            levelId = newIdentity == null ? string.Empty : newIdentity.levelId;
            identityName = newIdentity == null ? string.Empty : newIdentity.displayName;
            grammarId = newGrammar == null ? string.Empty : newGrammar.grammarId;
            atmosphereId = newAtmosphere == null ? string.Empty : newAtmosphere.atmosphereId;

            validationPassed = newValidationReport != null && newValidationReport.passed;
            grammarScore = newValidationReport == null ? 0f : newValidationReport.grammarScore;
            atmosphereScore = newValidationReport == null ? 0f : newValidationReport.atmosphereScore;
            landmarkScore = newValidationReport == null ? 0f : newValidationReport.landmarkScore;
            identityScore = newValidationReport == null ? 0f : newValidationReport.identityScore;
            routeScore = newValidationReport == null ? 0f : newValidationReport.routeScore;
            validationIssueCount = newValidationReport == null || newValidationReport.issues == null
                ? 0
                : newValidationReport.issues.Count;
        }
    }
}
