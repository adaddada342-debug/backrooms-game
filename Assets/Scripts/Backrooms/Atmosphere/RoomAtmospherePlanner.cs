using System.Collections.Generic;
using Backrooms.SceneAssembly;
using UnityEngine;

namespace Backrooms.Atmosphere
{
    public static class RoomAtmospherePlanner
    {
        public static List<RoomAtmosphereTag> CreateRoomTags(SceneAssemblyPlan plan)
        {
            List<RoomAtmosphereTag> tags = new List<RoomAtmosphereTag>();
            if (plan == null || plan.rooms == null)
            {
                return tags;
            }

            AtmosphereProfile atmosphere = plan.atmosphere;
            foreach (BlockoutRoomPlan room in plan.rooms)
            {
                if (room == null)
                {
                    continue;
                }

                float hum = atmosphere == null ? 0.5f : atmosphere.humIntensity;
                float flicker = atmosphere == null ? 0.05f : atmosphere.flickerChance;
                float silence = atmosphere == null ? 0.1f : atmosphere.silenceChance;
                float pressure = atmosphere == null ? 0.5f : atmosphere.psychologicalPressure;

                if (room.roomType == "corridor")
                {
                    hum += 0.15f;
                }

                if (room.roomType == "dead_end")
                {
                    silence += 0.2f;
                    pressure += 0.08f;
                }

                if (room.roomType == "transition_room")
                {
                    flicker += 0.15f;
                }

                if (room.roomId != null && room.roomId.StartsWith("branch_"))
                {
                    pressure += 0.06f;
                }

                tags.Add(new RoomAtmosphereTag
                {
                    roomId = room.roomId,
                    atmosphereId = atmosphere == null ? string.Empty : atmosphere.atmosphereId,
                    localHumIntensity = Mathf.Clamp01(hum),
                    localFlickerChance = Mathf.Clamp01(flicker),
                    localSilenceChance = Mathf.Clamp01(silence),
                    psychologicalPressure = Mathf.Clamp01(pressure),
                    notes = "Wave 8 primitive room atmosphere tag."
                });
            }

            return tags;
        }
    }
}
