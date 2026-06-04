using Backrooms.Atmosphere;
using Backrooms.Grammar;
using Backrooms.LayoutSynthesis.Scoring;
using Backrooms.SceneAssembly;
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
        public int roomCount;
        public int connectionCount;
        public int openingCount;
        public int landmarkCount;
        public bool readabilityPassed;
        public float readabilityScore;

        public void Configure(
            LevelIdentityProfile newIdentity,
            RoomGrammarProfile newGrammar,
            AtmosphereProfile newAtmosphere,
            AssemblyValidationReport newValidationReport)
        {
            Configure(newIdentity, newGrammar, newAtmosphere, newValidationReport, null);
        }

        public void Configure(
            LevelIdentityProfile newIdentity,
            RoomGrammarProfile newGrammar,
            AtmosphereProfile newAtmosphere,
            AssemblyValidationReport newValidationReport,
            SceneAssemblyPlan plan)
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
            roomCount = plan == null || plan.rooms == null ? 0 : plan.rooms.Count;
            connectionCount = plan == null || plan.connections == null ? 0 : plan.connections.Count;
            openingCount = plan == null || plan.openings == null ? 0 : plan.openings.Count;
            landmarkCount = plan == null || plan.landmarks == null ? 0 : plan.landmarks.Count;
            readabilityPassed = false;
            readabilityScore = 0f;
        }

        public void Configure(
            SceneAssemblyPlan plan,
            AssemblyValidationReport newValidationReport,
            RouteReadabilityReport readabilityReport)
        {
            Configure(
                plan == null ? null : plan.identity,
                plan == null ? null : plan.grammar,
                plan == null ? null : plan.atmosphere,
                newValidationReport,
                plan);

            readabilityPassed = readabilityReport != null && readabilityReport.passed;
            readabilityScore = readabilityReport == null ? 0f : readabilityReport.totalScore;
        }
    }
}
