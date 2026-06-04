using System;

namespace Backrooms.Mapping.Discovery
{
    [Serializable]
    public class MapDiscoverySettings
    {
        public float roomDiscoveryRadius = 6f;
        public float discoveryCheckInterval = 0.25f;
        public bool discoverNearestRoomOnStart = true;
        public bool revealMainRouteOnDebug = false;
        public bool showUndiscoveredRooms = true;
        public bool dimUndiscoveredRooms = true;
        public bool saveDiscoveryImmediately = true;
        public string notes;

        public void ClampValues()
        {
            if (roomDiscoveryRadius < 1f)
            {
                roomDiscoveryRadius = 1f;
            }

            if (discoveryCheckInterval < 0.05f)
            {
                discoveryCheckInterval = 0.05f;
            }
        }
    }
}
