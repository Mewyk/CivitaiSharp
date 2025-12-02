namespace CivitaiSharp.Sdk.Models.Jobs;

using System.Text.Json.Serialization;
using CivitaiSharp.Sdk.Enums;
using CivitaiSharp.Sdk.Json.Converters;

/// <summary>
/// Generation parameters for image jobs.
/// </summary>
public sealed class ImageJobParams
{
    /// <summary>
    /// Gets or sets the positive prompt for image generation. Required.
    /// </summary>
    [JsonPropertyName("prompt")]
    public required string Prompt { get; init; }

    /// <summary>
    /// Gets or sets the negative prompt describing what to avoid in the generated image.
    /// </summary>
    [JsonPropertyName("negativePrompt")]
    public string? NegativePrompt { get; init; }

    /// <summary>
    /// Gets or sets the sampling algorithm to use.
    /// </summary>
    [JsonPropertyName("scheduler")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonConverter(typeof(NullableSchedulerConverter))]
    public Scheduler? Scheduler { get; init; }

    /// <summary>
    /// Gets or sets the number of sampling steps. Range: 1-100, default: 20.
    /// </summary>
    [JsonPropertyName("steps")]
    public int? Steps { get; init; }

    /// <summary>
    /// Gets or sets the classifier-free guidance scale. Range: 1-30, default: 7.0.
    /// </summary>
    /// <remarks>
    /// Higher values make the image more closely match the prompt but may reduce quality.
    /// </remarks>
    [JsonPropertyName("cfgScale")]
    public decimal? CfgScale { get; init; }

    /// <summary>
    /// Gets or sets the image width in pixels. Must be a multiple of 8. Range: 64-2048.
    /// </summary>
    [JsonPropertyName("width")]
    public int? Width { get; init; }

    /// <summary>
    /// Gets or sets the image height in pixels. Must be a multiple of 8. Range: 64-2048.
    /// </summary>
    [JsonPropertyName("height")]
    public int? Height { get; init; }

    /// <summary>
    /// Gets or sets the random seed for reproducible generation.
    /// </summary>
    [JsonPropertyName("seed")]
    public long? Seed { get; init; }

    /// <summary>
    /// Gets or sets the number of CLIP layers to skip. Range: 1-12.
    /// </summary>
    /// <remarks>
    /// A value of 2 is commonly used for anime/Pony models.
    /// </remarks>
    [JsonPropertyName("clipSkip")]
    public int? ClipSkip { get; init; }

    /// <summary>
    /// Gets or sets the source image URL for image-to-image generation.
    /// </summary>
    [JsonPropertyName("image")]
    public string? Image { get; init; }

    /// <summary>
    /// Gets or sets the denoising strength for image-to-image generation. Range: 0.0-1.0.
    /// </summary>
    /// <remarks>
    /// Lower values preserve more of the source image; higher values allow more change.
    /// </remarks>
    [JsonPropertyName("strength")]
    public decimal? Strength { get; init; }
}
