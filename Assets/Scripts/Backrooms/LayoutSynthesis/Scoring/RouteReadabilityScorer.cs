using System;
using System.Collections.Generic;
using Backrooms.Landmarks;
using Backrooms.SceneAssembly;
using UnityEngine;

namespace Backrooms.LayoutSynthesis.Scoring
{
    public static class RouteReadabilityScorer
    {
        public static RouteReadabilityReport Score(SceneAssemblyPlan plan)
        {
            RouteReadabilityReport report = new RouteReadabilityReport();
            if (plan == null)
            {
                AddIssue(report, "plan.missing", "Cannot score readability without a SceneAssemblyPlan.", true);
                FinalizeScores(report);
                return report;
            }

            report.mainRouteRoomCount = CountMainRouteRooms(plan);
            report.branchCount = CountBranches(plan);
            report.deadEndCount = CountDeadEnds(plan);
            report.landmarkCount = plan.landmarks == null ? 0 : plan.landmarks.Count;
            report.requiredLandmarkCount = plan.identity == null || plan.identity.requiredLandmarks == null ? 0 : plan.identity.requiredLandmarks.Length;
            report.missingRequiredLandmarkCount = CountMissingRequiredLandmarks(plan);
            report.transitionCount = plan.transitions == null ? 0 : plan.transitions.Count;

            bool hasSpawn = HasRoomId(plan, "spawn_office");
            bool hasTransitionRoom = HasRoomId(plan, "transition_room");
            bool hasRoute = hasSpawn && hasTransitionRoom && HasRoute(plan, "spawn_office", "transition_room");

            if (!hasSpawn)
            {
                AddIssue(report, "route.spawn_missing", "Readability failed because spawn_office is missing.", true);
            }

            if (!hasTransitionRoom)
            {
                AddIssue(report, "route.transition_room_missing", "Readability failed because transition_room is missing.", true);
            }

            if (!hasRoute)
            {
                AddIssue(report, "route.path_missing", "Readability failed because no route exists from spawn_office to transition_room.", true);
            }

            if (report.transitionCount == 0)
            {
                AddIssue(report, "transition.missing", "Readability failed because no transition trigger exists.", true);
            }

            report.routeDirectnessScore = ScoreRouteDirectness(hasRoute, hasSpawn, hasTransitionRoom, report.mainRouteRoomCount);
            report.branchClarityScore = ScoreBranchClarity(report.branchCount);
            report.landmarkSupportScore = ScoreLandmarkSupport(report.landmarkCount, report.requiredLandmarkCount, report.missingRequiredLandmarkCount);
            report.deadEndPenaltyScore = ScoreDeadEnds(report.deadEndCount);
            report.transitionFindabilityScore = ScoreTransitions(report.transitionCount);
            FinalizeScores(report);

            if (report.totalScore < 0.65f)
            {
                AddIssue(report, "readability.low_score", "Route readability score is below the Wave 7 target threshold.", false);
                FinalizeScores(report);
            }

            return report;
        }

        private static float ScoreRouteDirectness(bool hasRoute, bool hasSpawn, bool hasTransitionRoom, int mainRouteRoomCount)
        {
            float score = hasRoute ? 0.55f : 0f;
            if (hasSpawn)
            {
                score += 0.1f;
            }

            if (hasTransitionRoom)
            {
                score += 0.1f;
            }

            if (mainRouteRoomCount >= 4 && mainRouteRoomCount <= 10)
            {
                score += 0.25f;
            }
            else if (mainRouteRoomCount > 0)
            {
                score += 0.1f;
            }

            return Mathf.Clamp01(score);
        }

        private static float ScoreBranchClarity(int branchCount)
        {
            if (branchCount >= 1 && branchCount <= 4)
            {
                return 1f;
            }

            if (branchCount == 0)
            {
                return 0.75f;
            }

            return Mathf.Clamp01(1f - (branchCount - 4) * 0.18f);
        }

