using System;
using System.Collections.Generic;

namespace Backrooms.LayoutSynthesis.Routes
{
    [Serializable]
    public class LayoutRouteAnnotation
    {
        public string routeId;
        public string packageId;
        public string levelId;
        public string startRoomId;
        public string endRoomId;
        public List<string> orderedRoomIds = new List<string>();
        public List<string> connectionIds = new List<string>();
        public int routeLength;
        public bool reachesTransition;
        public float routeComplexity;
        public string notes;

        public bool ContainsRoom(string roomId)
        {
            if (orderedRoomIds == null || string.IsNullOrWhiteSpace(roomId))
            {
                return false;
            }

            foreach (string candidate in orderedRoomIds)
            {
                if (string.Equals(candidate, roomId, StringComparison.Ordinal))
                {
                    return true;
                }
            }

            return false;
        }

        public bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(startRoomId) &&
                   !string.IsNullOrWhiteSpace(endRoomId) &&
                   orderedRoomIds != null &&
                   orderedRoomIds.Count > 0 &&
                   reachesTransition;
        }
    }
}
