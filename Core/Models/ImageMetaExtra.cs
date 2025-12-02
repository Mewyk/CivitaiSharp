namespace CivitaiSharp.Core.Models;

using System.Text.Json.Serialization;

/// <summary>
/// Extra metadata for an image, such as remix information.
/// </summary>
/// <param name="RemixOfId">The unique identifier of the image this was remixed from. Maps to JSON property "remixOfId".</param>
public sealed record ImageMetaExtra(
    [property: JsonPropertyName("remixOfId")] long? RemixOfId);
