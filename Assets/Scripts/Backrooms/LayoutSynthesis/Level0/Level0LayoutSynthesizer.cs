using System;
using System.Collections.Generic;
using Backrooms.Grammar;
using Backrooms.Landmarks;
using Backrooms.LayoutSynthesis.Contracts;
using Backrooms.LayoutSynthesis.Models;
using Backrooms.LayoutSynthesis.Validation;
using Backrooms.SceneAssembly;
using UnityEngine;

namespace Backrooms.LayoutSynthesis.Level0
{
    public class Level0LayoutSynthesizer : ILayoutSynthesizer
    {
        private const float OpeningWidth = 2.8f;
        private const float OpeningHeight = 2.4f;
        private const float OpeningDepth = 0.2f;

        public LayoutSynthesisResult Synthesize(LayoutSynthesisRequest request)
        {
            LayoutSynthesisResult result = CreateResultShell(request);
            if (request == null)
            {
                AddIssue(result, "request.missing", "Cannot synthesize layout without a request.", true);
                result.succeeded = false;
                return result;
            }

            if (!request.HasRequiredProfiles() || !request.HasValidCounts())
            {
                LayoutSynthesisValidator.Validate(request, result);
                return result;
            }

            System.Random random = new System.Random(request.seed);
            Dictionary<Vector2Int, LayoutNode> occupied = new Dictionary<Vector2Int, LayoutNode>();
            List<LayoutNode> nodes = new List<LayoutNode>();
            List<LayoutNode> mainRoute = CreateMainRoute(request, random, occupied, nodes, result);
            List<NodePair> pairs = new List<NodePair>();
            ConnectSequential(mainRoute, pairs);

            CreateBranches(request, random, occupied, nodes, mainRoute, pairs, result);
            CreateDeadEnds(request, random, occupied, nodes, mainRoute, pairs, result);
            PlaceLandmarks(request, random, nodes, result);

            result.plan = BuildPlan(request, nodes, pairs);
            LayoutSynthesisValidator.Validate(request, result);
            return result;
        }

        private static LayoutSynthesisResult CreateResultShell(LayoutSynthesisRequest request)
        {
            return new LayoutSynthesisResult
            {
                requestId = request == null ? string.Empty : request.requestId,
                packageId = request == null ? string.Empty : request.packageId,
                levelId = request == null ? string.Empty : request.levelId,
                seed = request == null ? 0 : request.seed
            };
        }

        private static List<LayoutNode> CreateMainRoute(
            LayoutSynthesisRequest request,
            System.Random random,
            Dictionary<Vector2Int, LayoutNode> occupied,
            List<LayoutNode> nodes,
            LayoutSynthesisResult result)
        {
            List<LayoutNode> route = new List<LayoutNode>();
            int length = Mathf.Clamp(request.targetMainRouteLength, 2, request.targetRoomCount);
            Vector2Int current = new Vector2Int(0, 0);

            for (int i = 0; i < length; i++)
            {
                if (i > 0)
                {
                    current = PickNextRoutePosition(current, i, random, occupied);
                }

                string roomId = CreateMainRouteRoomId(i, length);
                string roomType = CreateMainRouteRoomType(i, length, random);
                LayoutNode node = CreateNode(request, random, roomId, roomType, current, true, false, false, i == length - 1);
                AddNode(node, occupied, nodes);
                route.Add(node);
            }

            if (route.Count < request.targetMainRouteLength)
            {
                AddIssue(result, "route.short", "Main route could not reach the requested length.", false);
            }

            return route;
        }

        private static Vector2Int PickNextRoutePosition(
            Vector2Int current,
            int stepIndex,
            System.Random random,
            Dictionary<Vector2Int, LayoutNode> occupied)
        {
            if (stepIndex > 1 && random.NextDouble() < 0.28)
            {
                int side = random.NextDouble() < 0.5 ? -1 : 1;
                Vector2Int drift = current + new Vector2Int(side, 0);
                if (!occupied.ContainsKey(drift))
                {
                    return drift;
                }
            }

            Vector2Int forward = current + new Vector2Int(0, 1);
            if (!occupied.ContainsKey(forward))
            {
                return forward;
            }

            Vector2Int right = current + new Vector2Int(1, 0);
            if (!occupied.ContainsKey(right))
            {
                return right;
            }

            return current + new Vector2Int(-1, 0);
        }

