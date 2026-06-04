using Backrooms.Landmarks;
using UnityEngine;

namespace Backrooms.Landmarks.Runtime
{
    public class LandmarkPlaceholder : MonoBehaviour
    {
        public string landmarkId;
        public string displayName;
        public string landmarkType;
        public float importance;
        public bool requiredForLevelIdentity;

        public void Configure(LandmarkProfile profile)
        {
            landmarkId = profile == null ? string.Empty : profile.landmarkId;
            displayName = profile == null ? string.Empty : profile.displayName;
            landmarkType = profile == null ? string.Empty : profile.landmarkType;
            importance = profile == null ? 0f : profile.importance;
            requiredForLevelIdentity = profile != null && profile.requiredForLevelIdentity;
        }
    }
}
