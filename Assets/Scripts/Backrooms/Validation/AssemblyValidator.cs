using System;
using System.Collections.Generic;
using Backrooms.Grammar;
using Backrooms.Landmarks;
using Backrooms.SceneAssembly;

namespace Backrooms.Validation
{
    public static class AssemblyValidator
    {
        public static AssemblyValidationReport Validate(SceneAssemblyPlan plan)
        {
            AssemblyValidationReport report = new AssemblyValidationReport();

            if (plan == null)
            {
                AddIssue(report, "plan.missing", "Scene assembly plan is missing.", true);
                FinalizeScores(report, 0f, 0f, 0f, 0f, 0f);
                return report;
            }

            ValidateIdentity(plan, report);
            ValidateGrammar(plan, report);
            ValidateAtmosphere(plan, report);
            ValidateLandmarks(plan, report);
            ValidateRoute(plan, report);

            report.identityScore = HasBlockerWithPrefix(report, "identity.") ? 0.35f : 0.92f;
            report.grammarScore = HasBlockerWithPrefix(report, "grammar.") ? 0.4f : 0.88f;
            report.atmosphereScore = HasBlockerWithPrefix(report, "atmosphere.") ? 0.35f : 0.9f;
            report.landmarkScore = HasBlockerWithPrefix(report, "landmark.") ? 0.45f : 0.86f;
            report.routeScore = HasBlockerWithPrefix(report, "route.") ? 0.35f : 0.9f;
            report.passed = !HasBlockers(report);

            return report;
        }

        private static void ValidateIdentity(SceneAssemblyPlan plan, AssemblyValidationReport report)
        {
            if (plan.identity == null)
            {
                AddIssue(report, "identity.missing", "Level identity profile is required.", true);
                return;
            }

            if (plan.grammar == null)
            {
                AddIssue(report, "identity.grammar_missing", "Room grammar profile is required for identity compatibility.", true);
            }
            else if (!string.IsNullOrWhiteSpace(plan.identity.grammarId) &&
                     !string.Equals(plan.identity.grammarId, plan.grammar.grammarId, StringComparison.Ordinal))
            {
                AddIssue(report, "identity.grammar_mismatch", "Identity grammarId does not match the attached grammar profile.", true);
            }

            foreach (BlockoutRoomPlan room in SafeRooms(plan))
            {
                if (!plan.identity.AllowsRoomType(room.roomType))
                {
                    AddIssue(report, "identity.room_forbidden", "Identity does not allow room type '" + room.roomType + "'.", true);
                }
            }
        }

        private static void ValidateGrammar(SceneAssemblyPlan plan, AssemblyValidationReport report)
        {
            if (plan.grammar == null)
            {
                AddIssue(report, "grammar.missing", "Room grammar profile is required.", true);
                return;
            }

            int roomCount = plan.rooms == null ? 0 : plan.rooms.Count;
            int connectionCount = plan.connections == null ? 0 : plan.connections.Count;

            if (roomCount < plan.grammar.minimumRooms || roomCount > plan.grammar.maximumRooms)
            {
                AddIssue(report, "grammar.room_count", "Room count is outside grammar bounds.", true);
            }

            if (connectionCount < plan.grammar.minimumConnections || connectionCount > plan.grammar.maximumConnections)
            {
                AddIssue(report, "grammar.connection_count", "Connection count is outside grammar bounds.", true);
            }

            foreach (string roomType in SafeStrings(plan.grammar.mandatoryRoomTypes))
            {
                if (!HasRoomType(plan, roomType))
                {
                    AddIssue(report, "grammar.required_room_missing", "Required room type '" + roomType + "' is missing.", true);
                }
            }

            foreach (BlockoutRoomPlan room in SafeRooms(plan))
            {
                if (Contains(plan.grammar.forbiddenRoomTypes, room.roomType))
                {
                    AddIssue(report, "grammar.forbidden_room", "Grammar forbids room type '" + room.roomType + "'.", true);
                }

                if (string.Equals(room.roomType, "dead_end", StringComparison.OrdinalIgnoreCase) && !plan.grammar.allowDeadEnds)
                {
                    AddIssue(report, "grammar.dead_end_forbidden", "Dead-end rooms are present but not allowed by grammar.", true);
                }
            }

            bool hasBranches = connectionCount > Math.Max(0, roomCount - 1);
            if (hasBranches && !plan.grammar.allowBranches)
            {
                AddIssue(report, "grammar.branch_forbidden", "Branching connections are present but not allowed by grammar.", true);
            }

            bool hasLoopLikeGraph = connectionCount >= roomCount && roomCount > 0;
            if (hasLoopLikeGraph && !plan.grammar.allowLoops)
            {
                AddIssue(report, "grammar.loop_forbidden", "Loop-like connection count is present but loops are not allowed by grammar.", true);
            }
        }