        private static string CreateMainRouteRoomId(int index, int length)
        {
            if (index == 0)
            {
                return "spawn_office";
            }

            if (index == 1)
            {
                return "long_corridor";
            }

            if (index == length - 1)
            {
                return "transition_room";
            }

            return "main_route_" + index.ToString("00");
        }

        private static string CreateMainRouteRoomType(int index, int length, System.Random random)
        {
            if (index == 0)
            {
                return "spawn_office";
            }

            if (index == 1)
            {
                return "corridor";
            }

            if (index == length - 1)
            {
                return "transition_room";
            }

            double roll = random.NextDouble();
            if (roll < 0.45)
            {
                return "corridor";
            }

            if (roll < 0.75)
            {
                return "office_room";
            }

            return "junction";
        }

        private static void CreateBranches(
            LayoutSynthesisRequest request,
            System.Random random,
            Dictionary<Vector2Int, LayoutNode> occupied,
            List<LayoutNode> nodes,
            List<LayoutNode> mainRoute,
            List<NodePair> pairs,
            LayoutSynthesisResult result)
        {
            if (!request.includeSideBranches || request.grammar == null || !request.grammar.allowBranches)
            {
                return;
            }

            int created = 0;
            int attempts = 0;
            while (created < request.targetBranchCount &&
                   nodes.Count < request.targetRoomCount &&
                   attempts < request.targetBranchCount * 8)
            {
                attempts++;
                LayoutNode anchor = PickBranchAnchor(mainRoute, random);
                int side = random.NextDouble() < 0.5 ? -1 : 1;
                Vector2Int position = anchor.gridPosition + new Vector2Int(side, 0);
                if (occupied.ContainsKey(position))
                {
                    position = anchor.gridPosition + new Vector2Int(-side, 0);
                }

                if (occupied.ContainsKey(position))
                {
                    continue;
                }

                string roomType = PickBranchRoomType(random);
                LayoutNode branch = CreateNode(request, random, "branch_" + (created + 1).ToString("00"), roomType, position, false, true, false, false);
                AddNode(branch, occupied, nodes);
                pairs.Add(new NodePair(anchor, branch));
                created++;
            }

            if (created < request.targetBranchCount)
            {
                AddIssue(result, "branches.partial", "Synthesizer placed fewer side branches than requested.", false);
            }
        }

        private static LayoutNode PickBranchAnchor(List<LayoutNode> mainRoute, System.Random random)
        {
            if (mainRoute.Count <= 2)
            {
                return mainRoute[0];
            }

            int index = random.Next(1, Math.Max(2, mainRoute.Count - 1));
            return mainRoute[index];
        }

        private static string PickBranchRoomType(System.Random random)
        {
            double roll = random.NextDouble();
            if (roll < 0.45)
            {
                return "office_room";
            }

            if (roll < 0.75)
            {
                return "storage_room";
            }

            return "junction";
        }

        private static void CreateDeadEnds(
            LayoutSynthesisRequest request,
            System.Random random,
            Dictionary<Vector2Int, LayoutNode> occupied,
            List<LayoutNode> nodes,
            List<LayoutNode> mainRoute,
            List<NodePair> pairs,
            LayoutSynthesisResult result)
        {
            if (!request.includeDeadEnds || request.grammar == null || !request.grammar.allowDeadEnds)
            {
                return;
            }

            int created = 0;
            int attempts = 0;
            while (created < request.targetDeadEndCount &&
                   nodes.Count < request.targetRoomCount &&
                   attempts < request.targetDeadEndCount * 10)
            {
                attempts++;
                LayoutNode anchor = PickDeadEndAnchor(nodes, mainRoute, random);
                Vector2Int position;
                if (!TryFindFreeNeighbor(anchor.gridPosition, occupied, random, out position))
                {
                    continue;
                }

                string roomId = created == 0 ? "side_dead_end" : "dead_end_" + (created + 1).ToString("00");
                LayoutNode deadEnd = CreateNode(request, random, roomId, "dead_end", position, false, false, true, false);
                AddNode(deadEnd, occupied, nodes);
                pairs.Add(new NodePair(anchor, deadEnd));
                created++;
            }

            if (created < request.targetDeadEndCount)
            {
                AddIssue(result, "dead_ends.partial", "Synthesizer placed fewer dead ends than requested.", false);
            }
        }

