using System;
using UnityEngine;

namespace Backrooms.Grammar
{
    [Serializable]
    public class RoomArchetype
    {
        public string roomType;
        public string displayName;
        public string description;
        public Vector3 minimumSize;
        public Vector3 maximumSize;
        public bool allowLandmarks;
        public bool allowTransitions;
        public bool allowDeadEndUsage;
        public float encounterWeight;
        public float landmarkWeight;
        public float repetitionWeight;
        public string[] compatibleNeighbors;
        public string[] incompatibleNeighbors;
    }
}
