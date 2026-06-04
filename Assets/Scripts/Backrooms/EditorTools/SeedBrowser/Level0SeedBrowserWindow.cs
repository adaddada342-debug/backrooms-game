#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using Backrooms.LayoutSynthesis.Level0;
using Backrooms.LayoutSynthesis.Models;
using Backrooms.LayoutSynthesis.Preview;
using Backrooms.LayoutSynthesis.Routes;
using Backrooms.LayoutSynthesis.Scoring;
using Backrooms.SceneAssembly.Editor;
using Backrooms.Validation;
using UnityEditor;
using UnityEngine;

namespace Backrooms.EditorTools.SeedBrowser
{
    public class Level0SeedBrowserWindow : EditorWindow
    {
        private const string SelectedPreviewPath = "Assets/Data/EditorTools/SeedBrowser/level0_selected_seed_preview.json";
        private const string ResultsPath = "Assets/Data/EditorTools/SeedBrowser/level0_seed_browser_results.json";

        private int seedStart = 1001;
        private int seedCount = 20;
        private int selectedSeed = 1001;
        private Vector2 scrollPosition;
        private readonly List<LayoutPreviewSummary> previews = new List<LayoutPreviewSummary>();

        [MenuItem("Backrooms/Tools/Level 0 Seed Browser")]
        public static void Open()
        {
            GetWindow<Level0SeedBrowserWindow>("Level 0 Seeds");
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Level 0 Seed Browser", EditorStyles.boldLabel);
            seedStart = EditorGUILayout.IntField("Seed Start", seedStart);
            seedCount = Mathf.Max(1, EditorGUILayout.IntField("Seed Count", seedCount));
            selectedSeed = EditorGUILayout.IntField("Selected Seed", selectedSeed);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Generate Preview List"))
            {
                GeneratePreviewList();
            }

            if (GUILayout.Button("Generate Selected Scene"))
            {
                PrimitiveBlockoutSceneBuilder.CreateSceneForSeed(selectedSeed);
            }

            if (GUILayout.Button("Write Selected Preview Report"))
            {
                WriteSelectedPreviewReport();
            }

            if (GUILayout.Button("Open Reports Folder"))
            {
                EditorUtility.RevealInFinder("Assets/Data/EditorTools/SeedBrowser");
            }
            EditorGUILayout.EndHorizontal();

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            foreach (LayoutPreviewSummary preview in previews)
            {
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Toggle(selectedSeed == preview.seed, preview.seed.ToString(), "Button", GUILayout.Width(70)))
                {
                    selectedSeed = preview.seed;
                }

                EditorGUILayout.LabelField("synth " + preview.synthesisSucceeded, GUILayout.Width(80));
                EditorGUILayout.LabelField("assembly " + preview.assemblyValidationPassed, GUILayout.Width(100));
                EditorGUILayout.LabelField("read " + preview.readabilityScore.ToString("0.00"), GUILayout.Width(80));
                EditorGUILayout.LabelField("rooms " + preview.roomCount, GUILayout.Width(70));
                EditorGUILayout.LabelField("route " + preview.routeLength, GUILayout.Width(70));
                EditorGUILayout.LabelField("issues " + preview.issueCount, GUILayout.Width(70));
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndScrollView();
        }

        private void GeneratePreviewList()
        {
            previews.Clear();
            for (int i = 0; i < seedCount; i++)
            {
                int seed = seedStart + i;
                previews.Add(CreatePreview(seed));
            }

            Directory.CreateDirectory(Path.GetDirectoryName(ResultsPath));
            File.WriteAllText(ResultsPath, JsonUtility.ToJson(new PreviewList { previews = previews }, true));
            AssetDatabase.Refresh();
        }

        private void WriteSelectedPreviewReport()
        {
            LayoutPreviewSummary selected = null;
            foreach (LayoutPreviewSummary preview in previews)
            {
                if (preview.seed == selectedSeed)
                {
                    selected = preview;
                    break;
                }
            }

            if (selected == null)
            {
                selected = CreatePreview(selectedSeed);
            }

            Directory.CreateDirectory(Path.GetDirectoryName(SelectedPreviewPath));
            File.WriteAllText(SelectedPreviewPath, JsonUtility.ToJson(selected, true));
            AssetDatabase.Refresh();
        }

        private static LayoutPreviewSummary CreatePreview(int seed)
        {
            LayoutSynthesisRequest request = Level0LayoutSynthesisRequestFactory.CreateRequestForSeed(seed);
            Level0LayoutSynthesizer synthesizer = new Level0LayoutSynthesizer();
            LayoutSynthesisResult synthesis = synthesizer.Synthesize(request);
            AssemblyValidationReport assembly = synthesis == null || synthesis.plan == null ? null : AssemblyValidator.Validate(synthesis.plan);
            RouteReadabilityReport readability = synthesis == null || synthesis.plan == null ? null : RouteReadabilityScorer.Score(synthesis.plan);
            LayoutRouteAnnotation route = synthesis == null || synthesis.plan == null ? null : LayoutRouteAnnotator.CreateMainRoute(synthesis.plan);
            return LayoutPreviewSummary.From(synthesis, assembly, readability, route);
        }

        [Serializable]
        private class PreviewList
        {
            public List<LayoutPreviewSummary> previews = new List<LayoutPreviewSummary>();
        }
    }
}
#endif
