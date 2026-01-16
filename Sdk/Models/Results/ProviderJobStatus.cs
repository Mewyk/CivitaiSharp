namespace CivitaiSharp.Sdk.Models.Results;

using System.Text.Json.Serialization;
using CivitaiSharp.Sdk.Enums;

/// <summary>
/// Provider-specific information about whether a job is supported and, if queued, where it is.
/// </summary>
public sealed record ProviderJobStatus(
    [property: JsonRequired, JsonPropertyName("support")] JobSupport Support,
    [property: JsonPropertyName("queuePosition")] ProviderJobQueuePosition? QueuePosition);
