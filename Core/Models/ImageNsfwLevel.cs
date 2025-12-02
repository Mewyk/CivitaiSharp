namespace CivitaiSharp.Core.Models;

/// <summary>
/// NSFW levels for images used in API queries.
/// </summary>
/// <remarks>
/// API string mappings are defined in <see cref="Extensions.ApiStringRegistry"/>.
/// </remarks>
public enum ImageNsfwLevel
{
    /// <summary>
    /// Safe for work content with no adult themes.
    /// Maps to API value "None".
    /// </summary>
    None,

    /// <summary>
    /// Mildly suggestive content but not explicit.
    /// Maps to API value "Soft".
    /// </summary>
    Soft,

    /// <summary>
    /// Adult-oriented content that may be explicit.
    /// Maps to API value "Mature".
    /// </summary>
    Mature,

    /// <summary>
    /// Explicit adult content.
    /// Maps to API value "X".
    /// </summary>
    Explicit
}
