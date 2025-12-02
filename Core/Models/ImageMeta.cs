namespace CivitaiSharp.Core.Models;

using System.Text.Json;
using System.Text.Json.Serialization;

/// <summary>
/// Generation metadata and parameters for an image.
/// </summary>
/// <remarks>
/// <para>
/// The meta object has a HIGHLY variable structure depending on the generation
/// workflow (ComfyUI, A1111, etc.). This record captures common fields, but the full
/// JSON may contain many additional fields. Use <see cref="ExtensionData"/> to access
/// any fields not explicitly modeled.
/// </para>
/// <para>
/// <strong>Important:</strong> The variability and potential inconsistencies in this metadata
/// are not caused by Civitai.com. This data originates from the program that created, edited,
/// or generated the media (such as Automatic1111, ComfyUI, InvokeAI, or other generation tools).
/// Each tool embeds its own metadata format, which Civitai passes through without modification.
/// </para>
/// <para>
/// This type uses init-only properties instead of a positional record to support
/// <see cref="JsonExtensionDataAttribute"/>, which is incompatible with constructor parameters
/// when using System.Text.Json source generation.
/// </para>
/// </remarks>
public sealed record ImageMeta
{
    /// <summary>The image size as a string (e.g., "896x1152").</summary>
    [JsonPropertyName("Size")]
    public string? Size { get; init; }

    /// <summary>The random seed used for generation.</summary>
    [JsonPropertyName("seed")]
    public long? Seed { get; init; }

    /// <summary>The number of denoising steps used in generation.</summary>
    [JsonPropertyName("steps")]
    public int? Steps { get; init; }

    /// <summary>The positive prompt text used to guide image generation.</summary>
    [JsonPropertyName("prompt")]
    public string? Prompt { get; init; }

    /// <summary>The negative prompt text describing what to avoid in the generated image.</summary>
    [JsonPropertyName("negativePrompt")]
    public string? NegativePrompt { get; init; }

    /// <summary>The sampling algorithm used (e.g., "DPM++ 2M", "Euler").</summary>
    [JsonPropertyName("sampler")]
    public string? Sampler { get; init; }

    /// <summary>The classifier-free guidance scale controlling prompt influence.</summary>
    [JsonPropertyName("cfgScale")]
    public decimal? CfgScale { get; init; }

    /// <summary>The number of tokens to skip in the CLIP model.</summary>
    [JsonPropertyName("clipSkip")]
    public int? ClipSkip { get; init; }

    /// <summary>The width of the generated image in pixels.</summary>
    [JsonPropertyName("width")]
    public int? Width { get; init; }

    /// <summary>The height of the generated image in pixels.</summary>
    [JsonPropertyName("height")]
    public int? Height { get; init; }

    /// <summary>The denoising strength for inpainting or image-to-image operations.</summary>
    [JsonPropertyName("denoise")]
    public decimal? Denoise { get; init; }

    /// <summary>The noise scheduler used during generation (e.g., "Karras", "Normal").</summary>
    [JsonPropertyName("scheduler")]
    public string? Scheduler { get; init; }

    /// <summary>The name of the base model used for generation.</summary>
    [JsonPropertyName("Model")]
    public string? Model { get; init; }

    /// <summary>The hash identifier of the model used.</summary>
    [JsonPropertyName("Model hash")]
    public string? ModelHash { get; init; }

    /// <summary>The Variational Auto-Encoder (VAE) used for encoding/decoding.</summary>
    [JsonPropertyName("VAE")]
    public string? Vae { get; init; }

    /// <summary>The base model type (e.g., "Flux.1", "SDXL 1.0", "Pony").</summary>
    [JsonPropertyName("baseModel")]
    public string? BaseModel { get; init; }

    /// <summary>The Flux mode URN if using Flux generation.</summary>
    [JsonPropertyName("fluxMode")]
    public string? FluxMode { get; init; }

    /// <summary>The version identifier of the generation software.</summary>
    [JsonPropertyName("Version")]
    public string? Version { get; init; }

    /// <summary>The number of steps used in high-resolution upscaling.</summary>
    [JsonPropertyName("Hires steps")]
    public string? HiresSteps { get; init; }

    /// <summary>The upscaling factor for high-resolution generation.</summary>
    [JsonPropertyName("Hires upscale")]
    public string? HiresUpscale { get; init; }

    /// <summary>The upscaling algorithm used (e.g., "Latent", "RealESRGAN").</summary>
    [JsonPropertyName("Hires upscaler")]
    public string? HiresUpscaler { get; init; }

