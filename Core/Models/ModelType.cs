namespace CivitaiSharp.Core.Models;

/// <summary>
/// Type of model as represented by the Civitai API. Use <see cref="Extensions.EnumExtensions.ToApiString"/>
/// when converting to the API query string form.
/// </summary>
/// <remarks>
/// API string mappings are defined in <see cref="Extensions.ApiStringRegistry"/>.
/// </remarks>
public enum ModelType
{
    /// <summary>
    /// Stable Diffusion checkpoint model.
    /// Maps to API value "Checkpoint".
    /// </summary>
    Checkpoint,

    /// <summary>
    /// Textual inversion / embedding model.
    /// Maps to API value "TextualInversion".
    /// </summary>
    TextualInversion,

    /// <summary>
    /// Hypernetwork model.
    /// Maps to API value "Hypernetwork".
    /// </summary>
    Hypernetwork,

    /// <summary>
    /// Aesthetic gradient model.
    /// Maps to API value "AestheticGradient".
    /// </summary>
    AestheticGradient,

    /// <summary>
    /// LoRA (Low-Rank Adaptation) model.
    /// Maps to API value "LORA".
    /// </summary>
    Lora,

    /// <summary>
    /// LyCORIS (Lora beYond Conventional methods, Other Rank adaptation Implementations for Stable diffusion) model.
    /// Also includes LoCon (LoRA for Convolution Network) variants.
    /// Maps to API value "LoCon".
    /// </summary>
    LoCon,

    /// <summary>
    /// DoRA (Weight-Decomposed Low-Rank Adaptation) model.
    /// Maps to API value "DoRA".
    /// </summary>
    DoRa,

    /// <summary>
    /// ControlNet model for image composition control.
    /// Maps to API value "Controlnet".
    /// </summary>
    Controlnet,

    /// <summary>
    /// Pose estimation model.
    /// Maps to API value "Poses".
    /// </summary>
    Poses,

    /// <summary>
    /// Image upscaler model.
    /// Maps to API value "Upscaler".
    /// </summary>
    Upscaler,

    /// <summary>
    /// Motion module for video generation.
    /// Maps to API value "MotionModule".
    /// </summary>
    MotionModule,

    /// <summary>
    /// VAE (Variational Autoencoder) model.
    /// Maps to API value "VAE".
    /// </summary>
    Vae,

    /// <summary>
    /// Wildcards for dynamic prompting.
    /// Maps to API value "Wildcards".
    /// </summary>
    Wildcards,

    /// <summary>
    /// ComfyUI or other workflow definitions.
    /// Maps to API value "Workflows".
    /// </summary>
    Workflows,

    /// <summary>
    /// Other model types not covered by specific categories.
    /// Maps to API value "Other".
    /// </summary>
    Other
}
