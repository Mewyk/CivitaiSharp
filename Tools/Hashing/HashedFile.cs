namespace CivitaiSharp.Tools.Hashing;

using System;

/// <summary>
/// Result of a file hash computation containing the hash value and metadata.
/// </summary>
/// <param name="FilePath">The absolute path to the file that was hashed, or null if hashed from a stream.</param>
/// <param name="Hash">The computed hash as a lowercase hexadecimal string.</param>
/// <param name="Algorithm">The algorithm used to compute the hash.</param>
/// <param name="FileSize">The size of the data that was hashed in bytes. Returns -1 for non-seekable streams.</param>
/// <param name="ComputationTime">The time taken to compute the hash.</param>
public sealed record HashedFile(
    string? FilePath,
    string Hash,
    HashAlgorithm Algorithm,
    long FileSize,
    TimeSpan ComputationTime);
