using System;
using UnityEngine;

namespace Backrooms.LayoutSynthesis.Landmarks
{
    [Serializable]
    public class LandmarkPlacement
    {
        public string placementId;
        public string landmarkId;
        public string roomId;
        public Vector3 position;
        public string placementReason;
        public bool required;
        public float importance;
    }
}
