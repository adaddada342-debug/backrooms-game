using System;
using System.Collections.Generic;

namespace Backrooms.LayoutSynthesis.Landmarks
{
    [Serializable]
    public class LandmarkPlacementPlan
    {
        public string planId;
        public string packageId;
        public string levelId;
        public List<LandmarkPlacement> placements = new List<LandmarkPlacement>();

        public LandmarkPlacement FindByLandmarkId(string landmarkId)
        {
            if (placements == null || string.IsNullOrWhiteSpace(landmarkId))
            {
                return null;
            }

            foreach (LandmarkPlacement placement in placements)
            {
                if (placement != null && string.Equals(placement.landmarkId, landmarkId, StringComparison.OrdinalIgnoreCase))
                {
                    return placement;
                }
            }

            return null;
        }

        public List<LandmarkPlacement> FindByRoomId(string roomId)
        {
            List<LandmarkPlacement> matches = new List<LandmarkPlacement>();
            if (placements == null || string.IsNullOrWhiteSpace(roomId))
            {
                return matches;
            }

            foreach (LandmarkPlacement placement in placements)
            {
                if (placement != null && string.Equals(placement.roomId, roomId, StringComparison.Ordinal))
                {
                    matches.Add(placement);
                }
            }

            return matches;
        }
    }
}
