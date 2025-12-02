namespace CivitaiSharp.Core.Models;

/// <summary>
/// Sorting options for model queries.
/// </summary>
/// <remarks>
/// <para>
/// These values represent complete sort options with implied sort directions, not arbitrary field names.
/// The Civitai API does not support toggling ascending/descending independently. For example,
/// "Newest" implies descending by date, and "Most Downloaded" implies descending by download count.
/// </para>
/// <para>
/// API string mappings are defined in <see cref="Extensions.ApiStringRegistry"/>.
/// </para>
/// </remarks>
public enum ModelSort
{
    /// <summary>
    /// Sort by user rating with highest rated models first (descending).
    /// Maps to API value "Highest Rated".
    /// </summary>
    HighestRated,

    /// <summary>
    /// Sort by download count with most downloaded models first (descending).
    /// Maps to API value "Most Downloaded".
    /// </summary>
    MostDownloaded,

    /// <summary>
    /// Sort by creation date with newest models first (descending).
    /// Maps to API value "Newest".
    /// </summary>
    Newest
}
