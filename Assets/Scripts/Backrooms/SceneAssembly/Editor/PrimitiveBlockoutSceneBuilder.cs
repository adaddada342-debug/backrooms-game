#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using Backrooms.Core;
using Backrooms.LevelPackages;
using Backrooms.Loading;
using Backrooms.Player;
using Backrooms.SceneAssembly.Primitive;
using Backrooms.Transitions;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Backrooms.SceneAssembly.Editor
{
    public static class PrimitiveBlockoutSceneBuilder
    {
        private const string ScenePath = "Assets/Scenes/Level0_Local_Blockout.unity";
        private const string RegistryPath = "Assets/Data/LevelPackages/LevelPackageRegistry.asset";
        private const string ReportPath = "Assets/Data/SceneAssembly/Reports/level0_local_blockout_assembly_report.json";

        [MenuItem("Backrooms/Scene Assembly/Create Level 0 Local Blockout Scene")]
        public static void CreateScene()
        {
            SceneAssemblyPlan plan = PrimitiveLevel0BlockoutFactory.CreateDefaultPlan();
            SceneAssemblyResult result = new SceneAssemblyResult
            {
                sceneName = plan.sceneName,
                scenePath = ScenePath,
                planId = plan.planId,
                issues = new List<SceneAssemblyIssue>()
            };

            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            scene.name = plan.sceneName;

            Material wallMaterial = CreateMaterial("Blockout_SicklyYellow_Wall", new Color(0.78f, 0.73f, 0.43f));
            Material floorMaterial = CreateMaterial("Blockout_DampCarpet_Floor", new Color(0.42f, 0.38f, 0.25f));
            Material ceilingMaterial = CreateMaterial("Blockout_CeilingTile", new Color(0.68f, 0.67f, 0.58f));
            Material lightMaterial = CreateMaterial("Blockout_Fluorescent_Light", new Color(0.82f, 0.95f, 0.83f));

            GameObject geometryRoot = new GameObject("Blockout_Geometry");
            foreach (BlockoutRoomPlan room in plan.rooms)
            {
                CreateRoom(geometryRoot.transform, room, wallMaterial, floorMaterial, ceilingMaterial);
            }

            foreach (BlockoutConnectionPlan connection in plan.connections)
            {
                CreateConnection(geometryRoot.transform, connection, wallMaterial, floorMaterial, ceilingMaterial);
            }

            foreach (BlockoutLightPlan lightPlan in plan.lights)
            {
                CreateLightBar(geometryRoot.transform, lightPlan, lightMaterial);
            }

            GameObject runtimeRoot = new GameObject("Backrooms_Runtime");
            LevelLoader levelLoader = runtimeRoot.AddComponent<LevelLoader>();
            LevelPackageRegistry registry = CreateOrUpdateRegistry(plan);
            levelLoader.SetRegistry(registry);

            CreatePlayer();

            foreach (BlockoutTransitionPlan transition in plan.transitions)
            {
                CreateTransitionTrigger(transition, levelLoader);
            }

            GameObject spawnMarker = new GameObject("PlayerSpawn");
            spawnMarker.transform.position = new Vector3(0f, 1f, -2f);

            Directory.CreateDirectory(Path.GetDirectoryName(ScenePath));
            bool saved = EditorSceneManager.SaveScene(scene, ScenePath);
            EnsureSceneInBuildSettings(ScenePath);

            result.succeeded = saved;
            if (!saved)
            {
                result.issues.Add(new SceneAssemblyIssue
                {
                    code = "scene.save_failed",
                    message = "Unity failed to save the generated blockout scene.",
                    blocker = true
                });
            }

            WriteAssemblyReport(result);
            AssetDatabase.Refresh();

            Debug.Log(
                $"Level 0 local blockout scene created. Scene: {ScenePath}, saved: {saved}, report: {ReportPath}");
        }

        private static void CreateRoom(
            Transform parent,
            BlockoutRoomPlan room,
            Material wallMaterial,
            Material floorMaterial,
            Material ceilingMaterial)
        {
            GameObject root = new GameObject("Room_" + room.roomId);
            root.transform.SetParent(parent);

            Vector3 center = room.position;
            Vector3 size = room.size;
            float wallThickness = 0.15f;
            float floorY = center.y;
            float ceilingY = center.y + size.y;
            float wallY = center.y + size.y * 0.5f;

            CreateCube(root.transform, room.roomId + "_floor", new Vector3(center.x, floorY, center.z), new Vector3(size.x, wallThickness, size.z), floorMaterial);
            CreateCube(root.transform, room.roomId + "_ceiling", new Vector3(center.x, ceilingY, center.z), new Vector3(size.x, wallThickness, size.z), ceilingMaterial);
            CreateCube(root.transform, room.roomId + "_wall_north", new Vector3(center.x, wallY, center.z + size.z * 0.5f), new Vector3(size.x, size.y, wallThickness), wallMaterial);
            CreateCube(root.transform, room.roomId + "_wall_south", new Vector3(center.x, wallY, center.z - size.z * 0.5f), new Vector3(size.x, size.y, wallThickness), wallMaterial);
            CreateCube(root.transform, room.roomId + "_wall_east", new Vector3(center.x + size.x * 0.5f, wallY, center.z), new Vector3(wallThickness, size.y, size.z), wallMaterial);
            CreateCube(root.transform, room.roomId + "_wall_west", new Vector3(center.x - size.x * 0.5f, wallY, center.z), new Vector3(wallThickness, size.y, size.z), wallMaterial);
        }

        private static void CreateConnection(
            Transform parent,
            BlockoutConnectionPlan connection,
            Material wallMaterial,
            Material floorMaterial,
            Material ceilingMaterial)
        {
            BlockoutRoomPlan room = new BlockoutRoomPlan
            {
                roomId = connection.connectionId,
                roomType = "connection",
                position = connection.position,
                size = connection.size,
                materialHint = "connection"
            };

            CreateRoom(parent, room, wallMaterial, floorMaterial, ceilingMaterial);
        }

        private static void CreateLightBar(Transform parent, BlockoutLightPlan lightPlan, Material lightMaterial)
        {
            GameObject bar = CreateCube(parent, lightPlan.lightId, lightPlan.position, lightPlan.size, lightMaterial);
            Light light = bar.AddComponent<Light>();
            light.type = LightType.Point;
            light.intensity = lightPlan.intensity;
            light.range = 7f;
            light.color = new Color(0.86f, 1f, 0.84f);
        }

        private static GameObject CreateCube(
            Transform parent,
            string name,
            Vector3 position,
            Vector3 scale,
            Material material)
        {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.name = name;
            cube.transform.SetParent(parent);
            cube.transform.position = position;
            cube.transform.localScale = scale;
            Renderer renderer = cube.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.sharedMaterial = material;
            }

            return cube;
        }

        private static void CreatePlayer()
        {
            GameObject player = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            player.name = "Player";
            player.tag = "Player";
            player.transform.position = new Vector3(0f, 1.1f, -2f);

            Collider capsuleCollider = player.GetComponent<Collider>();
            if (capsuleCollider != null)
            {
                Object.DestroyImmediate(capsuleCollider);
            }

            CharacterController characterController = player.AddComponent<CharacterController>();
            characterController.height = 1.8f;
            characterController.radius = 0.35f;
            characterController.center = new Vector3(0f, 0.9f, 0f);

            GameObject cameraObject = new GameObject("PlayerCamera");
            cameraObject.transform.SetParent(player.transform);
            cameraObject.transform.localPosition = new Vector3(0f, 1.6f, 0f);
            Camera camera = cameraObject.AddComponent<Camera>();
            camera.nearClipPlane = 0.03f;

            SimpleFirstPersonController controller = player.AddComponent<SimpleFirstPersonController>();
            controller.SetCameraTransform(cameraObject.transform);
        }

        private static void CreateTransitionTrigger(BlockoutTransitionPlan transition, LevelLoader levelLoader)
        {
            GameObject trigger = new GameObject("TransitionTrigger_" + transition.transitionId);
            trigger.transform.position = transition.position;
            BoxCollider boxCollider = trigger.AddComponent<BoxCollider>();
            boxCollider.isTrigger = true;
            boxCollider.size = transition.size;

            LevelTransitionTrigger transitionTrigger = trigger.AddComponent<LevelTransitionTrigger>();
            transitionTrigger.Configure(
                transition.targetLevelId,
                transition.targetPackageId,
                transition.transitionType,
                levelLoader);
        }

        private static LevelPackageRegistry CreateOrUpdateRegistry(SceneAssemblyPlan plan)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(RegistryPath));
            LevelPackageRegistry registry = AssetDatabase.LoadAssetAtPath<LevelPackageRegistry>(RegistryPath);
            if (registry == null)
            {
                registry = ScriptableObject.CreateInstance<LevelPackageRegistry>();
                AssetDatabase.CreateAsset(registry, RegistryPath);
            }

            registry.packages.Clear();
            registry.packages.Add(new LevelPackageManifest
            {
                packageId = plan.packageId,
                levelId = plan.levelId,
                displayName = "Level 0 Local Blockout",
                schemaVersion = BackroomsConstants.CurrentSchemaVersion,
                packageVersion = "0.1.0-local",
                seed = plan.seed,
                sceneName = plan.sceneName,
                sceneAddress = string.Empty,
                creditsId = plan.packageId + "_credits",
                validationReportId = plan.packageId + "_validation",
                estimatedSizeMb = 0.1f,
                checksum = plan.packageId + "_primitive_blockout"
            });

            EditorUtility.SetDirty(registry);
            AssetDatabase.SaveAssets();
            return registry;
        }

        private static Material CreateMaterial(string name, Color color)
        {
            Shader shader = Shader.Find("HDRP/Lit");
            if (shader == null)
            {
                shader = Shader.Find("Standard");
            }

            Material material = new Material(shader)
            {
                name = name,
                color = color
            };

            return material;
        }

        private static void EnsureSceneInBuildSettings(string scenePath)
        {
            List<EditorBuildSettingsScene> scenes = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
            foreach (EditorBuildSettingsScene scene in scenes)
            {
                if (scene.path == scenePath)
                {
                    scene.enabled = true;
                    EditorBuildSettings.scenes = scenes.ToArray();
                    return;
                }
            }

            scenes.Add(new EditorBuildSettingsScene(scenePath, true));
            EditorBuildSettings.scenes = scenes.ToArray();
        }

        private static void WriteAssemblyReport(SceneAssemblyResult result)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(ReportPath));
            File.WriteAllText(ReportPath, JsonUtility.ToJson(result, true));
        }
    }
}
#endif