    /// <summary>The denoising strength applied during upscaling.</summary>
    [JsonPropertyName("Denoising strength")]
    public string? DenoisingStrength { get; init; }

    /// <summary>The generation workflow type (e.g., "txt2img", "img2img").</summary>
    [JsonPropertyName("workflow")]
    public string? Workflow { get; init; }

    /// <summary>The date when the image was created (ISO format).</summary>
    [JsonPropertyName("Created Date")]
    public string? CreatedDate { get; init; }

    /// <summary>The random number generator type used.</summary>
    [JsonPropertyName("RNG")]
    public string? Rng { get; init; }

    /// <summary>The noise schedule type (e.g., "Karras").</summary>
    [JsonPropertyName("Schedule type")]
    public string? ScheduleType { get; init; }

    /// <summary>Raw ComfyUI workflow JSON for complex multi-step generations.</summary>
    [JsonPropertyName("comfy")]
    public string? Comfy { get; init; }

    /// <summary>Indicates whether the image has not-safe-for-work content.</summary>
    [JsonPropertyName("nsfw")]
    public bool? IsNsfw { get; init; }

    /// <summary>Indicates whether the image is a draft version.</summary>
    [JsonPropertyName("draft")]
    public bool? Draft { get; init; }

    /// <summary>The number of images generated in this batch.</summary>
    [JsonPropertyName("quantity")]
    public int? Quantity { get; init; }

    /// <summary>Extra metadata such as remix information.</summary>
    [JsonPropertyName("extra")]
    public ImageMetaExtra? Extra { get; init; }

    /// <summary>Dictionary of model and LoRA hashes used in generation.</summary>
    [JsonPropertyName("hashes")]
    public IReadOnlyDictionary<string, string>? Hashes { get; init; }

    /// <summary>Array of Variational Auto-Encoder names used in generation.</summary>
    [JsonPropertyName("vaes")]
    public IReadOnlyList<string>? VariationalAutoEncoders { get; init; }

    /// <summary>Resources used in generation (typically from A1111 workflows).</summary>
    [JsonPropertyName("resources")]
    public IReadOnlyList<ImageMetaResource>? Resources { get; init; }

    /// <summary>Civitai resources with type, model version ID, version name, and weight.</summary>
    [JsonPropertyName("civitaiResources")]
    public IReadOnlyList<CivitaiResource>? CivitaiResources { get; init; }

    /// <summary>
    /// Additional resources used in the generation.
    /// </summary>
    /// <remarks>
    /// This property uses <c>IReadOnlyList&lt;object&gt;</c> because the Civitai API returns highly variable
    /// structures for additional resources depending on the generation workflow. This is fine since the
    /// JSON structure is not documented by the API, nor is the data consistent.
    /// </remarks>
    [JsonPropertyName("additionalResources")]
    public IReadOnlyList<object>? AdditionalResources { get; init; }

    /// <summary>
    /// ControlNet configurations used in generation.
    /// </summary>
    /// <remarks>
    /// This property uses <c>IReadOnlyList&lt;object&gt;</c> because the Civitai API returns highly variable
    /// structures for ControlNet configurations depending on the generation workflow. This is fine since
    /// the JSON structure is not documented by the API, nor is the data consistent.
    /// </remarks>
    [JsonPropertyName("controlNets")]
    public IReadOnlyList<object>? ControlNets { get; init; }

    /// <summary>Array of upscaler names used in post-processing.</summary>
    [JsonPropertyName("upscalers")]
    public IReadOnlyList<string>? Upscalers { get; init; }

    /// <summary>
    /// Model configurations used in generation.
    /// </summary>
    /// <remarks>
    /// This property uses <c>IReadOnlyList&lt;object&gt;</c> because the Civitai API returns highly variable
    /// structures for model configurations depending on the generation workflow. This is fine since the
    /// JSON structure is not documented by the API, nor is the data consistent.
    /// </remarks>
    [JsonPropertyName("models")]
    public IReadOnlyList<object>? Models { get; init; }

    /// <summary>Array of model IDs used to generate the image.</summary>
    [JsonPropertyName("modelIds")]
    public IReadOnlyList<long>? ModelIds { get; init; }

    /// <summary>Array of model version IDs used to generate the image.</summary>
    [JsonPropertyName("versionIds")]
    public IReadOnlyList<long>? VersionIds { get; init; }

    /// <summary>
    /// Contains any additional properties not explicitly modeled in this record.
    /// This handles the highly variable nature of generation metadata.
    /// </summary>
    [JsonExtensionData]
    public Dictionary<string, JsonElement>? ExtensionData { get; set; }
}
