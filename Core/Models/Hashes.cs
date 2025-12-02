namespace CivitaiSharp.Core.Models;

using System.Text.Json.Serialization;

/// <summary>
/// Checksums and hashes for a model file using various algorithms.
/// </summary>
/// <param name="Sha256">SHA256 hash of the model file. Maps to JSON property "SHA256".</param>
/// <param name="Crc32">CRC32 hash of the model file. Maps to JSON property "CRC32".</param>
/// <param name="Blake3">BLAKE3 hash of the model file. Maps to JSON property "BLAKE3".</param>
/// <param name="AutoV1">AutoV1 hash algorithm result. Maps to JSON property "AutoV1".</param>
/// <param name="AutoV2">AutoV2 hash algorithm result. Maps to JSON property "AutoV2".</param>
/// <param name="AutoV3">AutoV3 hash algorithm result. Maps to JSON property "AutoV3".</param>
public sealed record Hashes(
    [property: JsonPropertyName("SHA256")] string? Sha256 = null,
    [property: JsonPropertyName("CRC32")] string? Crc32 = null,
    [property: JsonPropertyName("BLAKE3")] string? Blake3 = null,
    [property: JsonPropertyName("AutoV1")] string? AutoV1 = null,
    [property: JsonPropertyName("AutoV2")] string? AutoV2 = null,
    [property: JsonPropertyName("AutoV3")] string? AutoV3 = null);
