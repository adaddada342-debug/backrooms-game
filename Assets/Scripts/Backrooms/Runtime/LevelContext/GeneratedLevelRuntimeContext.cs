using Backrooms.LayoutSynthesis.Landmarks;
using Backrooms.LayoutSynthesis.Preview;
using Backrooms.LayoutSynthesis.Routes;
using Backrooms.SceneAssembly;
using UnityEngine;

namespace Backrooms.Runtime.LevelContext
{
    public class GeneratedLevelRuntimeContext : MonoBehaviour
    {
        public string packageId;
        public string levelId;
        public int seed;
        public string sceneName;
        public SceneAssemblyPlan plan;
        public LayoutRouteAnnotation routeAnnotation;
        public LandmarkPlacementPlan landmarkPlacementPlan;
        public LayoutPreviewSummary previewSummary;

        public void Configure(
            SceneAssemblyPlan newPlan,
            LayoutRouteAnnotation newRouteAnnotation,
            LandmarkPlacementPlan newLandmarkPlacementPlan,
            LayoutPreviewSummary newPreviewSummary)
        {
            plan = newPlan;
            routeAnnotation = newRouteAnnotation;
            landmarkPlacementPlan = newLandmarkPlacementPlan;
            previewSummary = newPreviewSummary;
            packageId = newPlan == null ? string.Empty : newPlan.packageId;
            levelId = newPlan == null ? string.Empty : newPlan.levelId;
            seed = newPlan == null ? 0 : newPlan.seed;
            sceneName = newPlan == null ? string.Empty : newPlan.sceneName;
        }

        public BlockoutRoomPlan FindRoomById(string roomId)
        {
            if (plan == null || plan.rooms == null || string.IsNullOrWhiteSpace(roomId))
            {
                return null;
            }

            foreach (BlockoutRoomPlan room in plan.rooms)
            {
                if (room != null && room.roomId == roomId)
                {
                    return room;
                }
            }

            return null;
        }

        public BlockoutRoomPlan FindNearestRoom(Vector3 worldPosition)
        {
            if (plan == null || plan.rooms == null || plan.rooms.Count == 0)
            {
                return null;
            }

            BlockoutRoomPlan nearest = null;
            float bestDistance = float.MaxValue;
            foreach (BlockoutRoomPlan room in plan.rooms)
            {
                if (room == null)
                {
                    continue;
                }

                float distance = Vector3.SqrMagnitude(room.position - worldPosition);
                if (distance < bestDistance)
                {
                    bestDistance = distance;
                    nearest = room;
                }
            }

            return nearest;
        }

        public bool HasValidPlan()
        {
            return plan != null && plan.rooms != null && plan.rooms.Count > 0;
        }
    }
}
