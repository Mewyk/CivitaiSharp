namespace CivitaiSharp.Sdk.Enums;

using System.Text.Json.Serialization;
using CivitaiSharp.Sdk.Json.Converters;

/// <summary>
/// Represents the type of event that last occurred for a job.
/// These values are returned by the Civitai orchestration API.
/// </summary>
[JsonConverter(typeof(JobEventTypeConverter))]
public enum JobEventType
{
    /// <summary>Job was created/initialized.</summary>
    Initialized = 0,

    /// <summary>Job was claimed by a worker/provider.</summary>
    Claimed,

    /// <summary>Job was rejected before execution.</summary>
    Rejected,

    /// <summary>Job was rejected after it was already claimed.</summary>
    LateRejected,

    /// <summary>Job claim expired before execution could continue.</summary>
    ClaimExpired,

    /// <summary>Job state was updated (progress event).</summary>
    Updated,

    /// <summary>Job execution failed.</summary>
    Failed,

    /// <summary>Job execution succeeded.</summary>
    Succeeded,

    /// <summary>Job expired before completion.</summary>
    Expired,

    /// <summary>Job was deleted/cancelled server-side.</summary>
    Deleted
}
