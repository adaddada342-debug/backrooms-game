#if UNITY_EDITOR
using System.IO;
using Backrooms.SceneAssembly;
using Backrooms.SceneAssembly.Primitive;
using UnityEditor;
using UnityEngine;

namespace Backrooms.Soundscape.Editor
{
    public static class Level0SoundscapeReportRunner
    {
        private const string ReportPath = "Assets/Data/Soundscape/Reports/level0_soundscape_plan.json";

        [MenuItem("Backrooms/Soundscape/Generate Level 0 Soundscape Report Only")]
        public static void GenerateLevel0SoundscapeReportOnly()
        {
            SceneAssemblyPlan scenePlan = PrimitiveLevel0BlockoutFactory.CreateSynthesizedDefaultPlan();
            SoundscapePlan soundscapePlan = Level0SoundscapeFactory.CreatePlan(scenePlan);

            Directory.CreateDirectory(Path.GetDirectoryName(ReportPath));
            File.WriteAllText(ReportPath, JsonUtility.ToJson(soundscapePlan, true));
            AssetDatabase.Refresh();

            int emitterCount = soundscapePlan == null || soundscapePlan.emitters == null ? 0 : soundscapePlan.emitters.Count;
            Debug.Log($"Level 0 soundscape report generated. emitters: {emitterCount}, path: {ReportPath}");
        }
    }
}
#endif
