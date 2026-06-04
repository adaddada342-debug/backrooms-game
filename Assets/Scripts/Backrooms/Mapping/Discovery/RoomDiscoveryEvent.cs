using System;
using UnityEngine;

namespace Backrooms.Mapping.Discovery
{
    [Serializable]
    public class RoomDiscoveryEvent
    {
        public string roomId;
        public string roomType;
        public Vector3 worldPosition;
        public string discoveredAtUtc;
        public bool firstDiscovery;
        public string discoveryReason;
    }
}
