namespace CivitaiSharp.Core.Models;

using System.Text.Json.Serialization;

/// <summary>
/// An image associated with a model version (for gallery/examples).
/// When fetched via /model-versions/:id endpoint, some fields like Id may be absent.
/// </summary>
/// <param name="Id">The unique identifier for the image. Maps to JSON property "id".</param>
/// <param name="Url">The URL where the image is hosted. Maps to JSON property "url".</param>
/// <param name="NsfwLevel">The NSFW content level of the image on a numeric scale. Maps to JSON property "nsfwLevel".</param>
/// <param name="Width">The original width of the image in pixels. Maps to JSON property "width".</param>
/// <param name="Height">The original height of the image in pixels. Maps to JSON property "height".</param>
/// <param name="Hash">The blurhash of the image for placeholder generation. Maps to JSON property "hash".</param>
/// <param name="Type">The media type of the image. Maps to JSON property "type".</param>
/// <param name="Minor">Indicates whether the image contains a minor. Maps to JSON property "minor".</param>
/// <param name="IsPersonOfInterest">Indicates whether the image is of a person of interest. Maps to JSON property "poi".</param>
/// <param name="HasMetadata">Indicates whether the image contains generation metadata. Maps to JSON property "hasMeta".</param>
/// <param name="HasPositivePrompt">Indicates whether the image has a positive prompt. Maps to JSON property "hasPositivePrompt".</param>
/// <param name="OnSite">Indicates whether the image was generated on Civitai's servers. Maps to JSON property "onSite".</param>
/// <param name="RemixOfId">The unique identifier of the image this was remixed from. Maps to JSON property "remixOfId".</param>
/// <param name="Availability">The availability status of the image. Maps to JSON property "availability".</param>
/// <param name="Metadata">File properties of the image including dimensions and hash. Maps to JSON property "metadata".</param>
/// <param name="Meta">Generation metadata and parameters used to create this image. Maps to JSON property "meta".</param>
public sealed record ModelVersionImage(
    [property: JsonPropertyName("id")] long? Id,
    [property: JsonPropertyName("url")] string Url,
    [property: JsonPropertyName("nsfwLevel")] int NsfwLevel,
    [property: JsonPropertyName("width")] int Width,
    [property: JsonPropertyName("height")] int Height,
    [property: JsonPropertyName("hash")] string Hash,
    [property: JsonPropertyName("type")] MediaType Type,
    [property: JsonPropertyName("minor")] bool Minor,
    [property: JsonPropertyName("poi")] bool IsPersonOfInterest,
    [property: JsonPropertyName("hasMeta")] bool HasMetadata,
    [property: JsonPropertyName("hasPositivePrompt")] bool HasPositivePrompt,
    [property: JsonPropertyName("onSite")] bool OnSite,
    [property: JsonPropertyName("remixOfId")] long? RemixOfId,
    [property: JsonPropertyName("availability")] Availability? Availability,
    [property: JsonPropertyName("metadata")] ModelVersionImageFile? Metadata,
    [property: JsonPropertyName("meta")] ImageMeta? Meta);