        private static void ValidateAtmosphere(SceneAssemblyPlan plan, AssemblyValidationReport report)
        {
            if (plan.atmosphere == null)
            {
                AddIssue(report, "atmosphere.missing", "Atmosphere profile is required.", true);
                return;
            }

            if (plan.identity != null &&
                !string.IsNullOrWhiteSpace(plan.identity.atmosphereId) &&
                !string.Equals(plan.identity.atmosphereId, plan.atmosphere.atmosphereId, StringComparison.Ordinal))
            {
                AddIssue(report, "atmosphere.identity_mismatch", "Atmosphere profile does not match identity atmosphereId.", true);
            }
        }

        private static void ValidateLandmarks(SceneAssemblyPlan plan, AssemblyValidationReport report)
        {
            if (plan.landmarks == null)
            {
                AddIssue(report, "landmark.list_missing", "Landmark list is missing.", true);
                return;
            }

            if (plan.identity != null)
            {
                foreach (string landmarkId in SafeStrings(plan.identity.requiredLandmarks))
                {
                    if (!HasLandmark(plan, landmarkId))
                    {
                        AddIssue(report, "landmark.required_missing", "Required landmark '" + landmarkId + "' is missing.", true);
                    }
                }

                foreach (LandmarkProfile landmark in plan.landmarks)
                {
                    if (landmark != null && plan.identity.ForbidsLandmark(landmark.landmarkId))
                    {
                        AddIssue(report, "landmark.forbidden_present", "Forbidden landmark '" + landmark.landmarkId + "' is present.", true);
                    }
                }
            }
        }

        private static void ValidateRoute(SceneAssemblyPlan plan, AssemblyValidationReport report)
        {
            if (plan.transitions == null || plan.transitions.Count == 0)
            {
                AddIssue(report, "route.transition_missing", "At least one transition is required.", true);
            }

            if (!HasRoomId(plan, "spawn_office") || !HasRoomId(plan, "long_corridor") || !HasRoomId(plan, "transition_room"))
            {
                AddIssue(report, "route.core_rooms_missing", "Core route rooms are missing.", true);
            }

            if (!HasConnection(plan, "spawn_office", "long_corridor"))
            {
                AddIssue(report, "route.spawn_to_corridor_missing", "Route must connect spawn_office to long_corridor.", true);
            }

            if (!HasRoute(plan, "long_corridor", "transition_room"))
            {
                AddIssue(report, "route.corridor_to_transition_missing", "Route must connect long_corridor to transition_room.", true);
            }
        }

        private static IEnumerable<BlockoutRoomPlan> SafeRooms(SceneAssemblyPlan plan)
        {
            return plan.rooms ?? new List<BlockoutRoomPlan>();
        }

        private static IEnumerable<string> SafeStrings(string[] values)
        {
            return values ?? new string[0];
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

        private static bool HasLandmark(SceneAssemblyPlan plan, string landmarkId)
        {
            if (plan.landmarks == null || string.IsNullOrWhiteSpace(landmarkId))
            {
                return false;
            }

            foreach (LandmarkProfile landmark in plan.landmarks)
            {
                if (landmark != null && string.Equals(landmark.landmarkId, landmarkId, StringComparison.OrdinalIgnoreCase))
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
            foreach (BlockoutRoomPlan room in SafeRooms(plan))
            {
                if (room != null && !string.IsNullOrWhiteSpace(room.roomId) && !graph.ContainsKey(room.roomId))
                {
                    graph.Add(room.roomId, new List<string>());
                }
            }

            if (!graph.ContainsKey(startRoomId) || !graph.ContainsKey(targetRoomId))
            {
                return false;
            }

            foreach (BlockoutConnectionPlan connection in plan.connections ?? new List<BlockoutConnectionPlan>())
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

        private static bool Contains(string[] values, string value)
        {
            foreach (string candidate in SafeStrings(values))
            {
                if (string.Equals(candidate, value, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        private static void AddIssue(AssemblyValidationReport report, string code, string message, bool blocker)
        {
            report.issues.Add(new AssemblyValidationIssue
            {
                code = code,
                message = message,
                blocker = blocker
            });
        }

        private static bool HasBlockers(AssemblyValidationReport report)
        {
            foreach (AssemblyValidationIssue issue in report.issues)
            {
                if (issue != null && issue.blocker)
                {
                    return true;
                }
            }

            return false;
        }

        private static bool HasBlockerWithPrefix(AssemblyValidationReport report, string prefix)
        {
            foreach (AssemblyValidationIssue issue in report.issues)
            {
                if (issue != null && issue.blocker && issue.code != null && issue.code.StartsWith(prefix, StringComparison.Ordinal))
                {
                    return true;
                }
            }

            return false;
        }

        private static void FinalizeScores(
            AssemblyValidationReport report,
            float grammarScore,
            float atmosphereScore,
            float landmarkScore,
            float routeScore,
            float identityScore)
        {
            report.grammarScore = grammarScore;
            report.atmosphereScore = atmosphereScore;
            report.landmarkScore = landmarkScore;
            report.routeScore = routeScore;
            report.identityScore = identityScore;
            report.passed = !HasBlockers(report);
        }
    }
}
