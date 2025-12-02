namespace CivitaiSharp.Sdk.Enums;

/// <summary>
/// Availability status of a model or asset on the generation infrastructure.
/// </summary>
/// <remarks>
/// API string mappings are defined in <see cref="Extensions.SdkApiStringRegistry"/>.
/// </remarks>
public enum AvailabilityStatus
{
    /// <summary>
    /// The model is available and ready for generation.
    /// Maps to API value "Available".
    /// </summary>
    Available,

    /// <summary>
    /// The model is not currently available.
    /// Maps to API value "Unavailable".
    /// </summary>
    Unavailable,

    /// <summary>
    /// The model is available but with limited capacity.
    /// Generation may experience delays.
    /// Maps to API value "Degraded".
    /// </summary>
    Degraded
}
