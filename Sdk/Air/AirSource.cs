namespace CivitaiSharp.Sdk.Air;

/// <summary>
/// Supported source platforms in the AIR (Artificial Intelligence Resource) identifier system.
/// </summary>
/// <remarks>
/// API string mappings are defined in <see cref="Extensions.SdkApiStringRegistry"/>.
/// </remarks>
public enum AirSource
{
    /// <summary>
    /// Civitai platform (https://civitai.com).
    /// Maps to API value "civitai".
    /// </summary>
    Civitai,

    /// <summary>
    /// Hugging Face platform (https://huggingface.co).
    /// Maps to API value "huggingface".
    /// </summary>
    HuggingFace,

    /// <summary>
    /// OpenAI platform (https://openai.com).
    /// Maps to API value "openai".
    /// </summary>
    OpenAi,

    /// <summary>
    /// Leonardo.Ai platform (https://leonardo.ai).
    /// Maps to API value "leonardo".
    /// </summary>
    Leonardo
}
