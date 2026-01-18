namespace CivitaiSharp.Sdk.Models.Results;

using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using CivitaiSharp.Sdk.Json;
using System.Collections.ObjectModel;

/// <summary>
/// Status information for an individual job.
/// </summary>
/// <param name="JobId">The unique job identifier. Maps to JSON property "jobId".</param>
/// <param name="Cost">The Buzz cost incurred for this job. Maps to JSON property "cost".</param>
/// <param name="Result">The result information for this job. Maps to JSON property "result".</param>
/// <param name="Scheduled">
/// Indicates whether the job is still being processed. Maps to JSON property "scheduled".
/// When <see langword="true"/>, the job is queued or processing.
/// When <see langword="false"/>, the job is complete - check <see cref="LastEvent"/> to determine success or failure.
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
/// <param name="Job">
/// Optional job definition payload. Maps to JSON property "job".
/// This is returned when queries are performed with the "detailed" flag.
/// </param>
/// <param name="LastEvent">
/// The most recent job lifecycle event. Maps to JSON property "lastEvent".
/// Check <see cref="JobEvent.Type"/> to determine if the job succeeded or failed.
/// The API does not provide an ErrorMessage property - use <see cref="JobEvent.Context"/> for error details.
/// </param>
public sealed record JobStatus(
    [property: JsonPropertyName("jobId")] Guid JobId,
    [property: JsonPropertyName("cost")] decimal Cost,
    [property: JsonPropertyName("result")] JobResult? Result,
    [property: JsonPropertyName("scheduled")] bool Scheduled,
    [property: JsonPropertyName("properties")] IReadOnlyDictionary<string, JsonElement>? Properties,
    [property: JsonPropertyName("serviceProviders")] JsonElement? ServiceProviders,
    [property: JsonPropertyName("position")] int? Position,
    [property: JsonPropertyName("job")] JsonElement? Job = null,
    [property: JsonPropertyName("lastEvent")] JobEvent? LastEvent = null)
{
    private static readonly IReadOnlyDictionary<string, ProviderJobStatus> EmptyProviderStatuses
        = ReadOnlyDictionary<string, ProviderJobStatus>.Empty;

    /// <summary>
    /// Attempts to parse <see cref="ServiceProviders"/> into a strongly-typed dictionary.
    /// </summary>
    /// <remarks>
    /// The official OpenAPI schema models <c>serviceProviders</c> as a mapping of provider name to provider-specific status.
    /// This helper returns an empty dictionary when the payload is missing or not an object.
    /// If the payload is present but cannot be parsed, a <see cref="JsonException"/> is thrown.
    /// </remarks>
    [JsonIgnore]
    public IReadOnlyDictionary<string, ProviderJobStatus> ServiceProviderStatuses
    {
        get
        {
            if (ServiceProviders is not { } providersElement || providersElement.ValueKind != JsonValueKind.Object)
            {
                return EmptyProviderStatuses;
            }

            // Deserialize via source-generated type info to remain AOT-compatible.
            // This is intentionally strict: invalid provider payloads should fail loudly.
            return JsonSerializer.Deserialize(
                providersElement.GetRawText(),
                SdkJsonContext.Default.IReadOnlyDictionaryStringProviderJobStatus)
                ?? throw new JsonException("Failed to deserialize job serviceProviders into a provider status dictionary.");
        }
    }
}
