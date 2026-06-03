using Backrooms.Atmosphere;
using Backrooms.Grammar;
using Backrooms.Validation;
using UnityEngine;

namespace Backrooms.SceneAssembly
{
    public class LevelDebugInfo : MonoBehaviour
    {
        public LevelIdentityProfile identity;
        public RoomGrammarProfile grammar;
        public AtmosphereProfile atmosphere;
        public AssemblyValidationReport validationSummary;

        public void Configure(
            LevelIdentityProfile newIdentity,
            RoomGrammarProfile newGrammar,
            AtmosphereProfile newAtmosphere,
            AssemblyValidationReport newValidationSummary)
        {
            identity = newIdentity;
            grammar = newGrammar;
            atmosphere = newAtmosphere;
            validationSummary = newValidationSummary;
        }
    }
}
