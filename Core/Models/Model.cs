namespace CivitaiSharp.Core.Models;

using System.Collections.Generic;
using System.Text.Json.Serialization;

/// <summary>
/// Model returned by the public API. This type mirrors the commonly returned
/// fields from the /models endpoints including nested information such as
/// creator, statistics and model versions so callers can inspect returned JSON
/// without losing data.
/// </summary>
/// <param name="Id">The unique identifier for the model. Maps to JSON property "id".</param>
/// <param name="Name">The name of the model. Maps to JSON property "name".</param>
/// <param name="Description">The description of the model formatted as HTML. Maps to JSON property "description".</param>
/// <param name="Type">The model type (e.g., "Checkpoint", "TextualInversion", "LORA", "Controlnet"). Maps to JSON property "type".</param>
/// <param name="IsNsfw">Indicates whether the model is not safe for work content. Maps to JSON property "nsfw".</param>
/// <param name="NsfwLevel">The NSFW content level of the model on a numeric scale. Maps to JSON property "nsfwLevel".</param>
/// <param name="Tags">Array of tags associated with the model for categorization. Maps to JSON property "tags".</param>
/// <param name="Creator">Information about the creator of this model. Maps to JSON property "creator".</param>
/// <param name="Stats">Statistical information about user interactions with this model. Maps to JSON property "stats".</param>
/// <param name="ModelVersions">List of all versions of this model. Maps to JSON property "modelVersions".</param>
/// <param name="AllowNoCredit">Indicates whether the model allows use without giving credit to the creator. Maps to JSON property "allowNoCredit".</param>
/// <param name="AllowDerivatives">Indicates whether the model allows creation of derivative works. Maps to JSON property "allowDerivatives".</param>
/// <param name="AllowDifferentLicense">Indicates whether derivatives of this model are allowed to have a different license. Maps to JSON property "allowDifferentLicense".</param>
/// <param name="AllowCommercialUse">Array of commercial use permission types (e.g., "Image", "Rent", "Sell"). Maps to JSON property "allowCommercialUse".</param>
/// <param name="IsPersonOfInterest">Indicates whether the model is of a person of interest. Maps to JSON property "poi".</param>
/// <param name="Minor">Indicates whether the model involves a minor. Maps to JSON property "minor".</param>
/// <param name="IsSafeForWorkOnly">Indicates whether the model is safe for work content only. Maps to JSON property "sfwOnly".</param>
/// <param name="Availability">The availability status of the model (e.g., "Public", "Archived"). Maps to JSON property "availability".</param>
/// <param name="Cosmetic">Cosmetic information or display metadata for the model. Maps to JSON property "cosmetic".</param>
/// <param name="SupportsGeneration">Indicates whether this model supports generation capabilities. Maps to JSON property "supportsGeneration".</param>
/// <param name="UserId">The user ID of the creator who uploaded this model. Maps to JSON property "userId".</param>
/// <param name="DownloadUrl">Convenience download URL for the model. Maps to JSON property "downloadUrl".</param>
/// <param name="Mode">The current mode of the model (e.g., "Archived", "TakenDown"). Maps to JSON property "mode".</param>
public sealed record Model(
    [property: JsonPropertyName("id")] long Id,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("description")] string? Description,
    [property: JsonPropertyName("type")] ModelType Type,
    [property: JsonPropertyName("nsfw")] bool IsNsfw,
    [property: JsonPropertyName("nsfwLevel")] int NsfwLevel,
    [property: JsonPropertyName("tags")] IReadOnlyList<string>? Tags,
    [property: JsonPropertyName("creator")] Creator? Creator,
    [property: JsonPropertyName("stats")] ModelStats? Stats,
    [property: JsonPropertyName("modelVersions")] IReadOnlyList<ModelVersion>? ModelVersions,
    [property: JsonPropertyName("allowNoCredit")] bool AllowNoCredit,
    [property: JsonPropertyName("allowDerivatives")] bool AllowDerivatives,
    [property: JsonPropertyName("allowDifferentLicense")] bool AllowDifferentLicense,
    [property: JsonPropertyName("allowCommercialUse")] IReadOnlyList<CommercialUsePermission>? AllowCommercialUse,
    [property: JsonPropertyName("poi")] bool IsPersonOfInterest,
    [property: JsonPropertyName("minor")] bool Minor,
    [property: JsonPropertyName("sfwOnly")] bool IsSafeForWorkOnly,
    [property: JsonPropertyName("availability")] Availability? Availability,
    [property: JsonPropertyName("cosmetic")] string? Cosmetic,
    [property: JsonPropertyName("supportsGeneration")] bool SupportsGeneration,
    [property: JsonPropertyName("userId")] long? UserId,
    [property: JsonPropertyName("downloadUrl")] string? DownloadUrl,
    [property: JsonPropertyName("mode")] ModelMode? Mode);
