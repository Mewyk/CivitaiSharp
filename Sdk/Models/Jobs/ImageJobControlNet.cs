namespace CivitaiSharp.Sdk.Models.Jobs;

using System;
using System.Text.Json.Serialization;
using CivitaiSharp.Sdk.Enums;
using CivitaiSharp.Sdk.Json.Converters;

/// <summary>
/// Configuration for a ControlNet to apply during generation.
/// </summary>
/// <remarks>
/// Either <see cref="ImageUrl"/> or <see cref="BlobKey"/> must be provided, but not both.
/// </remarks>
public sealed class ImageJobControlNet
{
    /// <summary>
    /// Gets or sets the URL of the control image.
    /// </summary>
    /// <remarks>
    /// Provide either this property or <see cref="BlobKey"/>, not both.
    /// </remarks>
    [JsonPropertyName("imageUrl")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? ImageUrl { get; init; }

    /// <summary>
    /// Gets or sets the blob key referencing an already-uploaded control image.
    /// </summary>
    /// <remarks>
    /// Provide either this property or <see cref="ImageUrl"/>, not both.
    /// Use this when the image has been pre-uploaded to Civitai's blob storage.
    /// </remarks>
    [JsonPropertyName("blobKey")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? BlobKey { get; init; }

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

    /// <summary>
    /// Validates that exactly one of <see cref="ImageUrl"/> or <see cref="BlobKey"/> is provided.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown when neither or both properties are provided.
    /// </exception>
    public void Validate()
    {
        var hasUrl = !string.IsNullOrWhiteSpace(ImageUrl);
        var hasBlobKey = !string.IsNullOrWhiteSpace(BlobKey);

        if (!hasUrl && !hasBlobKey)
        {
            throw new InvalidOperationException(
                "Either ImageUrl or BlobKey must be provided for ControlNet configuration.");
        }

        if (hasUrl && hasBlobKey)
        {
            throw new InvalidOperationException(
                "Only one of ImageUrl or BlobKey can be provided for ControlNet configuration, not both.");
        }
    }
}
