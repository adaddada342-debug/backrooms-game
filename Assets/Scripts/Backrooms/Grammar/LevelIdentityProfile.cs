using System;

namespace Backrooms.Grammar
{
    [Serializable]
    public class LevelIdentityProfile
    {
        public string levelId;
        public string displayName;
        public string description;
        public string dangerRating;
        public string atmosphereId;
        public string grammarId;
        public string visualTheme;
        public string audioTheme;
        public string[] requiredLandmarks;
        public string[] forbiddenLandmarks;
        public string[] allowedRoomTypes;
        public string[] forbiddenRoomTypes;
        public float navigationComplexity;
        public float liminalityScore;
        public float repetitionScore;
        public float landmarkDensity;
        public float perceivedSafety;
        public float isolationScore;
        public float environmentalVariation;
        public string notes;

        public bool AllowsRoomType(string roomType)
        {
            if (string.IsNullOrWhiteSpace(roomType))
            {
                return false;
            }

            if (Contains(forbiddenRoomTypes, roomType))
            {
                return false;
            }

            return allowedRoomTypes == null ||
                   allowedRoomTypes.Length == 0 ||
                   Contains(allowedRoomTypes, roomType);
        }

        public bool RequiresLandmark(string landmarkId)
        {
            return Contains(requiredLandmarks, landmarkId);
        }

        public bool ForbidsLandmark(string landmarkId)
        {
            return Contains(forbiddenLandmarks, landmarkId);
        }

        private static bool Contains(string[] values, string value)
        {
            if (values == null || string.IsNullOrWhiteSpace(value))
            {
                return false;
            }

            foreach (string candidate in values)
            {
                if (string.Equals(candidate, value, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