        private static float ScoreLandmarkSupport(int landmarkCount, int requiredLandmarkCount, int missingRequiredLandmarkCount)
        {
            if (requiredLandmarkCount > 0 && missingRequiredLandmarkCount >= requiredLandmarkCount)
            {
                return 0.1f;
            }

            float score = landmarkCount > 0 ? 0.35f : 0f;
            if (requiredLandmarkCount == 0)
            {
                score += 0.45f;
            }
            else
            {
                score += 0.65f * (1f - (float)missingRequiredLandmarkCount / requiredLandmarkCount);
            }

            return Mathf.Clamp01(score);
        }

        private static float ScoreDeadEnds(int deadEndCount)
        {
            if (deadEndCount <= 3)
            {
                return 1f;
            }

            return Mathf.Clamp01(1f - (deadEndCount - 3) * 0.2f);
        }

        private static float ScoreTransitions(int transitionCount)
        {
            if (transitionCount == 1)
            {
                return 1f;
            }

            if (transitionCount == 0)
            {
                return 0f;
            }

            return Mathf.Clamp01(0.8f - (transitionCount - 2) * 0.15f);
        }

        private static int CountMainRouteRooms(SceneAssemblyPlan plan)
        {
            int count = 0;
            foreach (BlockoutRoomPlan room in SafeRooms(plan))
            {
                if (room == null)
                {
                    continue;
                }

                if (string.Equals(room.roomId, "spawn_office", StringComparison.Ordinal) ||
                    string.Equals(room.roomId, "long_corridor", StringComparison.Ordinal) ||
                    string.Equals(room.roomId, "transition_room", StringComparison.Ordinal) ||
                    (room.roomId != null && room.roomId.StartsWith("main_route_", StringComparison.Ordinal)))
                {
                    count++;
                }
            }

            return count;
        }

        private static int CountBranches(SceneAssemblyPlan plan)
        {
            int count = 0;
            foreach (BlockoutRoomPlan room in SafeRooms(plan))
            {
                if (room != null && room.roomId != null && room.roomId.StartsWith("branch_", StringComparison.Ordinal))
                {
                    count++;
                }
            }

            return count;
        }

        private static int CountDeadEnds(SceneAssemblyPlan plan)
        {
            int count = 0;
            foreach (BlockoutRoomPlan room in SafeRooms(plan))
            {
                if (room == null)
                {
                    continue;
                }

                if (string.Equals(room.roomType, "dead_end", StringComparison.OrdinalIgnoreCase) ||
                    (room.roomId != null && room.roomId.IndexOf("dead_end", StringComparison.OrdinalIgnoreCase) >= 0))
                {
                    count++;
                }
            }

            return count;
        }

        private static int CountMissingRequiredLandmarks(SceneAssemblyPlan plan)
        {
            if (plan.identity == null || plan.identity.requiredLandmarks == null)
            {
                return 0;
            }

            int missing = 0;
            foreach (string requiredId in plan.identity.requiredLandmarks)
            {
                if (!HasLandmark(plan, requiredId))
                {
                    missing++;
                }
            }

            return missing;
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

        private static IEnumerable<BlockoutRoomPlan> SafeRooms(SceneAssemblyPlan plan)
        {
            return plan.rooms ?? new List<BlockoutRoomPlan>();
        }

        private static void FinalizeScores(RouteReadabilityReport report)
        {
            report.totalScore = Mathf.Clamp01((
                report.routeDirectnessScore +
                report.branchClarityScore +
                report.landmarkSupportScore +
                report.deadEndPenaltyScore +
                report.transitionFindabilityScore) / 5f);
            report.passed = report.totalScore >= 0.65f && !report.HasBlockers();
        }

        private static void AddIssue(RouteReadabilityReport report, string code, string message, bool blocker)
        {
            report.issues.Add(new RouteReadabilityIssue
            {
                code = code,
                message = message,
                blocker = blocker
            });
        }
    }
}
