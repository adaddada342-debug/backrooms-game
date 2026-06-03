using System;
using UnityEngine;

namespace Backrooms.LayoutSynthesis.Models
{
    [Serializable]
    public class LayoutEdge
    {
        public string edgeId;
        public string fromNodeId;
        public string toNodeId;
        public Vector2Int fromGridPosition;
        public Vector2Int toGridPosition;
        public Vector3 worldPosition;
        public Vector3 size;
        public string direction;
    }
}
