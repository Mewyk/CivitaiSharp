namespace CivitaiSharp.Sdk.Request;

using CivitaiSharp.Sdk.Enums;
using CivitaiSharp.Sdk.Models.Jobs;

/// <summary>
/// Fluent builder for constructing <see cref="ImageJobNetworkParams"/> instances.
/// </summary>
/// <remarks>
/// This builder follows an immutable design pattern. Each method returns a new instance
/// with the updated configuration, making it thread-safe and cacheable.
/// </remarks>
public sealed record ImageJobNetworkParamsBuilder(
    NetworkType? Type = null,
    decimal? Strength = null,
    string? TriggerWord = null,
    decimal? ClipStrength = null)
{

    /// <summary>
    /// Creates a new <see cref="ImageJobNetworkParamsBuilder"/> instance.
    /// </summary>
    /// <returns>A new builder instance.</returns>
    public static ImageJobNetworkParamsBuilder Create() => new();

    /// <summary>
    /// Sets the network type.
    /// </summary>
    /// <param name="type">The network type (LoRA, embedding, etc.). Required.</param>
    /// <returns>A new builder instance with the updated type.</returns>
    public ImageJobNetworkParamsBuilder WithType(NetworkType type)
        => this with { Type = type };

    /// <summary>
    /// Sets the strength/weight of the network.
    /// </summary>
    /// <param name="strength">The strength value. Typically 0.0-2.0, default: 1.0.</param>
    /// <returns>A new builder instance with the updated strength.</returns>
    public ImageJobNetworkParamsBuilder WithStrength(decimal strength)
        => this with { Strength = strength };

    /// <summary>
    /// Sets the trigger words for this network.
    /// </summary>
    /// <param name="triggerWord">The trigger word(s) to activate the network.</param>
    /// <returns>A new builder instance with the updated trigger word.</returns>
    public ImageJobNetworkParamsBuilder WithTriggerWord(string triggerWord)
        => this with { TriggerWord = triggerWord };

    /// <summary>
    /// Sets the CLIP strength for text encoder influence.
    /// </summary>
    /// <param name="clipStrength">The CLIP strength. Range: 0.0-2.0.</param>
    /// <returns>A new builder instance with the updated CLIP strength.</returns>
    public ImageJobNetworkParamsBuilder WithClipStrength(decimal clipStrength)
        => this with { ClipStrength = clipStrength };

    /// <summary>
    /// Builds the <see cref="ImageJobNetworkParams"/> instance.
    /// </summary>
    /// <returns>The configured <see cref="ImageJobNetworkParams"/>.</returns>
    /// <exception cref="InvalidOperationException">Thrown when required properties are missing.</exception>
    public ImageJobNetworkParams Build()
    {
        if (Type == null)
        {
            throw new InvalidOperationException("Type is required. Use WithType() to set it.");
        }

        return new ImageJobNetworkParams
        {
            Type = Type.Value,
            Strength = Strength,
            TriggerWord = TriggerWord,
            ClipStrength = ClipStrength
        };
    }
}
