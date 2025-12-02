namespace CivitaiSharp.Sdk.Models.Jobs;

using System.Text.Json.Serialization;
using CivitaiSharp.Sdk.Enums;
using CivitaiSharp.Sdk.Json.Converters;

/// <summary>
/// Configuration for an additional network (LoRA, embedding, etc.) to apply during generation.
/// </summary>
public sealed class ImageJobNetworkParams
{
    /// <summary>
    /// Gets or sets the type of network.
    /// </summary>
    [JsonPropertyName("type")]
    [JsonConverter(typeof(NetworkTypeConverter))]
    public required NetworkType Type { get; init; }

    /// <summary>
    /// Gets or sets the strength/weight of the network. Typically 0.0-2.0, default: 1.0.
    /// </summary>
    [JsonPropertyName("strength")]
    public decimal? Strength { get; init; }

    /// <summary>
    /// Gets or sets the trigger words for this network, if any.
    /// </summary>
    [JsonPropertyName("triggerWord")]
    public string? TriggerWord { get; init; }

    /// <summary>
    /// Gets or sets the CLIP strength for text encoder influence. Range: 0.0-2.0.
    /// </summary>
    [JsonPropertyName("clipStrength")]
    public decimal? ClipStrength { get; init; }
}
