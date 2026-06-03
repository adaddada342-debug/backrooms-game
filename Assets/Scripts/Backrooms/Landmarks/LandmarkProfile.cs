using System;

namespace Backrooms.Landmarks
{
    [Serializable]
    public class LandmarkProfile
    {
        public string landmarkId;
        public string displayName;
        public string description;
        public string landmarkType;
        public float rarity;
        public float importance;
        public bool requiredForLevelIdentity;
        public bool uniquePerPackage;
        public string[] allowedRoomTypes;
        public string[] forbiddenRoomTypes;
    }
}
