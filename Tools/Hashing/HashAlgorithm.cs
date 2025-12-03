namespace CivitaiSharp.Tools.Hashing;

/// <summary>
/// Specifies the hash algorithm to use for file hashing operations.
/// </summary>
public enum HashAlgorithm
{
    /// <summary>
    /// SHA-256 hash algorithm - widely compatible and used by Civitai for model verification.
    /// Produces a 256-bit (32-byte) hash, represented as 64 hexadecimal characters.
    /// </summary>
    Sha256,

    /// <summary>
    /// SHA-512 hash algorithm - provides stronger security than SHA-256.
    /// Produces a 512-bit (64-byte) hash, represented as 128 hexadecimal characters.
    /// </summary>
    Sha512,

    /// <summary>
    /// BLAKE3 hash algorithm - fast, modern, and cryptographically secure.
    /// Used by Civitai for model file hashing. Produces a 256-bit hash by default.
    /// </summary>
    Blake3,

    /// <summary>
    /// CRC32 checksum algorithm - fast but not cryptographically secure.
    /// Used for basic integrity checks. Produces a 32-bit checksum.
    /// </summary>
    Crc32
}
