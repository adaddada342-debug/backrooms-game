using System;
using UnityEngine;

namespace Backrooms.Soundscape.Assets
{
    [Serializable]
    public class SoundscapeClipSlot
    {
        public string slotId;
        public string soundTag;
        public AudioClip clip;
        public float defaultVolume;
        public bool loop;
        public string notes;
    }
}
