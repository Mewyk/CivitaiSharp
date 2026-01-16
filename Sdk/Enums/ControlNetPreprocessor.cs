namespace CivitaiSharp.Sdk.Enums;

/// <summary>
/// ControlNet preprocessor types for image guidance.
/// </summary>
/// <remarks>
/// API string mappings are defined in <see cref="Extensions.SdkApiStringRegistry"/>.
/// </remarks>
public enum ControlNetPreprocessor
{
    /// <summary>
    /// Canny edge detection preprocessor.
    /// Maps to API value "Canny".
    /// </summary>
    Canny,

    /// <summary>
    /// Depth (Zoe) estimation preprocessor.
    /// Maps to API value "DepthZoe".
    /// </summary>
    DepthZoe,

    /// <summary>
    /// PiDiNet soft edge preprocessor.
    /// Maps to API value "SoftedgePidinet".
    /// </summary>
    SoftEdgePidinet,

    /// <summary>
    /// Background removal preprocessor for subject isolation.
    /// Maps to API value "Rembg".
    /// </summary>
    Rembg,
}
