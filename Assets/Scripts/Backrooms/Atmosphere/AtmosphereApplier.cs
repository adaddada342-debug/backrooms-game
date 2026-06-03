using UnityEngine;

namespace Backrooms.Atmosphere
{
    public static class AtmosphereApplier
    {
        public static void ApplyBasicAtmosphere(AtmosphereProfile profile)
        {
            if (profile == null)
            {
                return;
            }

            RenderSettings.ambientLight = profile.ambientColor;
            RenderSettings.fog = true;
            RenderSettings.fogDensity = profile.fogDensity;
            // TODO: Integrate HDRP Volume settings after the production atmosphere pipeline is defined.
        }
    }
}
