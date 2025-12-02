namespace CivitaiSharp.Core.Models;

using System.Text.Json.Serialization;

/// <summary>
/// Information about the creator of a model or other resource.
/// </summary>
/// <param name="Username">The username of the creator. Maps to JSON property "username".</param>
/// <param name="ModelCount">The total number of models created by this user. Maps to JSON property "modelCount".</param>
/// <param name="Link">URL to retrieve all models created by this user. Maps to JSON property "link".</param>
/// <param name="Image">URL of the creator's avatar image. Maps to JSON property "image".</param>
public sealed record Creator(
    [property: JsonPropertyName("username")] string Username,
    [property: JsonPropertyName("modelCount")] int? ModelCount = null,
    [property: JsonPropertyName("link")] string? Link = null,
    [property: JsonPropertyName("image")] string? Image = null);