        private static LayoutNode PickDeadEndAnchor(List<LayoutNode> nodes, List<LayoutNode> mainRoute, System.Random random)
        {
            List<LayoutNode> candidates = new List<LayoutNode>();
            foreach (LayoutNode node in nodes)
            {
                if (node == null || node.isDeadEnd || node.isTransitionRoom || string.Equals(node.nodeId, "spawn_office", StringComparison.Ordinal))
                {
                    continue;
                }

                candidates.Add(node);
            }

            if (candidates.Count == 0)
            {
                return mainRoute[Mathf.Clamp(mainRoute.Count - 2, 0, mainRoute.Count - 1)];
            }

            return candidates[random.Next(0, candidates.Count)];
        }

        private static bool TryFindFreeNeighbor(
            Vector2Int origin,
            Dictionary<Vector2Int, LayoutNode> occupied,
            System.Random random,
            out Vector2Int position)
        {
            List<Vector2Int> directions = new List<Vector2Int>
            {
                new Vector2Int(-1, 0),
                new Vector2Int(1, 0),
                new Vector2Int(0, 1),
                new Vector2Int(0, -1)
            };

            while (directions.Count > 0)
            {
                int index = random.Next(0, directions.Count);
                Vector2Int candidate = origin + directions[index];
                directions.RemoveAt(index);
                if (!occupied.ContainsKey(candidate))
                {
                    position = candidate;
                    return true;
                }
            }

            position = origin;
            return false;
        }

        private static LayoutNode CreateNode(
            LayoutSynthesisRequest request,
            System.Random random,
            string nodeId,
            string roomType,
            Vector2Int gridPosition,
            bool isMainRoute,
            bool isBranch,
            bool isDeadEnd,
            bool isTransitionRoom)
        {
            return new LayoutNode
            {
                nodeId = nodeId,
                roomType = roomType,
                gridPosition = gridPosition,
                worldPosition = GridToWorld(request, gridPosition),
                size = PickRoomSize(request, random, roomType),
                isMainRoute = isMainRoute,
                isBranch = isBranch,
                isDeadEnd = isDeadEnd,
                isTransitionRoom = isTransitionRoom
            };
        }

        private static void AddNode(LayoutNode node, Dictionary<Vector2Int, LayoutNode> occupied, List<LayoutNode> nodes)
        {
            occupied[node.gridPosition] = node;
            nodes.Add(node);
        }

        private static Vector3 GridToWorld(LayoutSynthesisRequest request, Vector2Int gridPosition)
        {
            float spacing = request.roomSpacing <= 0f ? 10f : request.roomSpacing;
            float gridScale = request.gridSize <= 0f ? 1f : request.gridSize;
            return request.origin + new Vector3(gridPosition.x * spacing * gridScale, 0f, gridPosition.y * spacing * gridScale);
        }

        private static Vector3 PickRoomSize(LayoutSynthesisRequest request, System.Random random, string roomType)
        {
            RoomArchetype archetype = FindArchetype(request, roomType);
            Vector3 min = archetype == null ? new Vector3(4f, request.defaultHeight, 4f) : archetype.minimumSize;
            Vector3 max = archetype == null ? new Vector3(8f, request.defaultHeight, 8f) : archetype.maximumSize;
            float spacingLimit = Mathf.Max(3f, (request.roomSpacing <= 0f ? 10f : request.roomSpacing) - 2f);

            if (min.y <= 0f)
            {
                min.y = request.defaultHeight;
            }

            if (max.y <= 0f)
            {
                max.y = request.defaultHeight;
            }

            Vector3 size = new Vector3(
                Mathf.Lerp(min.x, Mathf.Max(min.x, max.x), (float)random.NextDouble()),
                Mathf.Lerp(min.y, Mathf.Max(min.y, max.y), (float)random.NextDouble()),
                Mathf.Lerp(min.z, Mathf.Max(min.z, max.z), (float)random.NextDouble()));

            size.x = Mathf.Clamp(size.x, 3f, Mathf.Min(16f, spacingLimit));
            size.y = Mathf.Clamp(size.y, 2.4f, 4f);
            size.z = Mathf.Clamp(size.z, 3f, Mathf.Min(30f, spacingLimit));
            return size;
        }

