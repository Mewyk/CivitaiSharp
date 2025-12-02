namespace CivitaiSharp.Core.Models;

/// <summary>
/// Media type of a resource (image or video).
/// </summary>
/// <remarks>
/// API string mappings are defined in <see cref="Extensions.ApiStringRegistry"/>.
/// </remarks>
public enum MediaType
{
    /// <summary>
    /// Static image media.
    /// Maps to API value "image".
    /// </summary>
    Image,

    /// <summary>
    /// Video media.
    /// Maps to API value "video".
    /// </summary>
    Video
}
