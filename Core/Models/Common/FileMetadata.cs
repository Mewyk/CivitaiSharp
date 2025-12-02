namespace CivitaiSharp.Core.Models.Common;

using System.Text.Json.Serialization;

/// <summary>
/// Metadata for a model file including format and precision information.
/// </summary>
/// <param name="Format">The model file format (e.g., "SafeTensor", "PickleTensor", "Other"). Maps to JSON property "format".</param>
/// <param name="Size">The model size specification (e.g., "full", "pruned"). Maps to JSON property "size".</param>
/// <param name="Precision">The floating point precision of the model (e.g., "fp16", "fp32"). Maps to JSON property "fp".</param>
public sealed record FileMetadata(
    [property: JsonPropertyName("format")] string? Format = null,
    [property: JsonPropertyName("size")] string? Size = null,
    [property: JsonPropertyName("fp")] string? Precision = null);
