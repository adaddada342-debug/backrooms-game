using Backrooms.Core;
using Backrooms.LayoutSynthesis.Models;
using Backrooms.ProductionData;
using UnityEngine;

namespace Backrooms.LayoutSynthesis.Level0
{
    public static class Level0LayoutSynthesisRequestFactory
    {
        public static LayoutSynthesisRequest CreateDefaultRequest()
        {
            return new LayoutSynthesisRequest
            {
                requestId = "level0_synthesis_default",
                packageId = BackroomsConstants.DefaultPackageId,
                levelId = BackroomsConstants.DefaultLevelId,
                seed = 1001,
                identity = Level0ProductionProfiles.CreateLevel0Identity(),
                grammar = Level0ProductionProfiles.CreateLevel0Grammar(),
                atmosphere = Level0ProductionProfiles.CreateLevel0Atmosphere(),
                roomArchetypes = Level0ProductionProfiles.CreateLevel0RoomArchetypes(),
                landmarks = Level0ProductionProfiles.CreateLevel0Landmarks(),
                targetRoomCount = 9,
                targetMainRouteLength = 6,
                targetBranchCount = 2,
                targetDeadEndCount = 1,
                origin = Vector3.zero,
                gridSize = 1f,
                roomSpacing = 10f,
                corridorWidth = 3f,
                defaultHeight = 3f,
                includeSideBranches = true,
                includeDeadEnds = true,
                includeLandmarks = true,
                includeTransition = true,
                targetSceneName = "Level0_Local_Blockout"
            };
        }
    }
}
