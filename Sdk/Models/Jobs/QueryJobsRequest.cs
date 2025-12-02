namespace CivitaiSharp.Sdk.Models.Jobs;

using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

/// <summary>
/// Request model for querying jobs by custom properties.
/// </summary>
public sealed class QueryJobsRequest
{
    /// <summary>
    /// Gets or sets the properties to match. All specified properties must match exactly.
    /// </summary>
    /// <remarks>
    /// Properties can contain any JSON-serializable values. Use <see cref="JsonElement"/>
    /// to preserve AOT compatibility while supporting arbitrary value types.
    /// </remarks>
    [JsonPropertyName("properties")]
    public required IReadOnlyDictionary<string, JsonElement> Properties { get; init; }
}
