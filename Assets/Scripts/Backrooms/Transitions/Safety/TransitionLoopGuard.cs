using System.Collections.Generic;
using UnityEngine;

namespace Backrooms.Transitions.Safety
{
    public static class TransitionLoopGuard
    {
        private const float DefaultCooldownSeconds = 1f;
        private static readonly Dictionary<string, float> LastTransitionTimes = new Dictionary<string, float>();

        public static bool CanTransition(string targetPackageId)
        {
            return CanTransition(targetPackageId, DefaultCooldownSeconds);
        }

        public static bool CanTransition(string targetPackageId, float cooldownSeconds)
        {
            string key = string.IsNullOrWhiteSpace(targetPackageId) ? "<missing-package>" : targetPackageId;
            float now = Time.realtimeSinceStartup;
            float lastTime;
            if (!LastTransitionTimes.TryGetValue(key, out lastTime))
            {
                return true;
            }

            return now - lastTime >= Mathf.Max(0f, cooldownSeconds);
        }

        public static void MarkTransition(string targetPackageId)
        {
            string key = string.IsNullOrWhiteSpace(targetPackageId) ? "<missing-package>" : targetPackageId;
            LastTransitionTimes[key] = Time.realtimeSinceStartup;
        }
    }
}
