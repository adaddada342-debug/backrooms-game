using System;
using System.Collections.Generic;
using Backrooms.Grammar;
using Backrooms.Landmarks;
using Backrooms.LayoutSynthesis.Models;
using Backrooms.SceneAssembly;

namespace Backrooms.LayoutSynthesis.Validation
{
    public static class LayoutSynthesisValidator
    {
        public static void Validate(LayoutSynthesisRequest request, LayoutSynthesisResult result)
        {
            if (result == null)
            {
                return;
            }

            if (request == null)
            {
                AddIssue(result, "request.missing", "Layout synthesis request is missing.", true);
                result.succeeded = false;
                return;
            }

            if (!request.HasRequiredProfiles())
            {
                AddIssue(result, "request.profiles_missing", "Layout synthesis request is missing one or more required profiles.", true);
            }

            if (!request.HasValidCounts())
            {
                AddIssue(result, "request.counts_invalid", "Layout synthesis request counts are invalid or outside grammar bounds.", true);
            }

            if (result.plan == null)
            {
                AddIssue(result, "plan.missing", "Synthesizer did not produce a SceneAssemblyPlan.", true);
                result.succeeded = false;
                return;
            }

            ValidateRooms(request, result);
            ValidateConnections(request, result);
            ValidateOpenings(result);
            ValidateLandmarks(request, result);
            ValidateRoute(result);

            result.succeeded = !result.HasBlockers();
        }

        private static void ValidateRooms(LayoutSynthesisRequest request, LayoutSynthesisResult result)
        {
            SceneAssemblyPlan plan = result.plan;
            HashSet<string> roomIds = new HashSet<string>(StringComparer.Ordinal);
            HashSet<string> positions = new HashSet<string>(StringComparer.Ordinal);

            int roomCount = plan.rooms == null ? 0 : plan.rooms.Count;
            if (request.grammar != null &&
                (roomCount < request.grammar.minimumRooms || roomCount > request.grammar.maximumRooms))
            {
                AddIssue(result, "rooms.count_outside_grammar", "Generated room count is outside grammar bounds.", true);
            }

            foreach (BlockoutRoomPlan room in SafeRooms(plan))
            {
                if (room == null)
                {
                    AddIssue(result, "rooms.null", "Generated room list contains a null room.", true);
                    continue;
                }

                if (string.IsNullOrWhiteSpace(room.roomId) || !roomIds.Add(room.roomId))
                {
                    AddIssue(result, "rooms.duplicate_id", "Generated room IDs must be unique and non-empty.", true);
                }

                string positionKey = PositionKey(room);
                if (!positions.Add(positionKey))
                {
                    AddIssue(result, "rooms.duplicate_position", "Generated rooms overlap the same synthesized grid/world position.", true);
                }

                if (request.identity != null && !request.identity.AllowsRoomType(room.roomType))
                {
                    AddIssue(result, "rooms.identity_forbidden_type", "Room type '" + room.roomType + "' is not allowed by level identity.", true);
                }
            }

            if (request.grammar != null)
            {
                foreach (string roomType in SafeStrings(request.grammar.mandatoryRoomTypes))
                {
                    if (!HasRoomType(plan, roomType))
                    {
                        AddIssue(result, "rooms.mandatory_type_missing", "Mandatory grammar room type '" + roomType + "' is missing.", true);
                    }
                }
            }
        }

        private static void ValidateConnections(LayoutSynthesisRequest request, LayoutSynthesisResult result)
        {
            SceneAssemblyPlan plan = result.plan;
            int connectionCount = plan.connections == null ? 0 : plan.connections.Count;
            if (request.grammar != null &&
                (connectionCount < request.grammar.minimumConnections || connectionCount > request.grammar.maximumConnections))
            {
                AddIssue(result, "connections.count_outside_grammar", "Generated connection count is outside grammar bounds.", true);
            }

            HashSet<string> roomIds = BuildRoomIdSet(plan);
            foreach (BlockoutConnectionPlan connection in SafeConnections(plan))
            {
                if (connection == null)
                {
                    AddIssue(result, "connections.null", "Generated connection list contains a null connection.", true);
                    continue;
                }

                if (!roomIds.Contains(connection.fromRoomId) || !roomIds.Contains(connection.toRoomId))
                {
                    AddIssue(result, "connections.room_missing", "Connection references a room that does not exist.", true);
                }
            }
        }

        private static void ValidateOpenings(LayoutSynthesisResult result)
        {
            HashSet<string> roomIds = BuildRoomIdSet(result.plan);
            foreach (BlockoutOpeningPlan opening in SafeOpenings(result.plan))
            {
                if (opening == null)
                {
                    AddIssue(result, "openings.null", "Generated opening list contains a null opening.", true);
                    continue;
                }

                if (!roomIds.Contains(opening.roomId))
                {
                    AddIssue(result, "openings.room_missing", "Opening references a room that does not exist.", true);
                }
            }
        }

