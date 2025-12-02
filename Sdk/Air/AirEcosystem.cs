namespace CivitaiSharp.Sdk.Air;

/// <summary>
/// Supported model ecosystems in the AIR (Artificial Intelligence Resource) identifier system.
/// </summary>
/// <remarks>
/// API string mappings are defined in <see cref="Extensions.SdkApiStringRegistry"/>.
/// </remarks>
public enum AirEcosystem
{
    /// <summary>
    /// Stable Diffusion 1.x architecture (e.g., SD 1.5).
    /// Maps to API value "sd1".
    /// </summary>
    StableDiffusion1,

    /// <summary>
    /// Stable Diffusion 2.x architecture.
    /// Maps to API value "sd2".
    /// </summary>
    StableDiffusion2,

    /// <summary>
    /// Stable Diffusion XL architecture.
    /// Maps to API value "sdxl".
    /// </summary>
    StableDiffusionXl,

    /// <summary>
    /// FLUX.1 architecture.
    /// Maps to API value "flux1".
    /// </summary>
    Flux1,

    /// <summary>
    /// Pony Diffusion architecture.
    /// Maps to API value "pony".
    /// </summary>
    Pony
}
