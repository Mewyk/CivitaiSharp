namespace CivitaiSharp.Core.Models;

using System.Text.Json.Serialization;

/// <summary>
/// File properties of an image in a model version gallery including dimensions and perceptual hash.
/// </summary>
/// <param name="Hash">The blurhash of the image for placeholder generation. Maps to JSON property "hash".</param>
/// <param name="Width">The original width of the image in pixels. Maps to JSON property "width".</param>
/// <param name="Height">The original height of the image in pixels. Maps to JSON property "height".</param>
/// <param name="Size">The file size of the image in bytes. Maps to JSON property "size".</param>
public sealed record ModelVersionImageFile(
    [property: JsonPropertyName("hash")] string? Hash,
    [property: JsonPropertyName("width")] int? Width,
    [property: JsonPropertyName("height")] int? Height,
    [property: JsonPropertyName("size")] long? Size);
