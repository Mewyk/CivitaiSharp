namespace CivitaiSharp.Core.Models;

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

/// <summary>
/// Image returned by the public API.
/// </summary>
/// <param name="Id">The unique identifier for the image. Maps to JSON property "id".</param>
/// <param name="Url">The URL of the image at its source resolution. Maps to JSON property "url".</param>
/// <param name="Hash">The blurhash of the image for placeholder generation. Maps to JSON property "hash".</param>
/// <param name="Width">The width of the image in pixels. Maps to JSON property "width".</param>
/// <param name="Height">The height of the image in pixels. Maps to JSON property "height".</param>
/// <param name="NsfwLevel">The NSFW content level of the image (e.g., "None", "Soft", "Mature", "X"). Maps to JSON property "nsfwLevel".</param>
/// <param name="Type">The type of media represented by this image (e.g., "image"). Maps to JSON property "type".</param>
/// <param name="IsNsfw">Indicates whether the image has any mature content labels applied. Maps to JSON property "nsfw".</param>
/// <param name="BrowsingLevel">The browsing level threshold for viewing this image. Maps to JSON property "browsingLevel".</param>
/// <param name="CreatedAt">The date and time when the image was posted. Maps to JSON property "createdAt".</param>
/// <param name="PostId">The unique identifier of the post this image belongs to. Maps to JSON property "postId".</param>
/// <param name="Stats">Statistical information about user reactions to this image. Maps to JSON property "stats".</param>
/// <param name="Meta">Generation metadata and parameters used to create this image. Maps to JSON property "meta".</param>
/// <param name="Username">The username of the creator who posted this image. Maps to JSON property "username".</param>
/// <param name="BaseModel">The base model used for image generation (e.g., "Pony", "Flux.1 D", "SDXL 1.0"). Maps to JSON property "baseModel".</param>
/// <param name="ModelVersionIds">Array of model version IDs used to generate this image. Maps to JSON property "modelVersionIds".</param>
public sealed record Image(
    [property: JsonPropertyName("id")] long Id,
    [property: JsonPropertyName("url")] string Url,
    [property: JsonPropertyName("hash")] string? Hash,
    [property: JsonPropertyName("width")] int Width,
    [property: JsonPropertyName("height")] int Height,
    [property: JsonPropertyName("nsfwLevel")] ImageNsfwLevel? NsfwLevel,
    [property: JsonPropertyName("type")] MediaType? Type,
    [property: JsonPropertyName("nsfw")] bool? IsNsfw,
    [property: JsonPropertyName("browsingLevel")] int? BrowsingLevel,
    [property: JsonPropertyName("createdAt")] DateTime? CreatedAt,
    [property: JsonPropertyName("postId")] long? PostId,
    [property: JsonPropertyName("stats")] ImageStats? Stats,
    [property: JsonPropertyName("meta")] ImageMeta? Meta,
    [property: JsonPropertyName("username")] string? Username,
    [property: JsonPropertyName("baseModel")] string? BaseModel,
    [property: JsonPropertyName("modelVersionIds")] IReadOnlyList<long>? ModelVersionIds);
