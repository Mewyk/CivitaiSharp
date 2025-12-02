namespace CivitaiSharp.Sdk.Models.Results;

using System;
using System.Text.Json.Serialization;

/// <summary>
/// Result information for a completed or in-progress job.
/// </summary>
/// <param name="BlobKey">The blob key for the generated image. Maps to JSON property "blobKey".</param>
/// <param name="Available">Indicates whether the result is available for download. Maps to JSON property "available".</param>
/// <param name="BlobUrl">The temporary URL for downloading the generated image. Maps to JSON property "blobUrl".</param>
/// <param name="BlobUrlExpirationDate">The expiration date for the blob URL. Maps to JSON property "blobUrlExpirationDate".</param>
public sealed record JobResult(
    [property: JsonPropertyName("blobKey")] string? BlobKey,
    [property: JsonPropertyName("available")] bool Available,
    [property: JsonPropertyName("blobUrl")] string? BlobUrl,
    [property: JsonPropertyName("blobUrlExpirationDate")] DateTime? BlobUrlExpirationDate);
