namespace CivitaiSharp.Core.Models;

/// <summary>
/// Availability status of a model on the Civitai platform.
/// </summary>
/// <remarks>
/// API string mappings are defined in <see cref="Extensions.ApiStringRegistry"/>.
/// </remarks>
public enum Availability
{
    /// <summary>
    /// The model is publicly available and visible.
    /// Maps to API value "Public".
    /// </summary>
    Public,

    /// <summary>
    /// The model is private and not visible to other users.
    /// Maps to API value "Private".
    /// </summary>
    Private,

    /// <summary>
    /// The model has been archived and is no longer actively maintained.
    /// Maps to API value "Archived".
    /// </summary>
    Archived,

    /// <summary>
    /// The model is not searchable but may still be accessible via direct link.
    /// Maps to API value "Unsearchable".
    /// </summary>
    Unsearchable
}
