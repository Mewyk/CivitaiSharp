namespace CivitaiSharp.Sdk.Enums;

/// <summary>
/// Types of additional networks that can be applied during image generation.
/// </summary>
/// <remarks>
/// API string mappings are defined in <see cref="Extensions.SdkApiStringRegistry"/>.
/// </remarks>
public enum NetworkType
{
    /// <summary>
    /// LoRA (Low-Rank Adaptation) network for fine-tuning model behavior.
    /// Maps to API value "lora".
    /// </summary>
    Lora,

    /// <summary>
    /// LyCORIS network variant.
    /// Maps to API value "lycoris".
    /// </summary>
    Lycoris,

    /// <summary>
    /// DoRA (Weight-Decomposed Low-Rank Adaptation) network.
    /// Maps to API value "dora".
    /// </summary>
    Dora,

    /// <summary>
    /// Textual Inversion embedding for adding new concepts.
    /// Maps to API value "embedding".
    /// </summary>
    Embedding,

    /// <summary>
    /// VAE (Variational Autoencoder) for encoding/decoding images.
    /// Maps to API value "vae".
    /// </summary>
    Vae
}