        private static RoomArchetype FindArchetype(LayoutSynthesisRequest request, string roomType)
        {
            if (request.roomArchetypes == null)
            {
                return null;
            }

            foreach (RoomArchetype archetype in request.roomArchetypes)
            {
                if (archetype != null && string.Equals(archetype.roomType, roomType, StringComparison.OrdinalIgnoreCase))
                {
                    return archetype;
                }
            }

            if (string.Equals(roomType, "spawn_office", StringComparison.OrdinalIgnoreCase))
            {
                return FindArchetype(request, "office_room");
            }

            return null;
        }

        private static void ConnectSequential(List<LayoutNode> route, List<NodePair> pairs)
        {
            for (int i = 1; i < route.Count; i++)
            {
                pairs.Add(new NodePair(route[i - 1], route[i]));
            }
        }

        private static void PlaceLandmarks(
            LayoutSynthesisRequest request,
            System.Random random,
            List<LayoutNode> nodes,
            LayoutSynthesisResult result)
        {
            if (!request.includeLandmarks || request.landmarks == null)
            {
                return;
            }

            foreach (string requiredId in SafeStrings(request.identity.requiredLandmarks))
            {
                LandmarkProfile landmark = FindLandmark(request.landmarks, requiredId);
                if (landmark == null || request.identity.ForbidsLandmark(requiredId))
                {
                    AddIssue(result, "landmarks.required_unavailable", "Required landmark '" + requiredId + "' is unavailable or forbidden.", true);
                    continue;
                }

                if (!TryAssignLandmark(landmark, nodes, random, true))
                {
                    AddIssue(result, "landmarks.required_unplaced", "Required landmark '" + requiredId + "' could not be placed.", true);
                }
            }

            int targetOptionalCount = Mathf.RoundToInt(nodes.Count * Mathf.Clamp01(request.identity.landmarkDensity));
            int optionalPlaced = CountAssignedLandmarks(nodes);
            foreach (LandmarkProfile landmark in request.landmarks)
            {
                if (landmark == null ||
                    landmark.requiredForLevelIdentity ||
                    request.identity.ForbidsLandmark(landmark.landmarkId) ||
                    optionalPlaced >= targetOptionalCount)
                {
                    continue;
                }

                if (random.NextDouble() <= Mathf.Clamp01(1f - landmark.rarity) &&
                    TryAssignLandmark(landmark, nodes, random, false))
                {
                    optionalPlaced++;
                }
            }
        }

        private static bool TryAssignLandmark(LandmarkProfile landmark, List<LayoutNode> nodes, System.Random random, bool required)
        {
            List<LayoutNode> candidates = new List<LayoutNode>();
            foreach (LayoutNode node in nodes)
            {
                if (node != null && string.IsNullOrWhiteSpace(node.landmarkId) && CanPlaceLandmark(landmark, node.roomType))
                {
                    candidates.Add(node);
                }
            }

            if (candidates.Count == 0 && required)
            {
                foreach (LayoutNode node in nodes)
                {
                    if (node != null && string.IsNullOrWhiteSpace(node.landmarkId))
                    {
                        candidates.Add(node);
                    }
                }
            }

            if (candidates.Count == 0)
            {
                return false;
            }

            LayoutNode selected = candidates[random.Next(0, candidates.Count)];
            selected.landmarkId = landmark.landmarkId;
            return true;
        }

        private static bool CanPlaceLandmark(LandmarkProfile landmark, string roomType)
        {
            if (landmark == null)
            {
                return false;
            }

            if (Contains(landmark.forbiddenRoomTypes, roomType))
            {
                return false;
            }

            return landmark.allowedRoomTypes == null ||
                   landmark.allowedRoomTypes.Length == 0 ||
                   Contains(landmark.allowedRoomTypes, roomType);
        }

        private static int CountAssignedLandmarks(List<LayoutNode> nodes)
        {
            int count = 0;
            foreach (LayoutNode node in nodes)
            {
                if (node != null && !string.IsNullOrWhiteSpace(node.landmarkId))
                {
                    count++;
                }
            }

            return count;
        }

        private static LandmarkProfile FindLandmark(List<LandmarkProfile> landmarks, string landmarkId)
        {
            foreach (LandmarkProfile landmark in landmarks)
            {
                if (landmark != null && string.Equals(landmark.landmarkId, landmarkId, StringComparison.OrdinalIgnoreCase))
                {
                    return landmark;
                }
            }

            return null;
        }

