using Backrooms.Atmosphere;
using Backrooms.Atmosphere.Reports;
using Backrooms.Grammar;
using Backrooms.LayoutSynthesis.Landmarks;
using Backrooms.LayoutSynthesis.Routes;
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
        public int roomAtmosphereTagCount;
        public int soundEmitterCount;
        public int materialProfileCount;
        public bool atmosphereApplied;
        public bool soundscapeCreated;
        public int routeLength;
        public float routeComplexity;
        public bool routeReachesTransition;
        public int landmarkPlacementCount;
        public bool mapNotePrototypeEnabled;

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
            roomAtmosphereTagCount = 0;
            soundEmitterCount = 0;
            materialProfileCount = 0;
            atmosphereApplied = false;
            soundscapeCreated = false;
            routeLength = 0;
            routeComplexity = 0f;
            routeReachesTransition = false;
            landmarkPlacementCount = 0;
            mapNotePrototypeEnabled = false;
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

        public void Configure(
            SceneAssemblyPlan plan,
            AssemblyValidationReport newValidationReport,
            RouteReadabilityReport readabilityReport,
            AtmosphereApplicationReport atmosphereReport)
        {
            Configure(plan, newValidationReport, readabilityReport);

            if (atmosphereReport == null)
            {
                return;
            }

            atmosphereId = atmosphereReport.atmosphereId;
            roomAtmosphereTagCount = atmosphereReport.roomAtmosphereTagCount;
            soundEmitterCount = atmosphereReport.soundEmitterCount;
            materialProfileCount = atmosphereReport.materialProfileCount;
            atmosphereApplied = atmosphereReport.renderSettingsApplied || atmosphereReport.fogApplied;
            soundscapeCreated = atmosphereReport.soundscapeRuntimeCreated;
        }

        public void Configure(
            SceneAssemblyPlan plan,
            AssemblyValidationReport newValidationReport,
            RouteReadabilityReport readabilityReport,
            AtmosphereApplicationReport atmosphereReport,
            LayoutRouteAnnotation routeAnnotation,
            LandmarkPlacementPlan landmarkPlacementPlan)
        {
            Configure(plan, newValidationReport, readabilityReport, atmosphereReport);

            routeLength = routeAnnotation == null ? 0 : routeAnnotation.routeLength;
            routeComplexity = routeAnnotation == null ? 0f : routeAnnotation.routeComplexity;
            routeReachesTransition = routeAnnotation != null && routeAnnotation.reachesTransition;
            landmarkPlacementCount = landmarkPlacementPlan == null || landmarkPlacementPlan.placements == null
                ? 0
                : landmarkPlacementPlan.placements.Count;
            mapNotePrototypeEnabled = true;
        }
    }
}
