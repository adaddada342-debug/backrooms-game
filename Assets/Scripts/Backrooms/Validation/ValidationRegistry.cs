using System.Collections.Generic;
using UnityEngine;

namespace Backrooms.Validation
{
    [CreateAssetMenu(
        fileName = "ValidationRegistry",
        menuName = "Backrooms/Validation/Validation Registry")]
    public class ValidationRegistry : ScriptableObject
    {
        public List<LevelValidationReport> reports = new List<LevelValidationReport>();

        public bool TryGetByReportId(string reportId, out LevelValidationReport report)
        {
            report = null;

            if (string.IsNullOrWhiteSpace(reportId) || reports == null)
            {
                return false;
            }

            foreach (LevelValidationReport candidate in reports)
            {
                if (candidate == null)
                {
                    continue;
                }

                if (string.Equals(candidate.reportId, reportId, System.StringComparison.Ordinal))
                {
                    report = candidate;
                    return true;
                }
            }

            return false;
        }

        public bool IsPackageApproved(string packageId)
        {
            if (string.IsNullOrWhiteSpace(packageId) || reports == null)
            {
                return false;
            }

            foreach (LevelValidationReport report in reports)
            {
                if (report == null ||
                    !string.Equals(report.packageId, packageId, System.StringComparison.Ordinal))
                {
                    continue;
                }

                bool hasBlockers = report.blockers != null && report.blockers.Count > 0;
                return report.passed &&
                       report.navmeshPassed &&
                       report.licensePassed &&
                       report.attributionPassed &&
                       report.performancePassed &&
                       !hasBlockers;
            }

            return false;
        }
    }
}
