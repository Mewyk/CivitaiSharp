namespace CivitaiSharp.Sdk.Models.Results;

using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using CivitaiSharp.Sdk.Enums;

/// <summary>
/// Represents a single event in the lifecycle of a job.
/// Check the <see cref="Type"/> property to determine job outcome (Succeeded, Failed, etc.).
/// The API does not provide an ErrorMessage property - use <see cref="Context"/> for error details when <see cref="Type"/> is Failed.
/// </summary>
public sealed record JobEvent(
    [property: JsonPropertyName("jobId")] Guid? JobId,
    [property: JsonRequired, JsonPropertyName("type")] JobEventType Type,
    [property: JsonPropertyName("dateTime")] DateTime? DateTime,
    [property: JsonRequired, JsonPropertyName("provider")] Provider Provider,
    [property: JsonPropertyName("workerId")] string? WorkerId,
    [property: JsonPropertyName("context")] IReadOnlyDictionary<string, JsonElement>? Context,
    [property: JsonPropertyName("claimDuration")] TimeSpanDetails? ClaimDuration,
    [property: JsonPropertyName("jobDuration")] TimeSpanDetails? JobDuration,
    [property: JsonPropertyName("retryAttempt")] int? RetryAttempt,
    [property: JsonPropertyName("cost")] decimal? Cost,
    [property: JsonPropertyName("jobProperties")] IReadOnlyDictionary<string, JsonElement>? JobProperties,
    [property: JsonPropertyName("jobType")] string? JobType,
    [property: JsonPropertyName("jobPriority")] int? JobPriority,
    [property: JsonPropertyName("claimHasCompleted")] bool? ClaimHasCompleted,
    [property: JsonPropertyName("jobHasCompleted")] bool? JobHasCompleted);
