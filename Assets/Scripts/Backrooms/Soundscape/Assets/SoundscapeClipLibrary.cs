using System;
using System.Collections.Generic;
using UnityEngine;

namespace Backrooms.Soundscape.Assets
{
    [CreateAssetMenu(menuName = "Backrooms/Soundscape/Soundscape Clip Library")]
    public class SoundscapeClipLibrary : ScriptableObject
    {
        public string libraryId;
        public List<SoundscapeClipSlot> slots = new List<SoundscapeClipSlot>();

        public SoundscapeClipSlot FindByTag(string soundTag)
        {
            if (slots == null || string.IsNullOrWhiteSpace(soundTag))
            {
                return null;
            }

            foreach (SoundscapeClipSlot slot in slots)
            {
                if (slot != null && string.Equals(slot.soundTag, soundTag, StringComparison.OrdinalIgnoreCase))
                {
                    return slot;
                }
            }

            return null;
        }
    }
}
