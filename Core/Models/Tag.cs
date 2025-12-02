namespace CivitaiSharp.Core.Models;

using System.Text.Json.Serialization;

/// <summary>
/// Tag returned by the public API.
/// </summary>
/// <param name="Name">The name of the tag. Maps to JSON property "name".</param>
/// <param name="Link">URL to retrieve all models associated with this tag. Maps to JSON property "link".</param>
public sealed record Tag(
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("link")] string Link);
