namespace CivitaiSharp.Sdk.Enums;

using System.Text.Json.Serialization;
using CivitaiSharp.Sdk.Json.Converters;

/// <summary>
/// Indicates whether a provider supports and can currently execute a given job.
/// </summary>
[JsonConverter(typeof(JobSupportConverter))]
public enum JobSupport
{
    /// <summary>The provider does not support this job type.</summary>
    Unsupported = 0,

    /// <summary>The provider supports the job type but is currently unable to run it.</summary>
    Unavailable,

    /// <summary>The provider supports the job type and is currently able to run it.</summary>
    Available
}
