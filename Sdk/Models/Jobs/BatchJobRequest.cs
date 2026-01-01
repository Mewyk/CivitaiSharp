namespace CivitaiSharp.Sdk.Models.Jobs;

using System.Collections.Generic;
using System.Text.Json.Serialization;

/// <summary>
/// Request model for submitting multiple image generation jobs as a batch.
/// </summary>
public sealed class BatchJobRequest
{
    /// <summary>
    /// Gets or sets the collection of image generation job requests to submit.
    /// </summary>
    [JsonPropertyName("jobs")]
    public required IReadOnlyList<ImageGenerationJobRequest> Jobs { get; init; }
}
