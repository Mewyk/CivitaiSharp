namespace CivitaiSharp.Sdk.Models.Jobs;

using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using CivitaiSharp.Sdk.Air;

/// <summary>
/// Request model for text-to-image generation jobs.
/// </summary>
public sealed class TextToImageJobRequest
{
    /// <summary>
    /// The job type discriminator value for text-to-image jobs.
    /// </summary>
    public const string JobType = "textToImage";

    /// <summary>
    /// Gets the job type discriminator. Always <see cref="JobType"/>.
    /// </summary>
    [JsonPropertyName("$type")]
    public string Type => JobType;

    /// <summary>
    /// Gets or sets the base model AIR identifier. Required.
    /// </summary>
    /// <example>urn:air:sdxl:checkpoint:civitai:4201@130072</example>
    [JsonPropertyName("model")]
    public required AirIdentifier Model { get; init; }

    /// <summary>
    /// Gets or sets the generation parameters. Required.
    /// </summary>
    [JsonPropertyName("params")]
    public required ImageJobParams Params { get; init; }

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
