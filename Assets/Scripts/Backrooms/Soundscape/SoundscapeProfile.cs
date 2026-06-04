using System;

namespace Backrooms.Soundscape
{
    [Serializable]
    public class SoundscapeProfile
    {
        public string soundscapeId;
        public string displayName;
        public string description;
        public float masterVolume;
        public float humVolume;
        public float airVolume;
        public float distantRumbleVolume;
        public float silenceChance;
        public float reverbIntensity;
        public float lowPassAmount;
        public bool useProceduralPlaceholders;
        public string[] soundTags;
    }
}
