namespace CivitaiSharp.Core.Models;

/// <summary>
/// Sorting options for image queries.
/// </summary>
/// <remarks>
/// <para>
/// These values represent complete sort options with implied sort directions, not arbitrary field names.
/// The Civitai API does not support toggling ascending/descending independently. For example, 
/// "Newest" implies descending by date, and "Most Reactions" implies descending by reaction count.
/// </para>
/// <para>
/// API string mappings are defined in <see cref="Extensions.ApiStringRegistry"/>.
/// </para>
/// </remarks>
public enum ImageSort
{
    /// <summary>
    /// Sort by the total number of reactions received (descending).
    /// Maps to API value "Most Reactions".
    /// </summary>
    MostReactions,

    /// <summary>
    /// Sort by the number of comments and discussions (descending).
    /// Maps to API value "Most Comments".
    /// </summary>
    MostComments,

    /// <summary>
    /// Sort by the number of times images have been collected (descending).
    /// Maps to API value "Most Collected".
    /// </summary>
    MostCollected,

    /// <summary>
    /// Sort by creation date with newest first (descending).
    /// Maps to API value "Newest".
    /// </summary>
    Newest,

    /// <summary>
    /// Sort by creation date with oldest first (ascending).
    /// Maps to API value "Oldest".
    /// </summary>
    Oldest,

    /// <summary>
    /// Random order.
    /// Maps to API value "Random".
    /// </summary>
    Random
}
