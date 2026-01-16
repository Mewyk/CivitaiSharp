namespace CivitaiSharp.Sdk.Models.Results;

using System;
using System.Text.Json.Serialization;

/// <summary>
/// Queue position information for a job on a specific provider.
/// </summary>
public sealed record ProviderJobQueuePosition(
    [property: JsonPropertyName("precedingJobs")] int? PrecedingJobs,
    [property: JsonPropertyName("precedingCost")] decimal? PrecedingCost,
    [property: JsonPropertyName("throughputRate")] decimal? ThroughputRate,
    [property: JsonPropertyName("workerId")] string? WorkerId,
    [property: JsonPropertyName("estimatedStartDuration")] TimeSpanDetails? EstimatedStartDuration,
    [property: JsonPropertyName("estimatedStartDate")] DateTime? EstimatedStartDate);
