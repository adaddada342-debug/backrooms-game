using System;
using UnityEngine;

namespace Backrooms.Soundscape
{
    [Serializable]
    public class SoundscapeEmitterPlan
    {
        public string emitterId;
        public string roomId;
        public Vector3 position;
        public float volume;
        public float minDistance;
        public float maxDistance;
        public bool loop;
        public string soundTag;
        public string notes;
    }
}
