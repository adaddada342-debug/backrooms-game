using System;

namespace Backrooms.Atmosphere
{
    [Serializable]
    public class RoomAtmosphereTag
    {
        public string roomId;
        public string atmosphereId;
        public float localHumIntensity;
        public float localFlickerChance;
        public float localSilenceChance;
        public float psychologicalPressure;
        public string notes;
    }
}
