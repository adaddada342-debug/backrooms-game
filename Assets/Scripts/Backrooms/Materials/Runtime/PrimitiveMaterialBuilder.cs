using System.Collections.Generic;
using UnityEngine;

namespace Backrooms.Materials.Runtime
{
    public static class PrimitiveMaterialBuilder
    {
        public static Material BuildMaterial(PrimitiveMaterialProfile profile)
        {
            Shader shader = FindBestShader();
            if (shader == null)
            {
                return null;
            }

            Material material = new Material(shader)
            {
                name = profile == null || string.IsNullOrWhiteSpace(profile.materialId)
                    ? "PrimitiveMaterial_Unknown"
                    : profile.materialId
            };

            Color baseColor = profile == null ? Color.white : profile.baseColor;
            if (profile != null && profile.transparent)
            {
                baseColor.a = Mathf.Clamp01(profile.alpha);
                ApplyTransparency(material);
            }

            material.color = baseColor;
            if (material.HasProperty("_BaseColor"))
            {
                material.SetColor("_BaseColor", baseColor);
            }

            if (profile != null)
            {
                if (material.HasProperty("_Smoothness"))
                {
                    material.SetFloat("_Smoothness", Mathf.Clamp01(profile.smoothness));
                }

                if (material.HasProperty("_Metallic"))
                {
                    material.SetFloat("_Metallic", Mathf.Clamp01(profile.metallic));
                }

                if (profile.emissionIntensity > 0f)
                {
                    ApplyEmission(material, profile);
                }
            }

            // Exact HDRP material tuning is deferred until the final material pipeline is asset-backed.
            return material;
        }

        public static Dictionary<MaterialRole, Material> BuildRoleMap(PrimitiveMaterialLibrary library)
        {
            Dictionary<MaterialRole, Material> roleMap = new Dictionary<MaterialRole, Material>();
            if (library == null || library.materials == null)
            {
                return roleMap;
            }

            foreach (PrimitiveMaterialProfile profile in library.materials)
            {
                if (profile == null || roleMap.ContainsKey(profile.role))
                {
                    continue;
                }

                roleMap.Add(profile.role, BuildMaterial(profile));
            }

            return roleMap;
        }

        private static Shader FindBestShader()
        {
            Shader shader = Shader.Find("HDRP/Lit");
            if (shader == null)
            {
                shader = Shader.Find("Standard");
            }

            if (shader == null)
            {
                shader = Shader.Find("Universal Render Pipeline/Lit");
            }

            if (shader == null)
            {
                shader = Shader.Find("Sprites/Default");
            }

            return shader;
        }

        private static void ApplyTransparency(Material material)
        {
            if (material == null)
            {
                return;
            }

            material.SetFloat("_Mode", 3f);
            material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            material.SetInt("_ZWrite", 0);
            material.DisableKeyword("_ALPHATEST_ON");
            material.EnableKeyword("_ALPHABLEND_ON");
            material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            material.renderQueue = 3000;
        }

        private static void ApplyEmission(Material material, PrimitiveMaterialProfile profile)
        {
            Color emission = profile.emissionColor * Mathf.Max(0f, profile.emissionIntensity);
            material.EnableKeyword("_EMISSION");
            if (material.HasProperty("_EmissionColor"))
            {
                material.SetColor("_EmissionColor", emission);
            }
        }
    }
}
