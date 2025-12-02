namespace CivitaiSharp.Sdk.Air;

/// <summary>
/// Supported asset types in the AIR (Artificial Intelligence Resource) identifier system.
/// </summary>
/// <remarks>
/// API string mappings are defined in <see cref="Extensions.SdkApiStringRegistry"/>.
/// </remarks>
public enum AirAssetType
{
    /// <summary>
    /// Base model checkpoint.
    /// Maps to API value "checkpoint".
    /// </summary>
    Checkpoint,

    /// <summary>
    /// LoRA (Low-Rank Adaptation) adapter.
    /// Maps to API value "lora".
    /// </summary>
    Lora,

    /// <summary>
    /// LyCORIS model variant.
    /// Maps to API value "lycoris".
    /// </summary>
    Lycoris,

    /// <summary>
    /// VAE (Variational Autoencoder) model.
    /// Maps to API value "vae".
    /// </summary>
    Vae,

    /// <summary>
    /// Textual Inversion embedding.
    /// Maps to API value "embedding".
    /// </summary>
    Embedding,

    /// <summary>
    /// Hypernetwork model.
    /// Maps to API value "hypernet".
    /// </summary>
    Hypernetwork
}
