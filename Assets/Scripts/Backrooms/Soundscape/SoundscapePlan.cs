using System;
using System.Collections.Generic;

namespace Backrooms.Soundscape
{
    [Serializable]
    public class SoundscapePlan
    {
        public string planId;
        public string packageId;
        public string levelId;
        public SoundscapeProfile profile;
        public List<SoundscapeEmitterPlan> emitters = new List<SoundscapeEmitterPlan>();
    }
}
