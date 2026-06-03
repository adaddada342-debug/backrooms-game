using System.Collections.Generic;
using UnityEngine;

namespace Backrooms.Credits
{
    [CreateAssetMenu(
        fileName = "CreditsRegistry",
        menuName = "Backrooms/Credits/Credits Registry")]
    public class CreditsRegistry : ScriptableObject
    {
        public List<CreditEntry> credits = new List<CreditEntry>();

        public List<CreditEntry> FindByPackageId(string packageId)
        {
            List<CreditEntry> matches = new List<CreditEntry>();

            if (string.IsNullOrWhiteSpace(packageId) || credits == null)
            {
                return matches;
            }

            foreach (CreditEntry credit in credits)
            {
                if (credit == null)
                {
                    continue;
                }

                if (string.Equals(credit.packageId, packageId, System.StringComparison.Ordinal))
                {
                    matches.Add(credit);
                }
            }

            return matches;
        }

        public bool HasCompleteCreditsForPackage(string packageId)
        {
            List<CreditEntry> packageCredits = FindByPackageId(packageId);
            if (packageCredits.Count == 0)
            {
                return false;
            }

            foreach (CreditEntry credit in packageCredits)
            {
                if (credit == null ||
                    string.IsNullOrWhiteSpace(credit.creatorName) ||
                    string.IsNullOrWhiteSpace(credit.sourceUrl) ||
                    string.IsNullOrWhiteSpace(credit.licenseName) ||
                    string.IsNullOrWhiteSpace(credit.attributionText))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
