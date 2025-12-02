namespace CivitaiSharp.Sdk.Enums;

/// <summary>
/// Sampling algorithms (schedulers) for image generation.
/// </summary>
/// <remarks>
/// <para>
/// Different schedulers provide varying trade-offs between generation speed and image quality.
/// The recommended scheduler depends on the base model being used.
/// </para>
/// <para>
/// API string mappings are defined in <see cref="Extensions.SdkApiStringRegistry"/>.
/// </para>
/// </remarks>
public enum Scheduler
{
    /// <summary>
    /// Euler method sampler. Fast with good quality.
    /// Maps to API value "euler".
    /// </summary>
    Euler,

    /// <summary>
    /// Euler Ancestral sampler. Fast with good quality, adds stochasticity.
    /// Maps to API value "euler_a".
    /// </summary>
    EulerAncestral,

    /// <summary>
    /// Linear Multistep sampler. Medium speed with good quality.
    /// Maps to API value "lms".
    /// </summary>
    LinearMultistep,

    /// <summary>
    /// Heun's method sampler. Slower but higher quality.
    /// Maps to API value "heun".
    /// </summary>
    Heun,

    /// <summary>
    /// DPM-Solver-2 sampler. Medium speed with high quality.
    /// Maps to API value "dpm_2".
    /// </summary>
    DpmSolver2,

    /// <summary>
    /// DPM-Solver-2 Ancestral sampler. Medium speed with high quality, adds stochasticity.
    /// Maps to API value "dpm_2_a".
    /// </summary>
    DpmSolver2Ancestral,

    /// <summary>
    /// DPM++ 2S Ancestral sampler. Medium speed with high quality.
    /// Maps to API value "dpmpp_2s_a".
    /// </summary>
    DpmPlusPlus2SAncestral,

    /// <summary>
    /// DPM++ 2M sampler. Fast with high quality.
    /// Maps to API value "dpmpp_2m".
    /// </summary>
    DpmPlusPlus2M,

    /// <summary>
    /// DPM++ SDE sampler. Slower with very high quality.
    /// Maps to API value "dpmpp_sde".
    /// </summary>
    DpmPlusPlusSde,

    /// <summary>
    /// DPM++ 2M SDE sampler. Medium speed with very high quality.
    /// Maps to API value "dpmpp_2m_sde".
    /// </summary>
    DpmPlusPlus2MSde,

    /// <summary>
    /// DPM++ 2M SDE Karras sampler. Medium speed with very high quality.
    /// Recommended for SDXL models.
    /// Maps to API value "dpmpp_2m_sde_karras".
    /// </summary>
    DpmPlusPlus2MSdeKarras,

    /// <summary>
    /// DPM++ 3M SDE sampler. Slower with very high quality.
    /// Maps to API value "dpmpp_3m_sde".
    /// </summary>
    DpmPlusPlus3MSde,

    /// <summary>
    /// DPM++ 3M SDE Karras sampler. Slower with very high quality.
    /// Maps to API value "dpmpp_3m_sde_karras".
    /// </summary>
    DpmPlusPlus3MSdeKarras,

    /// <summary>
    /// DDIM (Denoising Diffusion Implicit Models) sampler. Fast with good quality.
    /// Maps to API value "ddim".
    /// </summary>
    Ddim,

    /// <summary>
    /// PLMS (Pseudo Linear Multi-Step) sampler. Fast with good quality.
    /// Maps to API value "plms".
    /// </summary>
    Plms,

    /// <summary>
    /// UniPC sampler. Fast with high quality.
    /// Maps to API value "uni_pc".
    /// </summary>
    UniPc,

    /// <summary>
    /// UniPC BH2 sampler. Fast with high quality.
    /// Maps to API value "uni_pc_bh2".
    /// </summary>
    UniPcBh2,

    /// <summary>
    /// DDPM (Denoising Diffusion Probabilistic Models) sampler. Slower with high quality.
    /// Maps to API value "ddpm".
    /// </summary>
    Ddpm,

    /// <summary>
    /// LCM (Latent Consistency Model) sampler. Very fast with good quality.
    /// Designed for use with LCM-LoRA or LCM models.
    /// Maps to API value "lcm".
    /// </summary>
    Lcm
}
