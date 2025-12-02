namespace CivitaiSharp.Core.Models;

using System.Text.Json.Serialization;

/// <summary>
/// A resource reference in image generation metadata (different from CivitaiResource).
/// This represents resources embedded in the generation workflow, typically from A1111.
/// </summary>
/// <param name="Name">The name of the resource used in generation. Maps to JSON property "name".</param>
/// <param name="Type">The type of resource (e.g., "model", "lora", "embeddings"). Maps to JSON property "type".</param>
/// <param name="Hash">The hash value identifying the resource. Maps to JSON property "hash".</param>
/// <param name="Weight">The weight or strength at which this resource was applied during generation. Maps to JSON property "weight".</param>
public sealed record ImageMetaResource(
    [property: JsonPropertyName("name")] string? Name,
    [property: JsonPropertyName("type")] string? Type,
    [property: JsonPropertyName("hash")] string? Hash,
    [property: JsonPropertyName("weight")] decimal? Weight);
