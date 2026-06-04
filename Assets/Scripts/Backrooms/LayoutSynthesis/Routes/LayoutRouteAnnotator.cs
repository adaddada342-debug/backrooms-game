using System;
using System.Collections.Generic;
using Backrooms.SceneAssembly;
using UnityEngine;

namespace Backrooms.LayoutSynthesis.Routes
{
    public static class LayoutRouteAnnotator
    {
        public static LayoutRouteAnnotation CreateMainRoute(SceneAssemblyPlan plan)
        {
            LayoutRouteAnnotation annotation = new LayoutRouteAnnotation
            {
                routeId = "level0_main_route",
                packageId = plan == null ? string.Empty : plan.packageId,
                levelId = plan == null ? string.Empty : plan.levelId,
                startRoomId = "spawn_office",
                endRoomId = "transition_room",
                notes = "Wave 9 graph-based route annotation."
            };

            if (plan == null || plan.rooms == null || plan.connections == null)
            {
                annotation.notes = "Plan, rooms, or connections were missing.";
                return annotation;
            }

            Dictionary<string, List<EdgeInfo>> graph = BuildGraph(plan);
            List<string> route = FindRoute(graph, annotation.startRoomId, annotation.endRoomId);
            annotation.orderedRoomIds = route;
            annotation.routeLength = route.Count;
            annotation.reachesTransition = route.Count > 0 && route[route.Count - 1] == annotation.endRoomId;
            annotation.connectionIds = FindConnectionIdsForRoute(route, plan);
            annotation.routeComplexity = CalculateComplexity(plan, route);
            return annotation;
        }

        private static Dictionary<string, List<EdgeInfo>> BuildGraph(SceneAssemblyPlan plan)
        {
            Dictionary<string, List<EdgeInfo>> graph = new Dictionary<string, List<EdgeInfo>>(StringComparer.Ordinal);
            foreach (BlockoutRoomPlan room in plan.rooms)
            {
                if (room != null && !string.IsNullOrWhiteSpace(room.roomId) && !graph.ContainsKey(room.roomId))
                {
                    graph.Add(room.roomId, new List<EdgeInfo>());
                }
            }

            foreach (BlockoutConnectionPlan connection in plan.connections)
            {
                if (connection == null ||
                    !graph.ContainsKey(connection.fromRoomId) ||
                    !graph.ContainsKey(connection.toRoomId))
                {
                    continue;
                }

                graph[connection.fromRoomId].Add(new EdgeInfo(connection.toRoomId, connection.connectionId));
                graph[connection.toRoomId].Add(new EdgeInfo(connection.fromRoomId, connection.connectionId));
            }

            return graph;
        }

        private static List<string> FindRoute(Dictionary<string, List<EdgeInfo>> graph, string start, string end)
        {
            List<string> empty = new List<string>();
            if (!graph.ContainsKey(start) || !graph.ContainsKey(end))
            {
                return empty;
            }

            Queue<string> queue = new Queue<string>();
            Dictionary<string, string> previous = new Dictionary<string, string>(StringComparer.Ordinal);
            HashSet<string> visited = new HashSet<string>(StringComparer.Ordinal);
            queue.Enqueue(start);
            visited.Add(start);

            while (queue.Count > 0)
            {
                string current = queue.Dequeue();
                if (current == end)
                {
                    break;
                }

                foreach (EdgeInfo edge in graph[current])
                {
                    if (visited.Add(edge.toRoomId))
                    {
                        previous[edge.toRoomId] = current;
                        queue.Enqueue(edge.toRoomId);
                    }
                }
            }

            if (!visited.Contains(end))
            {
                return empty;
            }

            List<string> route = new List<string>();
            string cursor = end;
            route.Add(cursor);
            while (previous.ContainsKey(cursor))
            {
                cursor = previous[cursor];
                route.Add(cursor);
            }

            route.Reverse();
            return route;
        }

        private static List<string> FindConnectionIdsForRoute(List<string> route, SceneAssemblyPlan plan)
        {
            List<string> ids = new List<string>();
            if (route == null || route.Count < 2)
            {
                return ids;
            }

            for (int i = 1; i < route.Count; i++)
            {
                string id = FindConnectionId(plan, route[i - 1], route[i]);
                if (!string.IsNullOrWhiteSpace(id))
                {
                    ids.Add(id);
                }
            }

            return ids;
        }

        private static string FindConnectionId(SceneAssemblyPlan plan, string first, string second)
        {
            foreach (BlockoutConnectionPlan connection in plan.connections)
            {
                if (connection == null)
                {
                    continue;
                }

                bool forward = connection.fromRoomId == first && connection.toRoomId == second;
                bool reverse = connection.fromRoomId == second && connection.toRoomId == first;
                if (forward || reverse)
                {
                    return connection.connectionId;
                }
            }

            return string.Empty;
        }

        private static float CalculateComplexity(SceneAssemblyPlan plan, List<string> route)
        {
            int routeLength = route == null ? 0 : route.Count;
            int connectionCount = plan.connections == null ? 0 : plan.connections.Count;
            int roomCount = plan.rooms == null ? 0 : plan.rooms.Count;
            int branchEstimate = Mathf.Max(0, connectionCount - Mathf.Max(0, roomCount - 1));
            return Mathf.Clamp01(routeLength / 14f + branchEstimate * 0.08f);
        }

        private struct EdgeInfo
        {
            public readonly string toRoomId;
            public readonly string connectionId;

            public EdgeInfo(string toRoomId, string connectionId)
            {
                this.toRoomId = toRoomId;
                this.connectionId = connectionId;
            }
        }
    }
}
