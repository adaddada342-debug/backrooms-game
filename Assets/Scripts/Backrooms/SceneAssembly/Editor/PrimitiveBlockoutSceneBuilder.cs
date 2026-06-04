#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using Backrooms.Atmosphere;
using Backrooms.Atmosphere.Reports;
using Backrooms.Atmosphere.Runtime;
using Backrooms.Core;
using Backrooms.Debugging;
using Backrooms.LevelPackages;
using Backrooms.Landmarks;
using Backrooms.Landmarks.Runtime;
using Backrooms.LayoutSynthesis.Gizmos;
using Backrooms.LayoutSynthesis.Models;
using Backrooms.LayoutSynthesis.Scoring;
using Backrooms.Loading;
using Backrooms.Materials;
using Backrooms.Materials.Runtime;
using Backrooms.Player;
using Backrooms.SceneAssembly.Primitive;
using Backrooms.Soundscape;
using Backrooms.Soundscape.Runtime;
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
        private const string SynthesisReportPath = "Assets/Data/LayoutSynthesis/Reports/level0_synthesis_report.json";
        private const string ReadabilityReportPath = "Assets/Data/LayoutSynthesis/ReadabilityReports/level0_readability_report.json";
        private const string AtmosphereReportPath = "Assets/Data/Atmosphere/Reports/level0_atmosphere_application_report.json";
        private const string RoomAtmosphereTagsPath = "Assets/Data/Atmosphere/Reports/level0_room_atmosphere_tags.json";
        private const string SoundscapePlanPath = "Assets/Data/Soundscape/Reports/level0_soundscape_plan.json";

        [MenuItem("Backrooms/Scene Assembly/Create Level 0 Local Blockout Scene")]
        public static void CreateScene()
        {
            SceneAssemblyPlan plan = PrimitiveLevel0BlockoutFactory.CreateSynthesizedDefaultPlan();
            WriteSynthesisReport(
                PrimitiveLevel0BlockoutFactory.LastSynthesisResult,
                PrimitiveLevel0BlockoutFactory.LastSynthesisUsedFallback);
            SceneAssemblyResult result = new SceneAssemblyResult
            {
                sceneName = plan.sceneName,
                scenePath = ScenePath,
                planId = plan.planId,
                issues = new List<SceneAssemblyIssue>()
            };

            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            scene.name = plan.sceneName;

            PrimitiveMaterialLibrary materialLibrary = Level0MaterialLibraryFactory.CreateDefaultLevel0Library();
            Dictionary<MaterialRole, Material> roleMaterials = PrimitiveMaterialBuilder.BuildRoleMap(materialLibrary);
            Material wallMaterial = GetRoleMaterial(roleMaterials, MaterialRole.Wall, new Color(0.78f, 0.73f, 0.43f));
            Material floorMaterial = GetRoleMaterial(roleMaterials, MaterialRole.Floor, new Color(0.42f, 0.38f, 0.25f));
            Material connectorMaterial = GetRoleMaterial(roleMaterials, MaterialRole.Connector, new Color(0.34f, 0.32f, 0.22f));
            Material ceilingMaterial = GetRoleMaterial(roleMaterials, MaterialRole.Ceiling, new Color(0.68f, 0.67f, 0.58f));
            Material lightMaterial = GetRoleMaterial(roleMaterials, MaterialRole.Light, new Color(0.82f, 0.95f, 0.83f));
            Material openingMarkerMaterial = GetRoleMaterial(roleMaterials, MaterialRole.OpeningDebug, new Color(0.1f, 0.8f, 0.45f, 0.28f));
            Material triggerMarkerMaterial = GetRoleMaterial(roleMaterials, MaterialRole.TransitionDebug, new Color(0.2f, 0.65f, 1f, 0.35f));
            Material landmarkMaterial = GetRoleMaterial(roleMaterials, MaterialRole.LandmarkDebug, new Color(1f, 0.28f, 0.72f));
            List<RoomAtmosphereTag> roomAtmosphereTags = RoomAtmospherePlanner.CreateRoomTags(plan);
            SoundscapePlan soundscapePlan = Level0SoundscapeFactory.CreatePlan(plan);
            AtmosphereApplicationReport atmosphereReport = AtmosphereApplier.ApplyToScene(plan, materialLibrary, soundscapePlan);
            atmosphereReport.roomAtmosphereTagCount = roomAtmosphereTags.Count;
            WriteRoomAtmosphereTags(roomAtmosphereTags);
            WriteSoundscapePlan(soundscapePlan);

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
                CreateConnection(geometryRoot.transform, connection, wallMaterial, connectorMaterial, ceilingMaterial);
            }

            foreach (BlockoutLightPlan lightPlan in plan.lights)
            {
                CreateLightBar(geometryRoot.transform, lightPlan, lightMaterial, plan.atmosphere);
            }

            LayoutDebugGizmo layoutDebugGizmo = CreateLayoutDebugGizmo(plan);
            CreateLandmarkPlaceholders(geometryRoot.transform, plan, landmarkMaterial, layoutDebugGizmo);

            ValidatePlanHasBasicRoute(plan, result);
            AssemblyValidationReport validationReport = AssemblyValidator.Validate(plan);
            WriteValidationReport(validationReport);
            RouteReadabilityReport readabilityReport = RouteReadabilityScorer.Score(plan);
            WriteReadabilityReport(readabilityReport);
            if (!validationReport.passed)
            {
                Debug.LogWarning("Assembly validation failed for the generated Level 0 plan. The scene builder will continue unless scene assembly blockers are present.");
            }

            GameObject runtimeRoot = new GameObject("Backrooms_Runtime");
            LevelLoader levelLoader = runtimeRoot.AddComponent<LevelLoader>();
            LevelPackageRegistry registry = CreateOrUpdateRegistry(plan);
            levelLoader.SetRegistry(registry);
            CreateSoundscapeRuntime(soundscapePlan, atmosphereReport);
            WriteAtmosphereReport(atmosphereReport);
            CreateDebugObject(plan, validationReport, readabilityReport, atmosphereReport);

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
            Debug.Log($"Route readability score: {readabilityReport.totalScore:0.00}, passed: {readabilityReport.passed}");
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

            CreateSegmentedWall(root.transform, room, openings, "north", wallMaterial, wallThickness);
            CreateSegmentedWall(root.transform, room, openings, "south", wallMaterial, wallThickness);
            CreateSegmentedWall(root.transform, room, openings, "east", wallMaterial, wallThickness);
            CreateSegmentedWall(root.transform, room, openings, "west", wallMaterial, wallThickness);

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

        private static void CreateSegmentedWall(
            Transform parent,
            BlockoutRoomPlan room,
            List<BlockoutOpeningPlan> openings,
            string directionHint,
            Material wallMaterial,
            float wallThickness)
        {
            Vector3 center = room.position;
            Vector3 size = room.size;
            float wallY = center.y + size.y * 0.5f;
            BlockoutOpeningPlan opening = GetFirstOpeningOnWall(openings, directionHint);

            if (opening == null)
            {
                CreateSolidWall(parent, room.roomId + "_wall_" + directionHint + "_solid", center, size, directionHint, wallMaterial, wallThickness, wallY);
                return;
            }

            // Wave 7 keeps this intentionally simple: if multiple openings share one wall, the first one drives the wall gaps.
            if (directionHint == "north" || directionHint == "south")
            {
                float wallZ = directionHint == "north" ? center.z + size.z * 0.5f : center.z - size.z * 0.5f;
                float leftEdge = center.x - size.x * 0.5f;
                float rightEdge = center.x + size.x * 0.5f;
                float openingWidth = Mathf.Max(OpeningAxisWidth(opening, directionHint), 0.2f);
                float gapStart = Mathf.Clamp(opening.position.x - openingWidth * 0.5f, leftEdge, rightEdge);
                float gapEnd = Mathf.Clamp(opening.position.x + openingWidth * 0.5f, leftEdge, rightEdge);

                CreateHorizontalWallSegment(parent, room.roomId, directionHint, "left", leftEdge, gapStart, wallZ, center.y, size.y, wallMaterial, wallThickness);
                CreateHorizontalWallSegment(parent, room.roomId, directionHint, "right", gapEnd, rightEdge, wallZ, center.y, size.y, wallMaterial, wallThickness);
                CreateHorizontalTopSegment(parent, room.roomId, directionHint, opening.position.x, openingWidth, wallZ, center.y, size.y, opening.size.y, wallMaterial, wallThickness);
            }
            else
            {
                float wallX = directionHint == "east" ? center.x + size.x * 0.5f : center.x - size.x * 0.5f;
                float nearEdge = center.z - size.z * 0.5f;
                float farEdge = center.z + size.z * 0.5f;
                float openingWidth = Mathf.Max(OpeningAxisWidth(opening, directionHint), 0.2f);
                float gapStart = Mathf.Clamp(opening.position.z - openingWidth * 0.5f, nearEdge, farEdge);
                float gapEnd = Mathf.Clamp(opening.position.z + openingWidth * 0.5f, nearEdge, farEdge);

                CreateVerticalWallSegment(parent, room.roomId, directionHint, "near", wallX, nearEdge, gapStart, center.y, size.y, wallMaterial, wallThickness);
                CreateVerticalWallSegment(parent, room.roomId, directionHint, "far", wallX, gapEnd, farEdge, center.y, size.y, wallMaterial, wallThickness);
                CreateVerticalTopSegment(parent, room.roomId, directionHint, wallX, opening.position.z, openingWidth, center.y, size.y, opening.size.y, wallMaterial, wallThickness);
            }
        }

        private static void CreateSolidWall(
            Transform parent,
            string name,
            Vector3 center,
            Vector3 size,
            string directionHint,
            Material wallMaterial,
            float wallThickness,
            float wallY)
        {
            if (directionHint == "north")
            {
                CreateCube(parent, name, new Vector3(center.x, wallY, center.z + size.z * 0.5f), new Vector3(size.x, size.y, wallThickness), wallMaterial);
            }
            else if (directionHint == "south")
            {
                CreateCube(parent, name, new Vector3(center.x, wallY, center.z - size.z * 0.5f), new Vector3(size.x, size.y, wallThickness), wallMaterial);
            }
            else if (directionHint == "east")
            {
                CreateCube(parent, name, new Vector3(center.x + size.x * 0.5f, wallY, center.z), new Vector3(wallThickness, size.y, size.z), wallMaterial);
            }
            else if (directionHint == "west")
            {
                CreateCube(parent, name, new Vector3(center.x - size.x * 0.5f, wallY, center.z), new Vector3(wallThickness, size.y, size.z), wallMaterial);
            }
        }

        private static void CreateHorizontalWallSegment(
            Transform parent,
            string roomId,
            string directionHint,
            string segmentName,
            float startX,
            float endX,
            float wallZ,
            float floorY,
            float height,
            Material wallMaterial,
            float wallThickness)
        {
            float segmentWidth = endX - startX;
            if (segmentWidth <= 0.2f)
            {
                return;
            }

            CreateCube(
                parent,
                roomId + "_wall_" + directionHint + "_" + segmentName,
                new Vector3(startX + segmentWidth * 0.5f, floorY + height * 0.5f, wallZ),
                new Vector3(segmentWidth, height, wallThickness),
                wallMaterial);
        }

        private static void CreateVerticalWallSegment(
            Transform parent,
            string roomId,
            string directionHint,
            string segmentName,
            float wallX,
            float startZ,
            float endZ,
            float floorY,
            float height,
            Material wallMaterial,
            float wallThickness)
        {
            float segmentLength = endZ - startZ;
            if (segmentLength <= 0.2f)
            {
                return;
            }

            CreateCube(
                parent,
                roomId + "_wall_" + directionHint + "_" + segmentName,
                new Vector3(wallX, floorY + height * 0.5f, startZ + segmentLength * 0.5f),
                new Vector3(wallThickness, height, segmentLength),
                wallMaterial);
        }

        private static void CreateHorizontalTopSegment(
            Transform parent,
            string roomId,
            string directionHint,
            float openingCenterX,
            float openingWidth,
            float wallZ,
            float floorY,
            float wallHeight,
            float doorHeight,
            Material wallMaterial,
            float wallThickness)
        {
            float topHeight = wallHeight - Mathf.Clamp(doorHeight, 0f, wallHeight);
            if (topHeight <= 0.2f)
            {
                return;
            }

            CreateCube(
                parent,
                roomId + "_wall_" + directionHint + "_top",
                new Vector3(openingCenterX, floorY + doorHeight + topHeight * 0.5f, wallZ),
                new Vector3(openingWidth, topHeight, wallThickness),
                wallMaterial);
        }

        private static void CreateVerticalTopSegment(
            Transform parent,
            string roomId,
            string directionHint,
            float wallX,
            float openingCenterZ,
            float openingWidth,
            float floorY,
            float wallHeight,
            float doorHeight,
            Material wallMaterial,
            float wallThickness)
        {
            float topHeight = wallHeight - Mathf.Clamp(doorHeight, 0f, wallHeight);
            if (topHeight <= 0.2f)
            {
                return;
            }

            CreateCube(
                parent,
                roomId + "_wall_" + directionHint + "_top",
                new Vector3(wallX, floorY + doorHeight + topHeight * 0.5f, openingCenterZ),
                new Vector3(wallThickness, topHeight, openingWidth),
                wallMaterial);
        }

        private static float OpeningAxisWidth(BlockoutOpeningPlan opening, string directionHint)
        {
            return directionHint == "east" || directionHint == "west"
                ? Mathf.Max(opening.size.z, opening.size.x)
                : Mathf.Max(opening.size.x, opening.size.z);
        }

        private static void CreateLightBar(
            Transform parent,
            BlockoutLightPlan lightPlan,
            Material lightMaterial,
            AtmosphereProfile atmosphere)
        {
            GameObject bar = CreateCube(parent, lightPlan.lightId, lightPlan.position, lightPlan.size, lightMaterial);
            Light light = bar.AddComponent<Light>();
            light.type = LightType.Point;
            light.intensity = lightPlan.intensity;
            light.range = 7f;
            light.color = new Color(0.86f, 1f, 0.84f);

            FluorescentFlicker flicker = bar.AddComponent<FluorescentFlicker>();
            flicker.targetLight = light;
            flicker.targetRenderer = bar.GetComponent<Renderer>();
            flicker.baseIntensity = lightPlan.intensity;
            flicker.flickerChancePerSecond = atmosphere == null ? 0.08f : atmosphere.flickerChance;
            flicker.deterministic = true;
            flicker.seed = StableHash(lightPlan.lightId);
        }

        private static int StableHash(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return 0;
            }

            unchecked
            {
                int hash = 17;
                for (int i = 0; i < value.Length; i++)
                {
                    hash = hash * 31 + value[i];
                }

                return hash;
            }
        }

        private static Material GetRoleMaterial(
            Dictionary<MaterialRole, Material> roleMaterials,
            MaterialRole role,
            Color fallbackColor)
        {
            Material material;
            if (roleMaterials != null &&
                roleMaterials.TryGetValue(role, out material) &&
                material != null)
            {
                return material;
            }

            return CreateMaterial("Fallback_" + role, fallbackColor);
        }

        private static void CreateSoundscapeRuntime(
            SoundscapePlan soundscapePlan,
            AtmosphereApplicationReport atmosphereReport)
        {
            GameObject soundscapeObject = new GameObject("Backrooms_Soundscape");
            SoundscapeRuntime runtime = soundscapeObject.AddComponent<SoundscapeRuntime>();
            runtime.Configure(soundscapePlan);

            if (atmosphereReport != null)
            {
                atmosphereReport.soundscapeRuntimeCreated = true;
            }
        }

        private static LayoutDebugGizmo CreateLayoutDebugGizmo(SceneAssemblyPlan plan)
        {
            GameObject gizmoObject = new GameObject("Layout_Debug_Gizmos");
            LayoutDebugGizmo gizmo = gizmoObject.AddComponent<LayoutDebugGizmo>();
            gizmo.Configure(plan);
            return gizmo;
        }

        private static void CreateLandmarkPlaceholders(
            Transform parent,
            SceneAssemblyPlan plan,
            Material landmarkMaterial,
            LayoutDebugGizmo layoutDebugGizmo)
        {
            if (plan == null || plan.landmarks == null || plan.landmarks.Count == 0)
            {
                Debug.LogWarning("No Level 0 landmarks were available for debug placeholder creation.");
                return;
            }

            List<BlockoutRoomPlan> rooms = GetNonNullRooms(plan);
            if (rooms.Count == 0)
            {
                Debug.LogWarning("No rooms were available for debug landmark placeholder placement.");
                return;
            }

            for (int i = 0; i < plan.landmarks.Count; i++)
            {
                LandmarkProfile landmark = plan.landmarks[i];
                if (landmark == null)
                {
                    continue;
                }

                BlockoutRoomPlan room = rooms[i % rooms.Count];
                Vector3 position = CreateLandmarkPosition(room, i);
                GameObject placeholder = CreateLandmarkPrimitive(parent, landmark, position, landmarkMaterial);
                LandmarkPlaceholder component = placeholder.AddComponent<LandmarkPlaceholder>();
                component.Configure(landmark);

                Collider collider = placeholder.GetComponent<Collider>();
                if (collider != null)
                {
                    UnityEngine.Object.DestroyImmediate(collider);
                }

                if (layoutDebugGizmo != null)
                {
                    layoutDebugGizmo.AddLandmark(landmark.landmarkId, position);
                }
            }
        }

        private static List<BlockoutRoomPlan> GetNonNullRooms(SceneAssemblyPlan plan)
        {
            List<BlockoutRoomPlan> rooms = new List<BlockoutRoomPlan>();
            if (plan.rooms == null)
            {
                return rooms;
            }

            foreach (BlockoutRoomPlan room in plan.rooms)
            {
                if (room != null)
                {
                    rooms.Add(room);
                }
            }

            return rooms;
        }

        private static Vector3 CreateLandmarkPosition(BlockoutRoomPlan room, int index)
        {
            float offsetX = ((index % 3) - 1) * Mathf.Min(1.5f, room.size.x * 0.2f);
            float offsetZ = ((index / 3) % 3 - 1) * Mathf.Min(1.5f, room.size.z * 0.2f);
            return new Vector3(room.position.x + offsetX, room.position.y + 0.35f, room.position.z + offsetZ);
        }

        private static GameObject CreateLandmarkPrimitive(
            Transform parent,
            LandmarkProfile landmark,
            Vector3 position,
            Material landmarkMaterial)
        {
            PrimitiveType primitiveType = PrimitiveType.Cube;
            if (string.Equals(landmark.landmarkType, "floor_trace", StringComparison.OrdinalIgnoreCase))
            {
                primitiveType = PrimitiveType.Sphere;
            }
            else if (string.Equals(landmark.landmarkType, "lighting_anomaly", StringComparison.OrdinalIgnoreCase))
            {
                primitiveType = PrimitiveType.Cylinder;
            }

            GameObject primitive = GameObject.CreatePrimitive(primitiveType);
            primitive.name = "Landmark_" + landmark.landmarkId;
            primitive.transform.SetParent(parent);
            primitive.transform.position = position;
            primitive.transform.localScale = primitiveType == PrimitiveType.Sphere
                ? new Vector3(1.2f, 0.12f, 1.2f)
                : new Vector3(0.6f, 0.6f, 0.6f);

            Renderer renderer = primitive.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.sharedMaterial = landmarkMaterial;
            }

            return primitive;
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
            AssemblyValidationReport validationReport,
            RouteReadabilityReport readabilityReport,
            AtmosphereApplicationReport atmosphereReport)
        {
            GameObject debugObject = new GameObject("Backrooms_Debug");
            Backrooms.Debugging.LevelDebugInfo debugInfo = debugObject.AddComponent<Backrooms.Debugging.LevelDebugInfo>();
            debugInfo.Configure(plan, validationReport, readabilityReport, atmosphereReport);
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
            return GetFirstOpeningOnWall(openings, directionHint) != null;
        }

        private static BlockoutOpeningPlan GetFirstOpeningOnWall(List<BlockoutOpeningPlan> openings, string directionHint)
        {
            if (openings == null || string.IsNullOrWhiteSpace(directionHint))
            {
                return null;
            }

            foreach (BlockoutOpeningPlan opening in openings)
            {
                if (opening != null && string.Equals(opening.directionHint, directionHint, StringComparison.OrdinalIgnoreCase))
                {
                    return opening;
                }
            }

            return null;
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

            if (!HasRoute(plan, "long_corridor", "transition_room"))
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

        private static bool HasRoute(SceneAssemblyPlan plan, string startRoomId, string targetRoomId)
        {
            Dictionary<string, List<string>> graph = new Dictionary<string, List<string>>(StringComparer.Ordinal);
            if (plan.rooms != null)
            {
                foreach (BlockoutRoomPlan room in plan.rooms)
                {
                    if (room != null && !string.IsNullOrWhiteSpace(room.roomId) && !graph.ContainsKey(room.roomId))
                    {
                        graph.Add(room.roomId, new List<string>());
                    }
                }
            }

            if (!graph.ContainsKey(startRoomId) || !graph.ContainsKey(targetRoomId))
            {
                return false;
            }

            if (plan.connections != null)
            {
                foreach (BlockoutConnectionPlan connection in plan.connections)
                {
                    if (connection == null ||
                        !graph.ContainsKey(connection.fromRoomId) ||
                        !graph.ContainsKey(connection.toRoomId))
                    {
                        continue;
                    }

                    graph[connection.fromRoomId].Add(connection.toRoomId);
                    graph[connection.toRoomId].Add(connection.fromRoomId);
                }
            }

            Queue<string> queue = new Queue<string>();
            HashSet<string> visited = new HashSet<string>(StringComparer.Ordinal);
            queue.Enqueue(startRoomId);
            visited.Add(startRoomId);

            while (queue.Count > 0)
            {
                string current = queue.Dequeue();
                if (string.Equals(current, targetRoomId, StringComparison.Ordinal))
                {
                    return true;
                }

                foreach (string next in graph[current])
                {
                    if (visited.Add(next))
                    {
                        queue.Enqueue(next);
                    }
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

        private static void WriteReadabilityReport(RouteReadabilityReport report)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(ReadabilityReportPath));
            File.WriteAllText(ReadabilityReportPath, JsonUtility.ToJson(report, true));
        }

        private static void WriteAtmosphereReport(AtmosphereApplicationReport report)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(AtmosphereReportPath));
            File.WriteAllText(AtmosphereReportPath, JsonUtility.ToJson(report, true));
        }

        private static void WriteRoomAtmosphereTags(List<RoomAtmosphereTag> tags)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(RoomAtmosphereTagsPath));
            File.WriteAllText(RoomAtmosphereTagsPath, JsonUtility.ToJson(new RoomAtmosphereTagList { tags = tags }, true));
        }

        private static void WriteSoundscapePlan(SoundscapePlan plan)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(SoundscapePlanPath));
            File.WriteAllText(SoundscapePlanPath, JsonUtility.ToJson(plan, true));
        }

        private static void WriteSynthesisReport(LayoutSynthesisResult result, bool fallbackUsed)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(SynthesisReportPath));
            File.WriteAllText(SynthesisReportPath, JsonUtility.ToJson(LayoutSynthesisReport.FromResult(result, fallbackUsed), true));
        }

        [Serializable]
        private class RoomAtmosphereTagList
        {
            public List<RoomAtmosphereTag> tags = new List<RoomAtmosphereTag>();
        }
    }
}
#endif
