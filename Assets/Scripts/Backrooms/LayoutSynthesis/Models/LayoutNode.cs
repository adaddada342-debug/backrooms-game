using System;
using UnityEngine;

namespace Backrooms.LayoutSynthesis.Models
{
    [Serializable]
    public class LayoutNode
    {
        public string nodeId;
        public string roomType;
        public Vector2Int gridPosition;
        public Vector3 worldPosition;
        public Vector3 size;
        public bool isMainRoute;
        public bool isBranch;
        public bool isDeadEnd;
        public bool isTransitionRoom;
        public string landmarkId;
    }
}
