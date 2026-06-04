using Backrooms.SceneAssembly;
using UnityEngine;

namespace Backrooms.Mapping.UI
{
    public static class WorldToMapProjector
    {
        public static Vector2 Project(Vector3 worldPosition, Bounds worldBounds, Rect mapRect, float padding)
        {
            if (worldBounds.size.x <= 0.001f || worldBounds.size.z <= 0.001f)
            {
                return mapRect.center;
            }

            float x = Mathf.InverseLerp(worldBounds.min.x, worldBounds.max.x, worldPosition.x);
            float y = Mathf.InverseLerp(worldBounds.min.z, worldBounds.max.z, worldPosition.z);
            float width = Mathf.Max(1f, mapRect.width - padding * 2f);
            float height = Mathf.Max(1f, mapRect.height - padding * 2f);
            return new Vector2(mapRect.xMin + padding + x * width, mapRect.yMin + padding + y * height);
        }

        public static Bounds CalculateBounds(SceneAssemblyPlan plan)
        {
            if (plan == null || plan.rooms == null || plan.rooms.Count == 0)
            {
                return new Bounds(Vector3.zero, new Vector3(10f, 1f, 10f));
            }

            bool initialized = false;
            Bounds bounds = new Bounds(Vector3.zero, Vector3.zero);
            foreach (BlockoutRoomPlan room in plan.rooms)
            {
                if (room == null)
                {
                    continue;
                }

                Bounds roomBounds = new Bounds(room.position, room.size);
                if (!initialized)
                {
                    bounds = roomBounds;
                    initialized = true;
                }
                else
                {
                    bounds.Encapsulate(roomBounds);
                }
            }

            return initialized ? bounds : new Bounds(Vector3.zero, new Vector3(10f, 1f, 10f));
        }
    }
}
