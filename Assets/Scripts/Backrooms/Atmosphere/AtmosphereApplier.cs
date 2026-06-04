using Backrooms.Atmosphere.Reports;
using Backrooms.Materials;
using Backrooms.SceneAssembly;
using Backrooms.Soundscape;
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

        public static AtmosphereApplicationReport ApplyToScene(
            SceneAssemblyPlan plan,
            PrimitiveMaterialLibrary materialLibrary,
            SoundscapePlan soundscapePlan)
        {
            AtmosphereApplicationReport report = new AtmosphereApplicationReport
            {
                reportId = "level0_atmosphere_application",
                packageId = plan == null ? string.Empty : plan.packageId,
                levelId = plan == null ? string.Empty : plan.levelId,
                atmosphereId = plan == null || plan.atmosphere == null ? string.Empty : plan.atmosphere.atmosphereId,
                materialProfileCount = materialLibrary == null || materialLibrary.materials == null ? 0 : materialLibrary.materials.Count,
                soundEmitterCount = soundscapePlan == null || soundscapePlan.emitters == null ? 0 : soundscapePlan.emitters.Count
            };

            if (plan == null)
            {
                report.AddWarning("SceneAssemblyPlan was missing; atmosphere could not be fully applied.");
                return report;
            }

            if (plan.atmosphere == null)
            {
                report.AddWarning("AtmosphereProfile was missing; RenderSettings were not changed.");
            }
            else
            {
                ApplyBasicAtmosphere(plan.atmosphere);
                report.renderSettingsApplied = true;
                report.fogApplied = true;
            }

            report.materialLibraryApplied = materialLibrary != null && materialLibrary.materials != null && materialLibrary.materials.Count > 0;
            report.soundscapePlanCreated = soundscapePlan != null;
            // TODO: Add HDRP Volume integration with package-safe guards in a future atmosphere pass.
            return report;
        }
    }
}