        private static SceneAssemblyPlan BuildPlan(LayoutSynthesisRequest request, List<LayoutNode> nodes, List<NodePair> pairs)
        {
            SceneAssemblyPlan plan = new SceneAssemblyPlan
            {
                planId = request.requestId + "_plan",
                packageId = request.packageId,
                sceneName = request.targetSceneName,
                levelId = request.levelId,
                seed = request.seed,
                identity = request.identity,
                grammar = request.grammar,
                atmosphere = request.atmosphere
            };

            foreach (LayoutNode node in nodes)
            {
                plan.rooms.Add(new BlockoutRoomPlan
                {
                    roomId = node.nodeId,
                    roomType = node.roomType,
                    position = node.worldPosition,
                    size = node.size,
                    materialHint = "wallpaper"
                });
            }

            List<LayoutEdge> edges = new List<LayoutEdge>();
            foreach (NodePair pair in pairs)
            {
                LayoutEdge edge = CreateEdge(request, pair.from, pair.to);
                edges.Add(edge);
                plan.connections.Add(new BlockoutConnectionPlan
                {
                    connectionId = edge.edgeId,
                    fromRoomId = edge.fromNodeId,
                    toRoomId = edge.toNodeId,
                    position = edge.worldPosition,
                    size = edge.size
                });

                AddOpeningsForEdge(plan, pair.from, pair.to, edge);
            }

            plan.landmarks = BuildPlacedLandmarkList(request, nodes);
            AddLights(request, plan, nodes);
            AddTransition(request, plan, nodes);
            return plan;
        }

        private static LayoutEdge CreateEdge(LayoutSynthesisRequest request, LayoutNode from, LayoutNode to)
        {
            string direction = DirectionFromTo(from.gridPosition, to.gridPosition);
            Vector3 center = (from.worldPosition + to.worldPosition) * 0.5f;
            float height = request.defaultHeight <= 0f ? 3f : request.defaultHeight;
            float width = Mathf.Clamp(request.corridorWidth <= 0f ? 3f : request.corridorWidth, 1.5f, 6f);
            Vector3 size;

            if (direction == "east" || direction == "west")
            {
                float gap = Mathf.Abs(to.worldPosition.x - from.worldPosition.x) - from.size.x * 0.5f - to.size.x * 0.5f;
                size = new Vector3(Mathf.Max(1f, gap), height, width);
            }
            else
            {
                float gap = Mathf.Abs(to.worldPosition.z - from.worldPosition.z) - from.size.z * 0.5f - to.size.z * 0.5f;
                size = new Vector3(width, height, Mathf.Max(1f, gap));
            }

            return new LayoutEdge
            {
                edgeId = from.nodeId + "_to_" + to.nodeId,
                fromNodeId = from.nodeId,
                toNodeId = to.nodeId,
                fromGridPosition = from.gridPosition,
                toGridPosition = to.gridPosition,
                worldPosition = center,
                size = size,
                direction = direction
            };
        }

        private static void AddOpeningsForEdge(SceneAssemblyPlan plan, LayoutNode from, LayoutNode to, LayoutEdge edge)
        {
            string fromDirection = edge.direction;
            string toDirection = OppositeDirection(fromDirection);

            plan.openings.Add(new BlockoutOpeningPlan
            {
                openingId = from.nodeId + "_to_" + to.nodeId + "_opening",
                roomId = from.nodeId,
                position = OpeningPosition(from, fromDirection),
                size = OpeningSize(fromDirection),
                directionHint = fromDirection
            });

            plan.openings.Add(new BlockoutOpeningPlan
            {
                openingId = to.nodeId + "_to_" + from.nodeId + "_opening",
                roomId = to.nodeId,
                position = OpeningPosition(to, toDirection),
                size = OpeningSize(toDirection),
                directionHint = toDirection
            });
        }

        private static Vector3 OpeningPosition(LayoutNode node, string direction)
        {
            Vector3 position = node.worldPosition;
            position.y += OpeningHeight * 0.5f;

            if (direction == "north")
            {
                position.z += node.size.z * 0.5f;
            }
            else if (direction == "south")
            {
                position.z -= node.size.z * 0.5f;
            }
            else if (direction == "east")
            {
                position.x += node.size.x * 0.5f;
            }
            else if (direction == "west")
            {
                position.x -= node.size.x * 0.5f;
            }

            return position;
        }

