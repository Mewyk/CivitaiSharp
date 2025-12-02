namespace CivitaiSharp.Core.Models;

using System.Text.Json.Serialization;

/// <summary>
/// A Civitai resource reference used in image generation metadata.
/// </summary>
/// <param name="Type">The type of Civitai resource (e.g., "model", "lora"). Maps to JSON property "type".</param>
/// <param name="ModelVersionId">The unique identifier of the model version used in generation. Maps to JSON property "modelVersionId".</param>
/// <param name="ModelVersionName">The human-readable name of the model version. Maps to JSON property "modelVersionName".</param>
/// <param name="Weight">The weight or influence of the resource in the generation. Maps to JSON property "weight".</param>
/// <param name="Strength">The strength or intensity at which the resource was applied. Maps to JSON property "strength".</param>
public sealed record CivitaiResource(
    [property: JsonPropertyName("type")] string? Type,
    [property: JsonPropertyName("modelVersionId")] long? ModelVersionId,
    [property: JsonPropertyName("modelVersionName")] string? ModelVersionName,
    [property: JsonPropertyName("weight")] decimal? Weight,
    [property: JsonPropertyName("strength")] decimal? Strength);
