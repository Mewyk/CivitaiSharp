namespace CivitaiSharp.Tools.Downloads;

/// <summary>
/// Result of a file download operation containing the file path and metadata.
/// </summary>
/// <param name="FilePath">The absolute path to the downloaded file.</param>
/// <param name="SizeBytes">The size of the downloaded file in bytes.</param>
/// <param name="IsVerified">
/// True if the file hash was verified against an expected value and matched.
/// False if verification was not performed or was skipped.
/// </param>
/// <param name="ComputedHash">
/// The computed hash value if hashing was performed, or null if no hash was computed.
/// Always lowercase hexadecimal format.
/// </param>
public sealed record DownloadedFile(
    string FilePath,
    long SizeBytes,
    bool IsVerified,
    string? ComputedHash);
