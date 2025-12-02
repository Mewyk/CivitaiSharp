namespace CivitaiSharp.Sdk.Models.Jobs;

using System.Text.Json.Serialization;
using CivitaiSharp.Sdk.Enums;
using CivitaiSharp.Sdk.Json.Converters;

/// <summary>
/// Configuration for a ControlNet to apply during generation.
/// </summary>
/// <remarks>
/// Either <see cref="ImageUrl"/> or <see cref="Image"/> must be provided, but not both.
/// </remarks>
public sealed class ImageJobControlNet
{
    /// <summary>
    /// Gets or sets the URL of the control image.
    /// </summary>
    /// <remarks>
    /// Provide either this property or <see cref="Image"/>, not both.
    /// </remarks>
    [JsonPropertyName("imageUrl")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? ImageUrl { get; init; }

    /// <summary>
    /// Gets or sets the base64-encoded control image data.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Provide either this property or <see cref="ImageUrl"/>, not both.
    /// </para>
    /// <para>
    /// Format: <c>data:image/png;base64,iVBORw0KGgo...</c> or raw base64 string.
    /// </para>
    /// </remarks>
    [JsonPropertyName("image")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Image { get; init; }

    /// <summary>
    /// Gets or sets the preprocessor to apply to the control image.
    /// </summary>
    [JsonPropertyName("preprocessor")]
    [JsonConverter(typeof(NullableControlNetPreprocessorConverter))]
    public ControlNetPreprocessor? Preprocessor { get; init; }

    /// <summary>
    /// Gets or sets the weight/influence of this ControlNet. Range: 0.0-2.0, default: 1.0.
    /// </summary>
    [JsonPropertyName("weight")]
    public decimal? Weight { get; init; }

    /// <summary>
    /// Gets or sets the starting step for ControlNet influence. Range: 0.0-1.0.
    /// </summary>
    [JsonPropertyName("startStep")]
    public decimal? StartStep { get; init; }

    /// <summary>
    /// Gets or sets the ending step for ControlNet influence. Range: 0.0-1.0.
    /// </summary>
    [JsonPropertyName("endStep")]
    public decimal? EndStep { get; init; }
}
