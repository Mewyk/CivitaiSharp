namespace CivitaiSharp.Sdk.Models.Jobs;

using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using CivitaiSharp.Sdk.Air;

/// <summary>
/// Request model for image generation jobs. Supports both text-to-image and image-to-image modes.
/// </summary>
/// <remarks>
/// The job type is always "textToImage" for the Civitai API, regardless of whether source image is provided.
/// When <see cref="SourceImageUrl"/> is provided, the generation operates in image-to-image mode.
/// </remarks>
public sealed class ImageGenerationJobRequest
{
    /// <summary>
    /// The job type discriminator value for image generation jobs.
    /// </summary>
    public const string JobType = "textToImage";

    /// <summary>
    /// Gets the job type discriminator. Always <see cref="JobType"/>.
    /// </summary>
    [JsonPropertyName("$type")]
    public static string Type => JobType;

    /// <summary>
    /// Gets or sets the base AIR identifier. Required.
    /// </summary>
    /// <remarks>
    /// Note: The Civitai API uses "model" as the JSON property name, but this C# property
    /// is named "Air" to accurately reflect that it represents an AIR (Asset Identifier Resource)
    /// identifier, not a Model object. This distinction improves code clarity and type safety.
    /// </remarks>
    /// <example>urn:air:sdxl:checkpoint:civitai:4201@130072</example>
    [JsonPropertyName("model")]
    public required AirIdentifier Air { get; init; }

    /// <summary>
    /// Gets or sets the generation parameters. Required.
    /// </summary>
    [JsonPropertyName("params")]
    public required ImageJobParams Params { get; init; }

    /// <summary>
    /// Gets or sets the source image URL for image-to-image generation.
    /// </summary>
    /// <remarks>
    /// When provided, enables image-to-image mode where the source image is transformed
    /// according to the prompt and other parameters. Use <see cref="DenoisingStrength"/>
    /// to control how much the source image is transformed.
    /// </remarks>
    [JsonPropertyName("image")]
    public string? SourceImageUrl { get; init; }

    /// <summary>
    /// Gets or sets the denoising strength for image-to-image generation. Range: 0.0-1.0.
    /// </summary>
    /// <remarks>
    /// Only applicable when <see cref="SourceImageUrl"/> is provided.
    /// Lower values (e.g., 0.3) preserve more of the source image.
    /// Higher values (e.g., 0.8) allow more transformation.
    /// </remarks>
    [JsonPropertyName("strength")]
    public decimal? DenoisingStrength { get; init; }

    /// <summary>
    /// Gets or sets additional networks (LoRAs, embeddings, etc.) to apply.
    /// Key is the AIR identifier, value is the network configuration.
    /// </summary>
    [JsonPropertyName("additionalNetworks")]
    public IReadOnlyDictionary<AirIdentifier, ImageJobNetworkParams>? AdditionalNetworks { get; init; }

    /// <summary>
    /// Gets or sets ControlNet configurations for guided generation.
    /// </summary>
    [JsonPropertyName("controlNets")]
    public IReadOnlyList<ImageJobControlNet>? ControlNets { get; init; }

    /// <summary>
    /// Gets or sets the number of images to generate. Range: 1-10, default: 1.
    /// </summary>
    [JsonPropertyName("quantity")]
    public int? Quantity { get; init; }

    /// <summary>
    /// Gets or sets the priority configuration for job scheduling.
    /// </summary>
    [JsonPropertyName("priority")]
    public Priority? Priority { get; init; }

    /// <summary>
    /// Gets or sets custom properties for job tracking and querying.
    /// </summary>
    /// <remarks>
    /// Properties can contain any JSON-serializable values. Use <see cref="JsonElement"/>
    /// to preserve AOT compatibility while supporting arbitrary value types.
    /// </remarks>
    [JsonPropertyName("properties")]
    public IReadOnlyDictionary<string, JsonElement>? Properties { get; init; }

    /// <summary>
    /// Gets or sets the webhook URL to call when the job completes.
    /// </summary>
    [JsonPropertyName("callbackUrl")]
    public string? CallbackUrl { get; init; }

    /// <summary>
    /// Gets or sets the number of automatic retries on failure. Default: 0.
    /// </summary>
    [JsonPropertyName("retries")]
    public int? Retries { get; init; }

    /// <summary>
    /// Gets or sets the job timeout. Format: "HH:mm:ss". Default: "00:10:00".
    /// </summary>
    [JsonPropertyName("timeout")]
    public string? Timeout { get; init; }

    /// <summary>
    /// Gets or sets the number of CLIP layers to skip. Range: 1-12.
    /// </summary>
    /// <remarks>
    /// This can also be set in <see cref="ImageJobParams.ClipSkip"/>.
    /// </remarks>
    [JsonPropertyName("clipSkip")]
    public int? ClipSkip { get; init; }
}
