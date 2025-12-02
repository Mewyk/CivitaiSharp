namespace CivitaiSharp.Sdk.Models.Results;

using System.Collections.Generic;
using System.Text.Json.Serialization;

/// <summary>
/// Collection of job statuses returned from job submission or status queries.
/// </summary>
/// <param name="Token">
/// The batch token for polling or bulk operations. Maps to JSON property "token".
/// This is a Base64-encoded identifier that can be used with GetByToken, CancelByToken, or TaintByToken.
/// </param>
/// <param name="Jobs">The list of job statuses. Maps to JSON property "jobs".</param>
public sealed record JobStatusCollection(
    [property: JsonPropertyName("token")] string? Token = null,
    [property: JsonPropertyName("jobs")] IReadOnlyList<JobStatus>? Jobs = null)
{
    /// <summary>
    /// An empty list used to avoid allocations when <see cref="Jobs"/> is null.
    /// </summary>
    private static readonly IReadOnlyList<JobStatus> EmptyJobsList = [];

    /// <summary>
    /// Gets the list of job statuses, or an empty list if none.
    /// </summary>
    [JsonIgnore]
    public IReadOnlyList<JobStatus> JobsList => Jobs ?? EmptyJobsList;
}
