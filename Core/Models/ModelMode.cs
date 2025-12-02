namespace CivitaiSharp.Core.Models;

/// <summary>
/// Mode or status state of a model.
/// </summary>
/// <remarks>
/// API string mappings are defined in <see cref="Extensions.ApiStringRegistry"/>.
/// </remarks>
public enum ModelMode
{
    /// <summary>
    /// The model has been archived.
    /// Maps to API value "Archived".
    /// </summary>
    Archived,

    /// <summary>
    /// The model has been taken down and is no longer available.
    /// Maps to API value "TakenDown".
    /// </summary>
    TakenDown
}
