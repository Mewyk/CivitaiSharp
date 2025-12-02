namespace CivitaiSharp.Core.Models;

using System.Text.Json.Serialization;

/// <summary>
/// Abbreviated model information included in a standalone model version response.
/// </summary>
/// <param name="Name">The name of the model. Maps to JSON property "name".</param>
/// <param name="Type">The model type. Maps to JSON property "type".</param>
/// <param name="IsNsfw">Indicates whether the model is not safe for work content. Maps to JSON property "nsfw".</param>
/// <param name="IsPersonOfInterest">Indicates whether the model is of a person of interest. Maps to JSON property "poi".</param>
public sealed record ModelVersionModel(
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("type")] ModelType Type,
    [property: JsonPropertyName("nsfw")] bool IsNsfw,
    [property: JsonPropertyName("poi")] bool IsPersonOfInterest);
