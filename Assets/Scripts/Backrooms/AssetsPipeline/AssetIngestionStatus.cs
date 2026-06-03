namespace Backrooms.AssetsPipeline
{
    public enum AssetIngestionStatus
    {
        Unknown,
        Discovered,
        LicenseChecked,
        DownloadQueued,
        Downloaded,
        Imported,
        Validated,
        Rejected
    }
}
