using System.Collections.Generic;
using Backrooms.Atmosphere;
using Backrooms.Core;
using Backrooms.Grammar;
using Backrooms.Landmarks;
using UnityEngine;

namespace Backrooms.ProductionData
{
    public static class Level0ProductionProfiles
    {
        public static LevelIdentityProfile CreateLevel0Identity()
        {
            return new LevelIdentityProfile
            {
                levelId = BackroomsConstants.DefaultLevelId,
                displayName = "Level 0",
                description = "An endless, office-like liminal interior of yellow walls, damp carpet, fluorescent hum, and unreliable repetition.",
                dangerRating = "low_direct_threat_high_disorientation",
                atmosphereId = "level0_default",
                grammarId = "level0_office_liminal_grammar",
                visualTheme = "yellow_wallpaper_damp_carpet_fluorescent_office",
                audioTheme = "fluorescent_hum_air_movement_distant_silence",
                requiredLandmarks = new[] { "yellow_pillar", "broken_light_cluster", "wet_carpet_patch" },
                forbiddenLandmarks = new[] { "monster_nest", "weapon_cache", "boss_arena" },
                allowedRoomTypes = new[] { "spawn_office", "office_room", "corridor", "junction", "storage_room", "transition_room", "dead_end" },
                forbiddenRoomTypes = new[] { "combat_arena", "shop_room", "safe_house", "monster_den" },
                navigationComplexity = 0.62f,
                liminalityScore = 0.95f,
                repetitionScore = 0.88f,
                landmarkDensity = 0.28f,
                perceivedSafety = 0.58f,
                isolationScore = 0.91f,
                environmentalVariation = 0.32f,
                notes = "Classic Level 0 identity: uncanny office repetition, not a chase-horror or combat space."
            };
        }

        public static RoomGrammarProfile CreateLevel0Grammar()
        {
            return new RoomGrammarProfile
            {
                grammarId = "level0_office_liminal_grammar",
                displayName = "Level 0 Office Liminal Grammar",
                description = "Office rooms, narrow corridors, occasional branches, sparse landmarks, and controlled dead ends.",
                minimumRooms = 4,
                maximumRooms = 24,
                minimumConnections = 3,
                maximumConnections = 36,
                allowDeadEnds = true,
                allowLoops = false,
                allowBranches = true,
                requireLandmarks = true,
                corridorBias = 0.82f,
                roomBias = 0.55f,
                branchBias = 0.38f,
                deadEndBias = 0.32f,
                loopBias = 0.08f,
                mandatoryRoomTypes = new[] { "spawn_office", "corridor", "transition_room" },
                optionalRoomTypes = new[] { "office_room", "junction", "storage_room", "dead_end" },
                forbiddenRoomTypes = new[] { "combat_arena", "shop_room", "monster_den" }
            };
        }

        public static AtmosphereProfile CreateLevel0Atmosphere()
        {
            return new AtmosphereProfile
            {
                atmosphereId = "level0_default",
                displayName = "Level 0 Default",
                description = "Sickly fluorescent familiarity with damp carpet, low variation, persistent hum, and quiet pressure.",
                ambientColor = new Color(0.78f, 0.74f, 0.48f),
                fogDensity = 0.018f,
                exposure = 0.62f,
                bloom = 0.18f,
                lightIntensity = 0.74f,
                darknessBias = 0.22f,
                audioVolume = 0.72f,
                ambienceDensity = 0.64f,
                reverbIntensity = 0.36f,
                flickerChance = 0.08f,
                silenceChance = 0.18f,
                humIntensity = 0.86f,
                perceivedSafety = 0.55f,
                psychologicalPressure = 0.68f
            };
        }

