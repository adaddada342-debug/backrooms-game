#if UNITY_EDITOR
using System;
using System.IO;
using Backrooms.Mapping.Persistence;
using UnityEditor;
using UnityEngine;

namespace Backrooms.Mapping.Editor
{
    public static class MappingPrototypeReportRunner
    {
        private const string ReportPath = "Assets/Data/Mapping/wave9_mapping_prototype_report.json";

        [MenuItem("Backrooms/Mapping/Write Mapping Prototype Report")]
        public static void WriteMappingPrototypeReport()
        {
            MappingPrototypeReport report = new MappingPrototypeReport
            {
                featureName = "Wave 11 Mapping Discovery and Note Editing Prototype",
                notePlacementKey = "N",
                savesToDisk = true,
                hasRuntimeMarker = true,
                hasMapUi = true,
                notes = "Local-only mapping prototype with discovery, fog-of-war map state, persistent notes, and note editing/deleting UI."
            };

            Directory.CreateDirectory(Path.GetDirectoryName(ReportPath));
            File.WriteAllText(ReportPath, JsonUtility.ToJson(report, true));
            AssetDatabase.Refresh();
            Debug.Log("Mapping prototype report written: " + ReportPath);
        }

        [MenuItem("Backrooms/Mapping/Clear Local Runtime Map Save")]
        public static void ClearLocalRuntimeMapSave()
        {
            string savePath = LocalMapSaveService.GetSavePath();
            if (File.Exists(savePath))
            {
                File.Delete(savePath);
                Debug.Log("Deleted local runtime map save: " + savePath);
            }
            else
            {
                Debug.Log("No local runtime map save existed at: " + savePath);
            }
        }

        [Serializable]
        private class MappingPrototypeReport
        {
            public string featureName;
            public string notePlacementKey;
            public bool savesToDisk;
            public bool hasRuntimeMarker;
            public bool hasMapUi;
            public string notes;
        }
    }
}
#endif
