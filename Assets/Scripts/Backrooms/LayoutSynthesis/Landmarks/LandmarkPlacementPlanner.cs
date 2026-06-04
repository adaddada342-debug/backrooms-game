using System;
using System.Collections.Generic;
using Backrooms.Landmarks;
using Backrooms.SceneAssembly;
using UnityEngine;

namespace Backrooms.LayoutSynthesis.Landmarks
{
    public static class LandmarkPlacementPlanner
    {
        public static LandmarkPlacementPlan CreatePlacementPlan(SceneAssemblyPlan plan)
        {
            LandmarkPlacementPlan placementPlan = new LandmarkPlacementPlan
            {
                planId = plan == null ? "landmark_placement_missing_plan" : plan.planId + "_landmark_placements",
                packageId = plan == null ? string.Empty : plan.packageId,
                levelId = plan == null ? string.Empty : plan.levelId
            };

            if (plan == null || plan.landmarks == null || plan.rooms == null)
            {
                return placementPlan;
            }

            HashSet<string> placed = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            int index = 0;
            if (plan.identity != null && plan.identity.requiredLandmarks != null)
            {
                foreach (string requiredId in plan.identity.requiredLandmarks)
                {
                    LandmarkProfile profile = FindLandmark(plan, requiredId);
                    if (profile != null && placed.Add(profile.landmarkId))
                    {
                        placementPlan.placements.Add(CreatePlacement(plan, profile, true, index++));
                    }
                }
            }

            foreach (LandmarkProfile profile in plan.landmarks)
            {
                if (profile != null && placed.Add(profile.landmarkId))
                {
                    placementPlan.placements.Add(CreatePlacement(plan, profile, false, index++));
                }
            }

            return placementPlan;
        }

        private static LandmarkPlacement CreatePlacement(SceneAssemblyPlan plan, LandmarkProfile profile, bool required, int index)
        {
            BlockoutRoomPlan room = PickRoom(plan, profile, required);
            Vector3 position = room == null ? Vector3.zero : OffsetPosition(room, index);
            return new LandmarkPlacement
            {
                placementId = "placement_" + profile.landmarkId,
                landmarkId = profile.landmarkId,
                roomId = room == null ? string.Empty : room.roomId,
                position = position,
                placementReason = BuildReason(profile, room, required),
                required = required || profile.requiredForLevelIdentity,
                importance = profile.importance
            };
        }

        private static BlockoutRoomPlan PickRoom(SceneAssemblyPlan plan, LandmarkProfile profile, bool required)
        {
            BlockoutRoomPlan fallback = null;
            foreach (BlockoutRoomPlan room in plan.rooms)
            {
                if (room == null)
                {
                    continue;
                }

                if (fallback == null)
                {
                    fallback = room;
                }

                if (required && IsMainRouteRoom(room) && CanPlace(profile, room.roomType))
                {
                    return room;
                }

                if (!required && IsBranchOrDeadEnd(room) && CanPlace(profile, room.roomType))
                {
                    return room;
                }
            }

            foreach (BlockoutRoomPlan room in plan.rooms)
            {
                if (room != null && CanPlace(profile, room.roomType))
                {
                    return room;
                }
            }

            return fallback;
        }

        private static bool IsMainRouteRoom(BlockoutRoomPlan room)
        {
            return room.roomId == "spawn_office" ||
                   room.roomId == "long_corridor" ||
                   room.roomId == "transition_room" ||
                   (room.roomId != null && room.roomId.StartsWith("main_route_", StringComparison.Ordinal));
        }

        private static bool IsBranchOrDeadEnd(BlockoutRoomPlan room)
        {
            return room.roomType == "dead_end" ||
                   (room.roomId != null && room.roomId.StartsWith("branch_", StringComparison.Ordinal));
        }

        private static bool CanPlace(LandmarkProfile profile, string roomType)
        {
            if (profile == null)
            {
                return false;
            }

            if (Contains(profile.forbiddenRoomTypes, roomType))
            {
                return false;
            }

            return profile.allowedRoomTypes == null ||
                   profile.allowedRoomTypes.Length == 0 ||
                   Contains(profile.allowedRoomTypes, roomType);
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

        private static Vector3 OffsetPosition(BlockoutRoomPlan room, int index)
        {
            float x = ((index % 3) - 1) * Mathf.Min(1.3f, room.size.x * 0.18f);
            float z = (((index / 3) % 3) - 1) * Mathf.Min(1.3f, room.size.z * 0.18f);
            return new Vector3(room.position.x + x, room.position.y + 0.35f, room.position.z + z);
        }

        private static string BuildReason(LandmarkProfile profile, BlockoutRoomPlan room, bool required)
        {
            if (room == null)
            {
                return "No room was available.";
            }

            if (!CanPlace(profile, room.roomType))
            {
                return "Fallback placement because no matching allowed room type was available.";
            }

            return required ? "Required landmark placed on a main route room." : "Optional landmark placed on branch/dead-end when possible.";
        }

        private static LandmarkProfile FindLandmark(SceneAssemblyPlan plan, string landmarkId)
        {
            foreach (LandmarkProfile landmark in plan.landmarks)
            {
                if (landmark != null && string.Equals(landmark.landmarkId, landmarkId, StringComparison.OrdinalIgnoreCase))
                {
                    return landmark;
                }
            }

            return null;
        }
    }
}
