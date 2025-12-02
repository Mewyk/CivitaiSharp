namespace CivitaiSharp.Sdk.Models.Results;

using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

/// <summary>
/// Status information for an individual job.
/// </summary>
/// <param name="JobId">The unique job identifier. Maps to JSON property "jobId".</param>
/// <param name="Cost">The Buzz cost incurred for this job. Maps to JSON property "cost".</param>
/// <param name="Result">The result information for this job. Maps to JSON property "result".</param>
/// <param name="Scheduled">
/// Indicates whether the job is still being processed. Maps to JSON property "scheduled".
/// When true, the job is queued or processing. When false, the job is complete or failed.
/// </param>
/// <param name="Properties">
/// The custom properties from the original request. Maps to JSON property "properties".
/// Values are <see cref="JsonElement"/> because properties can contain any JSON type.
/// Use <see cref="JsonElement.TryGetProperty(string, out JsonElement)"/> and type accessors to extract values.
/// </param>
/// <param name="ServiceProviders">
/// Information about the service providers handling this job. Maps to JSON property "serviceProviders".
/// This property uses <see cref="JsonElement"/> because the API returns variable structures
/// for service provider information. Use <see cref="JsonElement.TryGetProperty(string, out JsonElement)"/> to safely access nested values.
/// </param>
/// <param name="Position">The queue position when the job is waiting. Maps to JSON property "position".</param>
public sealed record JobStatus(
    [property: JsonPropertyName("jobId")] Guid JobId,
    [property: JsonPropertyName("cost")] decimal Cost,
    [property: JsonPropertyName("result")] JobResult? Result,
    [property: JsonPropertyName("scheduled")] bool Scheduled,
    [property: JsonPropertyName("properties")] IReadOnlyDictionary<string, JsonElement>? Properties,
    [property: JsonPropertyName("serviceProviders")] JsonElement? ServiceProviders,
    [property: JsonPropertyName("position")] int? Position);