        private static Vector3 OpeningSize(string direction)
        {
            if (direction == "east" || direction == "west")
            {
                return new Vector3(OpeningDepth, OpeningHeight, OpeningWidth);
            }

            return new Vector3(OpeningWidth, OpeningHeight, OpeningDepth);
        }

        private static List<LandmarkProfile> BuildPlacedLandmarkList(LayoutSynthesisRequest request, List<LayoutNode> nodes)
        {
            List<LandmarkProfile> placed = new List<LandmarkProfile>();
            HashSet<string> used = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (LayoutNode node in nodes)
            {
                if (node == null || string.IsNullOrWhiteSpace(node.landmarkId) || !used.Add(node.landmarkId))
                {
                    continue;
                }

                LandmarkProfile landmark = FindLandmark(request.landmarks, node.landmarkId);
                if (landmark != null)
                {
                    placed.Add(landmark);
                }
            }

            return placed;
        }

        private static void AddLights(LayoutSynthesisRequest request, SceneAssemblyPlan plan, List<LayoutNode> nodes)
        {
            float intensity = Mathf.Clamp(0.85f + request.atmosphere.lightIntensity, 0.5f, 2.5f);
            for (int i = 0; i < nodes.Count; i++)
            {
                LayoutNode node = nodes[i];
                if (node == null || (!node.isTransitionRoom && !string.Equals(node.nodeId, "spawn_office", StringComparison.Ordinal) && !(node.isMainRoute && i % 2 == 0)))
                {
                    continue;
                }

                plan.lights.Add(new BlockoutLightPlan
                {
                    lightId = node.nodeId + "_fluorescent_bar",
                    position = new Vector3(node.worldPosition.x, node.worldPosition.y + node.size.y - 0.25f, node.worldPosition.z),
                    size = new Vector3(Mathf.Min(3f, node.size.x * 0.5f), 0.08f, 0.35f),
                    intensity = intensity,
                    lightTypeHint = "fluorescent_bar"
                });
            }
        }

        private static void AddTransition(LayoutSynthesisRequest request, SceneAssemblyPlan plan, List<LayoutNode> nodes)
        {
            if (!request.includeTransition)
            {
                return;
            }

            LayoutNode transition = null;
            foreach (LayoutNode node in nodes)
            {
                if (node != null && node.isTransitionRoom)
                {
                    transition = node;
                    break;
                }
            }

            if (transition == null)
            {
                return;
            }

            plan.transitions.Add(new BlockoutTransitionPlan
            {
                transitionId = "local_synthesized_transition",
                position = transition.worldPosition + new Vector3(0f, 1.25f, transition.size.z * 0.25f),
                size = new Vector3(3f, 2.5f, 1f),
                targetLevelId = request.levelId,
                targetPackageId = request.packageId,
                transitionType = "local_synthesized_loop"
            });
        }

        private static string DirectionFromTo(Vector2Int from, Vector2Int to)
        {
            Vector2Int delta = to - from;
            if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
            {
                return delta.x >= 0 ? "east" : "west";
            }

            return delta.y >= 0 ? "north" : "south";
        }

        private static string OppositeDirection(string direction)
        {
            if (direction == "north")
            {
                return "south";
            }

            if (direction == "south")
            {
                return "north";
            }

            if (direction == "east")
            {
                return "west";
            }

            return "east";
        }

        private static bool Contains(string[] values, string value)
        {
            if (values == null || string.IsNullOrWhiteSpace(value))
            {
                return false;
            }

            foreach (string candidate in values)
            {
                if (string.Equals(candidate, value, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        private static IEnumerable<string> SafeStrings(string[] values)
        {
            return values ?? new string[0];
        }

        private static void AddIssue(LayoutSynthesisResult result, string code, string message, bool blocker)
        {
            result.issues.Add(new LayoutSynthesisIssue
            {
                code = code,
                message = message,
                blocker = blocker
            });
        }

        private class NodePair
        {
            public readonly LayoutNode from;
            public readonly LayoutNode to;

            public NodePair(LayoutNode from, LayoutNode to)
            {
                this.from = from;
                this.to = to;
            }
        }
    }
}
