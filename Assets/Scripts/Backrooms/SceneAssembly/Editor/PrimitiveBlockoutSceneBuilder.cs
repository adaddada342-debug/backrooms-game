#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using Backrooms.Core;
using Backrooms.LevelPackages;
using Backrooms.Loading;
using Backrooms.Player;
using Backrooms.SceneAssembly.Primitive;
using Backrooms.Transitions;
using Backrooms.Validation;
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
        private const string ValidationReportPath = "Assets/Data/SceneAssembly/Reports/level0_validation_report.json";

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
            Material openingMarkerMaterial = CreateTransparentMaterial("Blockout_Opening_Debug", new Color(0.1f, 0.8f, 0.45f, 0.28f));
            Material triggerMarkerMaterial = CreateTransparentMaterial("Blockout_Transition_Trigger_Debug", new Color(0.2f, 0.65f, 1f, 0.35f));

            GameObject geometryRoot = new GameObject("Blockout_Geometry");
            foreach (BlockoutRoomPlan room in plan.rooms)
            {
                CreateRoom(
                    geometryRoot.transform,
                    room,
                    GetOpeningsForRoom(plan, room.roomId),
                    wallMaterial,
                    floorMaterial,
                    ceilingMaterial,
                    openingMarkerMaterial);
            }

            foreach (BlockoutConnectionPlan connection in plan.connections)
            {
                CreateConnection(geometryRoot.transform, connection, wallMaterial, floorMaterial, ceilingMaterial);
            }

            foreach (BlockoutLightPlan lightPlan in plan.lights)
            {
                CreateLightBar(geometryRoot.transform, lightPlan, lightMaterial);
            }

            ValidatePlanHasBasicRoute(plan, result);
            AssemblyValidationReport validationReport = AssemblyValidator.Validate(plan);
            WriteValidationReport(validationReport);

            GameObject runtimeRoot = new GameObject("Backrooms_Runtime");
            LevelLoader levelLoader = runtimeRoot.AddComponent<LevelLoader>();
            LevelPackageRegistry registry = CreateOrUpdateRegistry(plan);
            levelLoader.SetRegistry(registry);
            CreateDebugObject(plan, validationReport);

            CreatePlayer();

            foreach (BlockoutTransitionPlan transition in plan.transitions)
            {
                CreateTransitionTrigger(transition, levelLoader, triggerMarkerMaterial);
            }

            GameObject spawnMarker = new GameObject("PlayerSpawn");
            spawnMarker.transform.position = new Vector3(0f, 1f, -2f);

            Directory.CreateDirectory(Path.GetDirectoryName(ScenePath));
            bool saved = EditorSceneManager.SaveScene(scene, ScenePath);
            EnsureSceneInBuildSettings(ScenePath);

            result.succeeded = saved && !HasBlockers(result);
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
            Debug.Log(
                $"Assembly validation scores - Grammar: {validationReport.grammarScore:0.00}, Atmosphere: {validationReport.atmosphereScore:0.00}, Landmark: {validationReport.landmarkScore:0.00}, Identity: {validationReport.identityScore:0.00}, Route: {validationReport.routeScore:0.00}");
        }

        private static void CreateRoom(
            Transform parent,
            BlockoutRoomPlan room,
            List<BlockoutOpeningPlan> openings,
            Material wallMaterial,
            Material floorMaterial,
            Material ceilingMaterial,
            Material openingMarkerMaterial)
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

            // Wave 4.1 primitive simplification: walls with openings are omitted instead of cut with boolean geometry.
            if (!HasOpeningOnWall(openings, "north"))
            {
                CreateCube(root.transform, room.roomId + "_wall_north_solid", new Vector3(center.x, wallY, center.z + size.z * 0.5f), new Vector3(size.x, size.y, wallThickness), wallMaterial);
            }

            if (!HasOpeningOnWall(openings, "south"))
            {
                CreateCube(root.transform, room.roomId + "_wall_south_solid", new Vector3(center.x, wallY, center.z - size.z * 0.5f), new Vector3(size.x, size.y, wallThickness), wallMaterial);
            }

            if (!HasOpeningOnWall(openings, "east"))
            {
                CreateCube(root.transform, room.roomId + "_wall_east_solid", new Vector3(center.x + size.x * 0.5f, wallY, center.z), new Vector3(wallThickness, size.y, size.z), wallMaterial);
            }

            if (!HasOpeningOnWall(openings, "west"))
            {
                CreateCube(root.transform, room.roomId + "_wall_west_solid", new Vector3(center.x - size.x * 0.5f, wallY, center.z), new Vector3(wallThickness, size.y, size.z), wallMaterial);
            }

            foreach (BlockoutOpeningPlan opening in openings)
            {
                CreateOpeningMarker(root.transform, opening, openingMarkerMaterial);
            }
        }

        private static void CreateConnection(
            Transform parent,
            BlockoutConnectionPlan connection,
            Material wallMaterial,
            Material floorMaterial,
            Material ceilingMaterial)
        {
            GameObject root = new GameObject("OpenConnector_" + connection.connectionId);
            root.transform.SetParent(parent);

            Vector3 center = connection.position;
            Vector3 size = connection.size;
            float slabThickness = 0.12f;

            // Primitive connector volumes are intentionally open in Wave 4.1 so the test route is walkable.
            CreateCube(root.transform, connection.connectionId + "_connector_floor", new Vector3(center.x, center.y, center.z), new Vector3(size.x, slabThickness, size.z), floorMaterial);
            CreateCube(root.transform, connection.connectionId + "_connector_ceiling", new Vector3(center.x, center.y + size.y, center.z), new Vector3(size.x, slabThickness, size.z), ceilingMaterial);
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
                UnityEngine.Object.DestroyImmediate(capsuleCollider);
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

        private static void CreateTransitionTrigger(
            BlockoutTransitionPlan transition,
            LevelLoader levelLoader,
            Material triggerMarkerMaterial)
        {
            GameObject trigger = new GameObject("TransitionTrigger_" + transition.transitionId);
            trigger.transform.position = transition.position;
            BoxCollider boxCollider = trigger.AddComponent<BoxCollider>();
            boxCollider.isTrigger = true;
            boxCollider.size = transition.size;

            GameObject marker = CreateCube(trigger.transform, "TransitionTriggerMarker_" + transition.transitionId, Vector3.zero, transition.size, triggerMarkerMaterial);
            Collider markerCollider = marker.GetComponent<Collider>();
            if (markerCollider != null)
            {
                UnityEngine.Object.DestroyImmediate(markerCollider);
            }

            LevelTransitionTrigger transitionTrigger = trigger.AddComponent<LevelTransitionTrigger>();
            transitionTrigger.Configure(
                transition.targetLevelId,
                transition.targetPackageId,
                transition.transitionType,
                levelLoader);
        }

        private static void CreateDebugObject(
            SceneAssemblyPlan plan,
            AssemblyValidationReport validationReport)
        {
            GameObject debugObject = new GameObject("Backrooms_Debug");
            LevelDebugInfo debugInfo = debugObject.AddComponent<LevelDebugInfo>();
            debugInfo.Configure(plan.identity, plan.grammar, plan.atmosphere, validationReport);
        }

        private static List<BlockoutOpeningPlan> GetOpeningsForRoom(SceneAssemblyPlan plan, string roomId)
        {
            List<BlockoutOpeningPlan> matches = new List<BlockoutOpeningPlan>();
            if (plan == null || plan.openings == null || string.IsNullOrWhiteSpace(roomId))
            {
                return matches;
            }

            foreach (BlockoutOpeningPlan opening in plan.openings)
            {
                if (opening == null)
                {
                    continue;
                }

                if (string.Equals(opening.roomId, roomId, StringComparison.Ordinal))
                {
                    matches.Add(opening);
                }
            }

            return matches;
        }

        private static bool HasOpeningOnWall(List<BlockoutOpeningPlan> openings, string directionHint)
        {
            if (openings == null || string.IsNullOrWhiteSpace(directionHint))
            {
                return false;
            }

            foreach (BlockoutOpeningPlan opening in openings)
            {
                if (opening == null)
                {
                    continue;
                }

                if (string.Equals(opening.directionHint, directionHint, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        private static void CreateOpeningMarker(
            Transform parent,
            BlockoutOpeningPlan opening,
            Material openingMarkerMaterial)
        {
            GameObject marker = CreateCube(
                parent,
                "OpeningMarker_" + opening.openingId,
                opening.position,
                opening.size,
                openingMarkerMaterial);

            Collider markerCollider = marker.GetComponent<Collider>();
            if (markerCollider != null)
            {
                UnityEngine.Object.DestroyImmediate(markerCollider);
            }
        }

        private static void ValidatePlanHasBasicRoute(SceneAssemblyPlan plan, SceneAssemblyResult result)
        {
            if (plan == null)
            {
                AddIssue(result, "plan.missing", "Scene assembly plan is missing.", true);
                return;
            }

            if (plan.rooms == null || plan.rooms.Count == 0)
            {
                AddIssue(result, "rooms.missing", "Plan must contain at least one room.", true);
            }

            if (plan.connections == null || plan.connections.Count == 0)
            {
                AddIssue(result, "connections.missing", "Plan must contain at least one connection.", true);
            }

            if (plan.transitions == null || plan.transitions.Count == 0)
            {
                AddIssue(result, "transitions.missing", "Plan must contain at least one transition trigger.", true);
            }

            if (!HasRoom(plan, "spawn_office"))
            {
                AddIssue(result, "route.spawn_missing", "Plan is missing required room 'spawn_office'.", true);
            }

            if (!HasRoom(plan, "long_corridor"))
            {
                AddIssue(result, "route.corridor_missing", "Plan is missing required room 'long_corridor'.", true);
            }

            if (!HasRoom(plan, "transition_room"))
            {
                AddIssue(result, "route.transition_room_missing", "Plan is missing required room 'transition_room'.", true);
            }

            if (!HasRoom(plan, "side_dead_end"))
            {
                AddIssue(result, "route.side_dead_end_missing", "Plan is missing optional side dead-end room.", false);
            }

            if (!HasConnection(plan, "spawn_office", "long_corridor"))
            {
                AddIssue(result, "route.spawn_to_corridor_missing", "Plan must connect spawn_office to long_corridor.", true);
            }

            if (!HasConnection(plan, "long_corridor", "transition_room"))
            {
                AddIssue(result, "route.corridor_to_transition_missing", "Plan must connect long_corridor to transition_room.", true);
            }
        }

        private static bool HasRoom(SceneAssemblyPlan plan, string roomId)
        {
            if (plan.rooms == null)
            {
                return false;
            }

            foreach (BlockoutRoomPlan room in plan.rooms)
            {
                if (room != null && string.Equals(room.roomId, roomId, StringComparison.Ordinal))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool HasConnection(SceneAssemblyPlan plan, string firstRoomId, string secondRoomId)
        {
            if (plan.connections == null)
            {
                return false;
            }

            foreach (BlockoutConnectionPlan connection in plan.connections)
            {
                if (connection == null)
                {
                    continue;
                }

                bool forward = string.Equals(connection.fromRoomId, firstRoomId, StringComparison.Ordinal) &&
                               string.Equals(connection.toRoomId, secondRoomId, StringComparison.Ordinal);
                bool reverse = string.Equals(connection.fromRoomId, secondRoomId, StringComparison.Ordinal) &&
                               string.Equals(connection.toRoomId, firstRoomId, StringComparison.Ordinal);

                if (forward || reverse)
                {
                    return true;
                }
            }

            return false;
        }

        private static void AddIssue(
            SceneAssemblyResult result,
            string code,
            string message,
            bool blocker)
        {
            if (result == null)
            {
                return;
            }

            result.issues.Add(new SceneAssemblyIssue
            {
                code = code,
                message = message,
                blocker = blocker
            });
        }

        private static bool HasBlockers(SceneAssemblyResult result)
        {
            if (result == null || result.issues == null)
            {
                return false;
            }

            foreach (SceneAssemblyIssue issue in result.issues)
            {
                if (issue != null && issue.blocker)
                {
                    return true;
                }
            }

            return false;
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

        private static Material CreateTransparentMaterial(string name, Color color)
        {
            Shader shader = Shader.Find("Standard");
            Material material = new Material(shader)
            {
                name = name,
                color = color
            };

            material.SetFloat("_Mode", 3f);
            material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            material.SetInt("_ZWrite", 0);
            material.DisableKeyword("_ALPHATEST_ON");
            material.EnableKeyword("_ALPHABLEND_ON");
            material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            material.renderQueue = 3000;
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

        private static void WriteValidationReport(AssemblyValidationReport report)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(ValidationReportPath));
            File.WriteAllText(ValidationReportPath, JsonUtility.ToJson(report, true));
        }
    }
}
#endif
