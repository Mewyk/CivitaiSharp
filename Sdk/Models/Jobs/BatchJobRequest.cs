namespace CivitaiSharp.Sdk.Models.Jobs;

using System.Collections.Generic;
using System.Text.Json.Serialization;

/// <summary>
/// Request model for batch job submission.
/// </summary>
public sealed class BatchJobRequest
{
    /// <summary>
    /// Gets or sets the jobs to submit in this batch.
    /// </summary>
    [JsonPropertyName("jobs")]
    public required IReadOnlyList<TextToImageJobRequest> Jobs { get; init; }
}
