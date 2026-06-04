using Backrooms.SceneAssembly;
using UnityEngine;

namespace Backrooms.Soundscape
{
    public static class Level0SoundscapeFactory
    {
        public static SoundscapeProfile CreateDefaultProfile()
        {
            return new SoundscapeProfile
            {
                soundscapeId = "level0_placeholder_soundscape",
                displayName = "Level 0 Placeholder Soundscape",
                description = "Local placeholder soundscape plan for fluorescent hum, stale air, distant rumble, and silence pockets.",
                masterVolume = 0.65f,
                humVolume = 0.7f,
                airVolume = 0.35f,
                distantRumbleVolume = 0.22f,
                silenceChance = 0.18f,
                reverbIntensity = 0.35f,
                lowPassAmount = 0.18f,
                useProceduralPlaceholders = false,
                soundTags = new[] { "fluorescent_hum", "stale_air", "distant_rumble", "silence_pocket" }
            };
        }

        public static SoundscapePlan CreatePlan(SceneAssemblyPlan scenePlan)
        {
            SoundscapeProfile profile = CreateDefaultProfile();
            SoundscapePlan plan = new SoundscapePlan
            {
                planId = scenePlan == null ? "level0_soundscape_plan" : scenePlan.planId + "_soundscape",
                packageId = scenePlan == null ? string.Empty : scenePlan.packageId,
                levelId = scenePlan == null ? string.Empty : scenePlan.levelId,
                profile = profile
            };

            if (scenePlan == null || scenePlan.rooms == null)
            {
                return plan;
            }

            float atmosphereVolume = scenePlan.atmosphere == null ? 1f : Mathf.Clamp01(scenePlan.atmosphere.audioVolume);
            BlockoutRoomPlan spawn = FindRoom(scenePlan, "spawn_office");
            if (spawn != null)
            {
                AddEmitter(plan, "global_fluorescent_hum", spawn.roomId, spawn.position + Vector3.up * 2f, profile.humVolume * atmosphereVolume, "fluorescent_hum", "Global placeholder hum near spawn.");
            }

            int corridorIndex = 0;
            foreach (BlockoutRoomPlan room in scenePlan.rooms)
            {
                if (room == null)
                {
                    continue;
                }

                if ((room.roomType == "corridor" || (room.roomId != null && room.roomId.StartsWith("main_route_"))) && corridorIndex % 2 == 0)
                {
                    AddEmitter(plan, "corridor_hum_" + corridorIndex, room.roomId, room.position + Vector3.up * 2f, profile.humVolume * 0.65f * atmosphereVolume, "fluorescent_hum", "Corridor placeholder hum.");
                }

                if (room.roomType == "dead_end")
                {
                    AddEmitter(plan, "silence_pocket_" + room.roomId, room.roomId, room.position + Vector3.up * 1.4f, 0f, "silence_pocket", "Placeholder silence pocket marker.");
                }

                corridorIndex++;
            }

            BlockoutRoomPlan transition = FindRoom(scenePlan, "transition_room");
            if (transition != null)
            {
                AddEmitter(plan, "transition_distant_rumble", transition.roomId, transition.position + Vector3.up * 1.8f, profile.distantRumbleVolume * atmosphereVolume, "distant_rumble", "Faint placeholder rumble near transition room.");
            }

            return plan;
        }

        private static BlockoutRoomPlan FindRoom(SceneAssemblyPlan scenePlan, string roomId)
        {
            if (scenePlan.rooms == null)
            {
                return null;
            }

            foreach (BlockoutRoomPlan room in scenePlan.rooms)
            {
                if (room != null && room.roomId == roomId)
                {
                    return room;
                }
            }

            return null;
        }

        private static void AddEmitter(SoundscapePlan plan, string emitterId, string roomId, Vector3 position, float volume, string soundTag, string notes)
        {
            plan.emitters.Add(new SoundscapeEmitterPlan
            {
                emitterId = emitterId,
                roomId = roomId,
                position = position,
                volume = Mathf.Clamp01(volume),
                minDistance = 2f,
                maxDistance = 16f,
                loop = true,
                soundTag = soundTag,
                notes = notes
            });
        }
    }
}