        public static List<LandmarkProfile> CreateLevel0Landmarks()
        {
            return new List<LandmarkProfile>
            {
                CreateLandmark("yellow_pillar", "Yellow Pillar", "A slightly too-prominent yellow support column used as a navigation anchor.", "structural_anchor", 0.35f, 0.8f, true, false, new[] { "office_room", "corridor", "junction" }),
                CreateLandmark("broken_light_cluster", "Broken Light Cluster", "A group of fluorescent lights with one dead strip and one intermittent flicker.", "lighting_anomaly", 0.28f, 0.75f, true, false, new[] { "corridor", "transition_room", "office_room" }),
                CreateLandmark("wet_carpet_patch", "Wet Carpet Patch", "A darker patch of damp carpet that suggests recent presence without proving it.", "floor_trace", 0.45f, 0.7f, true, false, new[] { "spawn_office", "office_room", "dead_end", "corridor" }),
                CreateLandmark("endless_hall", "Endless Hall", "A corridor sightline that feels longer than the actual blockout can prove.", "route_memory", 0.2f, 0.9f, false, true, new[] { "corridor" }),
                CreateLandmark("office_island", "Office Island", "A lonely office-like cluster implied by room shape rather than props.", "spatial_anchor", 0.25f, 0.65f, false, false, new[] { "office_room", "spawn_office" }),
                CreateLandmark("strange_wall_pattern", "Strange Wall Pattern", "A repeated wallpaper rhythm that appears intentional but resists interpretation.", "surface_memory", 0.5f, 0.55f, false, false, new[] { "office_room", "corridor", "dead_end" })
            };
        }

        public static List<RoomArchetype> CreateLevel0RoomArchetypes()
        {
            return new List<RoomArchetype>
            {
                CreateArchetype("office_room", "Office Room", "Banal carpeted room with low certainty and repeated wall language.", new Vector3(5f, 2.6f, 5f), new Vector3(14f, 3.4f, 12f), true, false, true, 0.7f, 0.45f, 0.8f),
                CreateArchetype("corridor", "Corridor", "Primary traversal space with strong repetition and uncertain distance.", new Vector3(2.5f, 2.6f, 5f), new Vector3(5f, 3.4f, 30f), true, false, false, 1f, 0.3f, 0.9f),
                CreateArchetype("junction", "Junction", "A branching uncertainty node that should not feel like a clean hub.", new Vector3(4f, 2.6f, 4f), new Vector3(10f, 3.4f, 10f), true, false, false, 0.45f, 0.55f, 0.55f),
                CreateArchetype("storage_room", "Storage Room", "A small room implying former function without explaining the level.", new Vector3(3f, 2.6f, 3f), new Vector3(8f, 3.2f, 7f), true, false, true, 0.25f, 0.6f, 0.35f),
                CreateArchetype("transition_room", "Transition Room", "A room that can safely host local transition tests.", new Vector3(5f, 2.6f, 5f), new Vector3(12f, 3.4f, 12f), true, true, false, 0.2f, 0.75f, 0.45f),
                CreateArchetype("dead_end", "Dead End", "A plausible dead end that creates uncertainty without punishment.", new Vector3(3f, 2.6f, 3f), new Vector3(9f, 3.4f, 8f), true, false, true, 0.25f, 0.7f, 0.5f)
            };
        }

        private static LandmarkProfile CreateLandmark(
            string id,
            string displayName,
            string description,
            string type,
            float rarity,
            float importance,
            bool required,
            bool unique,
            string[] allowedRoomTypes)
        {
            return new LandmarkProfile
            {
                landmarkId = id,
                displayName = displayName,
                description = description,
                landmarkType = type,
                rarity = rarity,
                importance = importance,
                requiredForLevelIdentity = required,
                uniquePerPackage = unique,
                allowedRoomTypes = allowedRoomTypes,
                forbiddenRoomTypes = new[] { "combat_arena", "monster_den" }
            };
        }

        private static RoomArchetype CreateArchetype(
            string roomType,
            string displayName,
            string description,
            Vector3 minimumSize,
            Vector3 maximumSize,
            bool allowLandmarks,
            bool allowTransitions,
            bool allowDeadEndUsage,
            float encounterWeight,
            float landmarkWeight,
            float repetitionWeight)
        {
            return new RoomArchetype
            {
                roomType = roomType,
                displayName = displayName,
                description = description,
                minimumSize = minimumSize,
                maximumSize = maximumSize,
                allowLandmarks = allowLandmarks,
                allowTransitions = allowTransitions,
                allowDeadEndUsage = allowDeadEndUsage,
                encounterWeight = encounterWeight,
                landmarkWeight = landmarkWeight,
                repetitionWeight = repetitionWeight,
                compatibleNeighbors = new[] { "office_room", "corridor", "junction", "storage_room", "transition_room", "dead_end" },
                incompatibleNeighbors = new[] { "combat_arena", "monster_den" }
            };
        }
    }
}
