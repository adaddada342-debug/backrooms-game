#if UNITY_EDITOR
using System.IO;
using Backrooms.Materials.Runtime;
using UnityEditor;
using UnityEngine;

namespace Backrooms.Materials.Editor
{
    public static class PrimitiveMaterialAssetGenerator
    {
        private const string OutputFolder = "Assets/Data/Materials/Generated";

        [MenuItem("Backrooms/Materials/Generate Level 0 Primitive Material Assets")]
        public static void GenerateLevel0PrimitiveMaterialAssets()
        {
            PrimitiveMaterialLibrary library = Level0MaterialLibraryFactory.CreateDefaultLevel0Library();
            Directory.CreateDirectory(OutputFolder);

            int created = 0;
            int updated = 0;
            int skipped = 0;
            foreach (PrimitiveMaterialProfile profile in library.materials)
            {
                if (profile == null)
                {
                    skipped++;
                    continue;
                }

                Material material = PrimitiveMaterialBuilder.BuildMaterial(profile);
                if (material == null)
                {
                    Debug.LogWarning("Skipped material profile because no compatible shader was found: " + profile.materialId);
                    skipped++;
                    continue;
                }

                string path = OutputFolder + "/" + profile.role.ToString().ToLowerInvariant() + "_" + profile.materialId + ".mat";
                Material existing = AssetDatabase.LoadAssetAtPath<Material>(path);
                if (existing == null)
                {
                    AssetDatabase.CreateAsset(material, path);
                    created++;
                }
                else
                {
                    EditorUtility.CopySerialized(material, existing);
                    EditorUtility.SetDirty(existing);
                    updated++;
                }
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log($"Level 0 primitive material assets generated. created: {created}, updated: {updated}, skipped: {skipped}");
        }
    }
}
#endif