        private static void ValidateLandmarks(LayoutSynthesisRequest request, LayoutSynthesisResult result)
        {
            SceneAssemblyPlan plan = result.plan;
            if (request.includeLandmarks && plan.landmarks == null)
            {
                AddIssue(result, "landmarks.list_missing", "Landmark placement was requested, but no landmark list exists.", true);
                return;
            }

            if (request.identity == null)
            {
                return;
            }

            foreach (string requiredLandmark in SafeStrings(request.identity.requiredLandmarks))
            {
                if (!HasLandmark(plan, requiredLandmark))
                {
                    AddIssue(result, "landmarks.required_missing", "Required landmark '" + requiredLandmark + "' is missing.", true);
                }
            }

            foreach (LandmarkProfile landmark in SafeLandmarks(plan))
            {
                if (landmark != null && request.identity.ForbidsLandmark(landmark.landmarkId))
                {
                    AddIssue(result, "landmarks.forbidden_present", "Forbidden landmark '" + landmark.landmarkId + "' is present.", true);
                }
            }
        }

        private static void ValidateRoute(LayoutSynthesisResult result)
        {
            SceneAssemblyPlan plan = result.plan;
            if (plan.transitions == null || plan.transitions.Count == 0)
            {
                AddIssue(result, "route.transition_missing", "A transition is required for the synthesized Level 0 loop.", true);
            }

            if (!HasRoomId(plan, "spawn_office") || !HasRoomId(plan, "transition_room"))
            {
                AddIssue(result, "route.endpoints_missing", "Route endpoints spawn_office and transition_room are required.", true);
                return;
            }

            if (!HasRoute(plan, "spawn_office", "transition_room"))
            {
                AddIssue(result, "route.path_missing", "No connected route exists from spawn_office to transition_room.", true);
            }
        }

        private static IEnumerable<BlockoutRoomPlan> SafeRooms(SceneAssemblyPlan plan)
        {
            return plan.rooms ?? new List<BlockoutRoomPlan>();
        }

        private static IEnumerable<BlockoutConnectionPlan> SafeConnections(SceneAssemblyPlan plan)
        {
            return plan.connections ?? new List<BlockoutConnectionPlan>();
        }

        private static IEnumerable<BlockoutOpeningPlan> SafeOpenings(SceneAssemblyPlan plan)
        {
            return plan.openings ?? new List<BlockoutOpeningPlan>();
        }

        private static IEnumerable<LandmarkProfile> SafeLandmarks(SceneAssemblyPlan plan)
        {
            return plan.landmarks ?? new List<LandmarkProfile>();
        }

        private static IEnumerable<string> SafeStrings(string[] values)
        {
            return values ?? new string[0];
        }

        private static HashSet<string> BuildRoomIdSet(SceneAssemblyPlan plan)
        {
            HashSet<string> roomIds = new HashSet<string>(StringComparer.Ordinal);
            foreach (BlockoutRoomPlan room in SafeRooms(plan))
            {
                if (room != null && !string.IsNullOrWhiteSpace(room.roomId))
                {
                    roomIds.Add(room.roomId);
                }
            }

            return roomIds;
        }

        private static bool HasRoomId(SceneAssemblyPlan plan, string roomId)
        {
            foreach (BlockoutRoomPlan room in SafeRooms(plan))
            {
                if (room != null && string.Equals(room.roomId, roomId, StringComparison.Ordinal))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool HasRoomType(SceneAssemblyPlan plan, string roomType)
        {
            foreach (BlockoutRoomPlan room in SafeRooms(plan))
            {
                if (room != null && string.Equals(room.roomType, roomType, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool HasLandmark(SceneAssemblyPlan plan, string landmarkId)
        {
            foreach (LandmarkProfile landmark in SafeLandmarks(plan))
            {
                if (landmark != null && string.Equals(landmark.landmarkId, landmarkId, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool HasRoute(SceneAssemblyPlan plan, string startRoomId, string targetRoomId)
        {
            Dictionary<string, List<string>> graph = new Dictionary<string, List<string>>(StringComparer.Ordinal);
            foreach (BlockoutRoomPlan room in SafeRooms(plan))
            {
                if (room != null && !string.IsNullOrWhiteSpace(room.roomId) && !graph.ContainsKey(room.roomId))
                {
                    graph.Add(room.roomId, new List<string>());
                }
            }

            foreach (BlockoutConnectionPlan connection in SafeConnections(plan))
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

            if (!graph.ContainsKey(startRoomId) || !graph.ContainsKey(targetRoomId))
            {
                return false;
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

        private static string PositionKey(BlockoutRoomPlan room)
        {
            return room.position.x.ToString("0.###") + "|" +
                   room.position.y.ToString("0.###") + "|" +
                   room.position.z.ToString("0.###");
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
    }
}
