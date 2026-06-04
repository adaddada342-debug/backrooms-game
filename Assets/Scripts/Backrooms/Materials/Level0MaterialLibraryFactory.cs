using UnityEngine;

namespace Backrooms.Materials
{
    public static class Level0MaterialLibraryFactory
    {
        public static PrimitiveMaterialLibrary CreateDefaultLevel0Library()
        {
            PrimitiveMaterialLibrary library = new PrimitiveMaterialLibrary
            {
                libraryId = "level0_primitive_material_library",
                displayName = "Level 0 Primitive Material Library"
            };

            library.materials.Add(Create("level0_wallpaper", "Sickly Yellow Wallpaper", MaterialRole.Wall, new Color(0.78f, 0.73f, 0.43f), Color.black, 0.18f, 0f, 0f, false, 1f, "Primitive yellow wall role."));
            library.materials.Add(Create("level0_damp_carpet", "Damp Carpet", MaterialRole.Floor, new Color(0.42f, 0.38f, 0.25f), Color.black, 0.12f, 0f, 0f, false, 1f, "Primitive carpet role."));
            library.materials.Add(Create("level0_ceiling_tile", "Stained Ceiling Tile", MaterialRole.Ceiling, new Color(0.68f, 0.67f, 0.58f), Color.black, 0.16f, 0f, 0f, false, 1f, "Primitive ceiling tile role."));
            library.materials.Add(Create("level0_fluorescent_tube", "Fluorescent Tube", MaterialRole.Light, new Color(0.82f, 0.95f, 0.83f), new Color(0.75f, 1f, 0.72f), 0.25f, 0f, 1.6f, false, 1f, "Primitive fluorescent light role."));
            library.materials.Add(Create("level0_opening_debug", "Opening Debug", MaterialRole.OpeningDebug, new Color(0.1f, 0.8f, 0.45f), Color.black, 0f, 0f, 0f, true, 0.28f, "Transparent opening marker role."));
            library.materials.Add(Create("level0_transition_debug", "Transition Debug", MaterialRole.TransitionDebug, new Color(0.2f, 0.65f, 1f), Color.black, 0f, 0f, 0f, true, 0.35f, "Transparent transition marker role."));
            library.materials.Add(Create("level0_landmark_debug", "Landmark Debug", MaterialRole.LandmarkDebug, new Color(1f, 0.28f, 0.72f), new Color(1f, 0.2f, 0.55f), 0.2f, 0f, 0.35f, false, 1f, "Magenta debug landmark role."));
            library.materials.Add(Create("level0_connector_strip", "Connector Utility Strip", MaterialRole.Connector, new Color(0.34f, 0.32f, 0.22f), Color.black, 0.1f, 0f, 0f, false, 1f, "Slightly darker connector floor role."));
            library.materials.Add(Create("level0_neutral_debug", "Neutral Debug", MaterialRole.Debug, new Color(0.75f, 0.95f, 1f), Color.black, 0.2f, 0f, 0f, false, 1f, "Neutral debug role."));
            return library;
        }

        private static PrimitiveMaterialProfile Create(
            string materialId,
            string displayName,
            MaterialRole role,
            Color baseColor,
            Color emissionColor,
            float smoothness,
            float metallic,
            float emissionIntensity,
            bool transparent,
            float alpha,
            string notes)
        {
            return new PrimitiveMaterialProfile
            {
                materialId = materialId,
                displayName = displayName,
                role = role,
                baseColor = baseColor,
                emissionColor = emissionColor,
                smoothness = smoothness,
                metallic = metallic,
                emissionIntensity = emissionIntensity,
                transparent = transparent,
                alpha = alpha,
                notes = notes
            };
        }
    }
}
