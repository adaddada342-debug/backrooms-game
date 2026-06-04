using System.Collections.Generic;
using Backrooms.SceneAssembly;
using UnityEngine;

namespace Backrooms.LayoutSynthesis.Routes
{
    public class RouteVisualizer : MonoBehaviour
    {
        public LayoutRouteAnnotation route;
        public Color routeColor = Color.yellow;
        public bool drawRoute = true;
        public List<Vector3> routePositions = new List<Vector3>();

        public void Configure(SceneAssemblyPlan plan, LayoutRouteAnnotation annotation)
        {
            route = annotation;
            routePositions.Clear();
            if (plan == null || plan.rooms == null || annotation == null || annotation.orderedRoomIds == null)
            {
                return;
            }

            foreach (string roomId in annotation.orderedRoomIds)
            {
                BlockoutRoomPlan room = FindRoom(plan, roomId);
                if (room != null)
                {
                    routePositions.Add(room.position + Vector3.up * 0.6f);
                }
            }
        }

        private void OnDrawGizmos()
        {
            if (!drawRoute || routePositions == null || routePositions.Count == 0)
            {
                return;
            }

            UnityEngine.Gizmos.color = routeColor;
            for (int i = 0; i < routePositions.Count; i++)
            {
                UnityEngine.Gizmos.DrawSphere(routePositions[i], 0.35f);
                if (i > 0)
                {
                    UnityEngine.Gizmos.DrawLine(routePositions[i - 1], routePositions[i]);
                }
            }
        }

        private static BlockoutRoomPlan FindRoom(SceneAssemblyPlan plan, string roomId)
        {
            foreach (BlockoutRoomPlan room in plan.rooms)
            {
                if (room != null && room.roomId == roomId)
                {
                    return room;
                }
            }

            return null;
        }
    }
}
