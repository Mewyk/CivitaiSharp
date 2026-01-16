namespace CivitaiSharp.Sdk.Models.Results;

using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using CivitaiSharp.Sdk.Json.Converters;

/// <summary>
/// Represents the result payload of a job.
/// This type does NOT contain an ErrorMessage property.
/// To check for failures, inspect <see cref="JobStatus.LastEvent"/> and check if <see cref="JobEvent.Type"/> is Failed.
/// </summary>
/// <remarks>
/// The Civitai job API can return different shapes for <c>result</c> depending on job type.
/// This type preserves the raw JSON while exposing commonly used blob fields for image jobs.
/// </remarks>
/// <param name="Raw">The raw JSON value returned by the API for the <c>result</c> property.</param>
[JsonConverter(typeof(JobResultConverter))]
public sealed record JobResult(JsonElement Raw)
{
    /// <summary>
    /// Gets the blob key for an image result, if present.
    /// </summary>
    [JsonIgnore]
    public string? BlobKey => Raw.ValueKind == JsonValueKind.Object && Raw.TryGetProperty("blobKey", out var value) && value.ValueKind == JsonValueKind.String
        ? value.GetString()
        : null;

    /// <summary>
    /// Gets whether the result is available for download.
    /// Defaults to <see langword="false"/> when the API does not provide the field.
    /// </summary>
    [JsonIgnore]
    public bool Available => Raw.ValueKind == JsonValueKind.Object && Raw.TryGetProperty("available", out var value) && value.ValueKind is JsonValueKind.True or JsonValueKind.False && value.GetBoolean();

    /// <summary>
    /// Gets the temporary URL for downloading the generated image, if present.
    /// </summary>
    [JsonIgnore]
    public string? BlobUrl => Raw.ValueKind == JsonValueKind.Object && Raw.TryGetProperty("blobUrl", out var value) && value.ValueKind == JsonValueKind.String
        ? value.GetString()
        : null;

    /// <summary>
    /// Gets the expiration date for the blob URL, if present.
    /// </summary>
    [JsonIgnore]
    public DateTime? BlobUrlExpirationDate => Raw.ValueKind == JsonValueKind.Object && Raw.TryGetProperty("blobUrlExpirationDate", out var value) && value.ValueKind == JsonValueKind.String && value.TryGetDateTime(out var dateTime)
        ? dateTime
        : null;
}
