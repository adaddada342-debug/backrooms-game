using Backrooms.Core;
using Backrooms.LayoutSynthesis.Level0;
using Backrooms.LayoutSynthesis.Models;
using Backrooms.ProductionData;
using UnityEngine;

namespace Backrooms.SceneAssembly.Primitive
{
    public static class PrimitiveLevel0BlockoutFactory
    {
        public static LayoutSynthesisResult LastSynthesisResult { get; private set; }
        public static bool LastSynthesisUsedFallback { get; private set; }

        public static SceneAssemblyPlan CreateSynthesizedDefaultPlan()
        {
            LayoutSynthesisRequest request = Level0LayoutSynthesisRequestFactory.CreateDefaultRequest();
            Level0LayoutSynthesizer synthesizer = new Level0LayoutSynthesizer();
            LastSynthesisResult = synthesizer.Synthesize(request);
            LastSynthesisUsedFallback = LastSynthesisResult == null || !LastSynthesisResult.succeeded || LastSynthesisResult.plan == null;

            if (!LastSynthesisUsedFallback)
            {
                return LastSynthesisResult.plan;
            }

            Debug.LogWarning("Level 0 layout synthesis failed. Falling back to the hardcoded primitive Level 0 plan.");
            if (LastSynthesisResult != null && LastSynthesisResult.issues != null)
            {
                foreach (LayoutSynthesisIssue issue in LastSynthesisResult.issues)
                {
                    if (issue == null)
                    {
                        continue;
                    }

                    string message = issue.code + ": " + issue.message;
                    if (issue.blocker)
                    {
                        Debug.LogError(message);
                    }
                    else
                    {
                        Debug.LogWarning(message);
                    }
                }
            }

            return CreateDefaultPlan();
        }

        public static SceneAssemblyPlan CreateDefaultPlan()
        {
            SceneAssemblyPlan plan = new SceneAssemblyPlan
            {
                planId = "level0_local_blockout_plan",
                packageId = BackroomsConstants.DefaultPackageId,
                sceneName = "Level0_Local_Blockout",
                levelId = BackroomsConstants.DefaultLevelId,
                seed = 1001,
                identity = Level0ProductionProfiles.CreateLevel0Identity(),
                grammar = Level0ProductionProfiles.CreateLevel0Grammar(),
                atmosphere = Level0ProductionProfiles.CreateLevel0Atmosphere(),
                landmarks = Level0ProductionProfiles.CreateLevel0Landmarks()
            };

            plan.rooms.Add(new BlockoutRoomPlan
            {
                roomId = "spawn_office",
                roomType = "spawn_office",
                position = new Vector3(0f, 0f, 0f),
                size = new Vector3(10f, 3f, 8f),
                materialHint = "wallpaper"
            });

            plan.rooms.Add(new BlockoutRoomPlan
            {
                roomId = "long_corridor",
                roomType = "corridor",
                position = new Vector3(0f, 0f, 10f),
                size = new Vector3(4f, 3f, 16f),
                materialHint = "wallpaper"
            });

            plan.rooms.Add(new BlockoutRoomPlan
            {
                roomId = "side_dead_end",
                roomType = "dead_end",
                position = new Vector3(8f, 0f, 12f),
                size = new Vector3(8f, 3f, 6f),
                materialHint = "wallpaper"
            });

            plan.rooms.Add(new BlockoutRoomPlan
            {
                roomId = "transition_room",
                roomType = "transition_room",
                position = new Vector3(0f, 0f, 22f),
                size = new Vector3(8f, 3f, 8f),
                materialHint = "wallpaper"
            });

            plan.connections.Add(new BlockoutConnectionPlan
            {
                connectionId = "spawn_to_corridor",
                fromRoomId = "spawn_office",
                toRoomId = "long_corridor",
                position = new Vector3(0f, 0f, 5f),
                size = new Vector3(3f, 3f, 2f)
            });

            plan.connections.Add(new BlockoutConnectionPlan
            {
                connectionId = "corridor_to_dead_end",
                fromRoomId = "long_corridor",
                toRoomId = "side_dead_end",
                position = new Vector3(4f, 0f, 12f),
                size = new Vector3(4f, 3f, 2.5f)
            });

            plan.connections.Add(new BlockoutConnectionPlan
            {
                connectionId = "corridor_to_transition",
                fromRoomId = "long_corridor",
                toRoomId = "transition_room",
                position = new Vector3(0f, 0f, 18f),
                size = new Vector3(3f, 3f, 2f)
            });

            plan.openings.Add(new BlockoutOpeningPlan
            {
                openingId = "spawn_office_to_corridor",
                roomId = "spawn_office",
                position = new Vector3(0f, 1.2f, 4f),
                size = new Vector3(3f, 2.4f, 0.2f),
                directionHint = "north"
            });

            plan.openings.Add(new BlockoutOpeningPlan
            {
                openingId = "corridor_to_spawn_office",
                roomId = "long_corridor",
                position = new Vector3(0f, 1.2f, 2f),
                size = new Vector3(3f, 2.4f, 0.2f),
                directionHint = "south"
            });

            plan.openings.Add(new BlockoutOpeningPlan
            {
                openingId = "corridor_to_side_dead_end",
                roomId = "long_corridor",
                position = new Vector3(2f, 1.2f, 12f),
                size = new Vector3(0.2f, 2.4f, 3.2f),
                directionHint = "east"
            });

            plan.openings.Add(new BlockoutOpeningPlan
            {
                openingId = "side_dead_end_to_corridor",
                roomId = "side_dead_end",
                position = new Vector3(4f, 1.2f, 12f),
                size = new Vector3(0.2f, 2.4f, 3.2f),
                directionHint = "west"
            });

            plan.openings.Add(new BlockoutOpeningPlan
            {
                openingId = "corridor_to_transition_room",
                roomId = "long_corridor",
                position = new Vector3(0f, 1.2f, 18f),
                size = new Vector3(3f, 2.4f, 0.2f),
                directionHint = "north"
            });

            plan.openings.Add(new BlockoutOpeningPlan
            {
                openingId = "transition_room_to_corridor",
                roomId = "transition_room",
                position = new Vector3(0f, 1.2f, 18f),
                size = new Vector3(3f, 2.4f, 0.2f),
                directionHint = "south"
            });

            plan.lights.Add(new BlockoutLightPlan
            {
                lightId = "spawn_light_bar",
                position = new Vector3(0f, 2.75f, 0f),
                size = new Vector3(3f, 0.08f, 0.35f),
                intensity = 1.6f,
                lightTypeHint = "fluorescent_bar"
            });

            plan.lights.Add(new BlockoutLightPlan
            {
                lightId = "corridor_light_bar",
                position = new Vector3(0f, 2.75f, 11f),
                size = new Vector3(2.5f, 0.08f, 0.35f),
                intensity = 1.4f,
                lightTypeHint = "fluorescent_bar"
            });

            plan.lights.Add(new BlockoutLightPlan
            {
                lightId = "transition_light_bar",
                position = new Vector3(0f, 2.75f, 22f),
                size = new Vector3(3f, 0.08f, 0.35f),
                intensity = 1.3f,
                lightTypeHint = "fluorescent_bar"
            });

            plan.transitions.Add(new BlockoutTransitionPlan
            {
                transitionId = "local_test_transition",
                position = new Vector3(0f, 1.25f, 25f),
                size = new Vector3(3f, 2.5f, 1f),
                targetLevelId = BackroomsConstants.DefaultLevelId,
                targetPackageId = BackroomsConstants.DefaultPackageId,
                transitionType = "local_test_loop"
            });

            return plan;
        }
    }
}
