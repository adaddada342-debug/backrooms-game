using System;
using System.Collections.Generic;

namespace Backrooms.LayoutSynthesis.Comparison
{
    [Serializable]
    public class LayoutSeedComparisonReport
    {
        public string reportId;
        public string levelId;
        public int seedStart;
        public int seedCount;
        public int successCount;
        public int failureCount;
        public float averageReadabilityScore;
        public float bestReadabilityScore;
        public int bestSeed;
        public float worstReadabilityScore;
        public int worstSeed;
        public List<LayoutSeedComparisonEntry> entries = new List<LayoutSeedComparisonEntry>();

        public void RecalculateSummary()
        {
            successCount = 0;
            failureCount = 0;
            averageReadabilityScore = 0f;
            bestReadabilityScore = -1f;
            worstReadabilityScore = 2f;
            bestSeed = 0;
            worstSeed = 0;

            if (entries == null || entries.Count == 0)
            {
                bestReadabilityScore = 0f;
                worstReadabilityScore = 0f;
                return;
            }

            float total = 0f;
            foreach (LayoutSeedComparisonEntry entry in entries)
            {
                if (entry == null)
                {
                    continue;
                }

                if (entry.synthesisSucceeded && entry.assemblyValidationPassed && entry.readabilityPassed)
                {
                    successCount++;
                }
                else
                {
                    failureCount++;
                }

                total += entry.readabilityScore;
                if (entry.readabilityScore > bestReadabilityScore)
                {
                    bestReadabilityScore = entry.readabilityScore;
                    bestSeed = entry.seed;
                }

                if (entry.readabilityScore < worstReadabilityScore)
                {
                    worstReadabilityScore = entry.readabilityScore;
                    worstSeed = entry.seed;
                }
            }

            averageReadabilityScore = total / entries.Count;
            if (bestReadabilityScore < 0f)
            {
                bestReadabilityScore = 0f;
            }

            if (worstReadabilityScore > 1f)
            {
                worstReadabilityScore = 0f;
            }
        }
    }
}
