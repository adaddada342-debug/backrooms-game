using System;
using System.Collections.Generic;
using Backrooms.Atmosphere;
using Backrooms.Grammar;
using Backrooms.Landmarks;
using UnityEngine;

namespace Backrooms.LayoutSynthesis.Models
{
    [Serializable]
    public class LayoutSynthesisRequest
    {
        public string requestId;
        public string packageId;
        public string levelId;
        public int seed;
        public LevelIdentityProfile identity;
        public RoomGrammarProfile grammar;
        public AtmosphereProfile atmosphere;
        public List<RoomArchetype> roomArchetypes = new List<RoomArchetype>();
        public List<LandmarkProfile> landmarks = new List<LandmarkProfile>();
        public int targetRoomCount;
        public int targetMainRouteLength;
        public int targetBranchCount;
        public int targetDeadEndCount;
        public Vector3 origin;
        public float gridSize;
        public float roomSpacing;
        public float corridorWidth;
        public float defaultHeight;
        public bool includeSideBranches;
        public bool includeDeadEnds;
        public bool includeLandmarks;
        public bool includeTransition;
        public string targetSceneName;

        public bool HasRequiredProfiles()
        {
            return identity != null &&
                   grammar != null &&
                   atmosphere != null &&
                   roomArchetypes != null &&
                   roomArchetypes.Count > 0 &&
                   landmarks != null &&
                   landmarks.Count > 0;
        }

        public bool HasValidCounts()
        {
            if (targetRoomCount <= 0 ||
                targetMainRouteLength <= 0 ||
                targetBranchCount < 0 ||
                targetDeadEndCount < 0)
            {
                return false;
            }

            if (grammar == null)
            {
                return true;
            }

            return targetRoomCount >= grammar.minimumRooms &&
                   targetRoomCount <= grammar.maximumRooms;
        }
    }
}
