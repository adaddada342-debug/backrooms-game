using System;
using UnityEngine;

namespace Backrooms.Atmosphere
{
    [Serializable]
    public class AtmosphereProfile
    {
        public string atmosphereId;
        public string displayName;
        public string description;
        public Color ambientColor;
        public float fogDensity;
        public float exposure;
        public float bloom;
        public float lightIntensity;
        public float darknessBias;
        public float audioVolume;
        public float ambienceDensity;
        public float reverbIntensity;
        public float flickerChance;
        public float silenceChance;
        public float humIntensity;
        public float perceivedSafety;
        public float psychologicalPressure;
    }
}
