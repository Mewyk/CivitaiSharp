namespace CivitaiSharp.Core.Models;

/// <summary>
/// Commercial use permissions used by model queries.
/// </summary>
/// <remarks>
/// API string mappings are defined in <see cref="Extensions.ApiStringRegistry"/>.
/// </remarks>
public enum CommercialUsePermission
{
    /// <summary>
    /// No commercial use is permitted.
    /// Maps to API value "None".
    /// </summary>
    None,

    /// <summary>
    /// Commercial use is allowed for generated images only.
    /// Maps to API value "Image".
    /// </summary>
    Image,

    /// <summary>
    /// Commercial use is allowed for renting the model (e.g., on generation services).
    /// Maps to API value "Rent".
    /// </summary>
    Rent,

    /// <summary>
    /// Commercial use is allowed for renting on the Civitai platform specifically.
    /// Maps to API value "RentCivit".
    /// </summary>
    RentCivit,

    /// <summary>
    /// Commercial use is allowed including selling products made with the model.
    /// Maps to API value "Sell".
    /// </summary>
    Sell
}
