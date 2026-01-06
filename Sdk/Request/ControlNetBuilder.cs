namespace CivitaiSharp.Sdk.Request;

using CivitaiSharp.Sdk.Enums;
using CivitaiSharp.Sdk.Models.Jobs;

/// <summary>
/// Fluent builder for constructing <see cref="ImageJobControlNet"/> instances.
/// </summary>
/// <remarks>
/// This builder follows an immutable design pattern. Each method returns a new instance
/// with the updated configuration, making it thread-safe and cacheable.
/// </remarks>
public sealed record ControlNetBuilder(
    string? ImageUrl = null,
    string? BlobKey = null,
    ControlNetPreprocessor? Preprocessor = null,
    decimal? Weight = null,
    decimal? StartStep = null,
    decimal? EndStep = null)
{

    /// <summary>
    /// Creates a new <see cref="ControlNetBuilder"/> instance.
    /// </summary>
    /// <returns>A new builder instance.</returns>
    public static ControlNetBuilder Create() => new();

    /// <summary>
    /// Sets the URL of the control image.
    /// </summary>
    /// <param name="imageUrl">The control image URL.</param>
    /// <returns>A new builder instance with the updated image URL.</returns>
    /// <remarks>
    /// Provide either this or <see cref="WithBlobKey"/>, not both.
    /// </remarks>
    public ControlNetBuilder WithImageUrl(string imageUrl)
        => this with { ImageUrl = imageUrl, BlobKey = null };

    /// <summary>
    /// Sets the blob key referencing a pre-uploaded control image.
    /// </summary>
    /// <param name="blobKey">The blob storage key for the control image.</param>
    /// <returns>A new builder instance with the updated blob key.</returns>
    /// <remarks>
    /// Provide either this or <see cref="WithImageUrl"/>, not both.
    /// Use this when the control image has been pre-uploaded to Civitai's blob storage.
    /// </remarks>
    public ControlNetBuilder WithBlobKey(string blobKey)
        => this with { BlobKey = blobKey, ImageUrl = null };

    /// <summary>
    /// Sets the preprocessor to apply to the control image.
    /// </summary>
    /// <param name="preprocessor">The preprocessor type.</param>
    /// <returns>A new builder instance with the updated preprocessor.</returns>
    public ControlNetBuilder WithPreprocessor(ControlNetPreprocessor preprocessor)
        => this with { Preprocessor = preprocessor };

    /// <summary>
    /// Sets the weight/influence of this ControlNet.
    /// </summary>
    /// <param name="weight">The weight value. Range: 0.0-2.0, default: 1.0.</param>
    /// <returns>A new builder instance with the updated weight.</returns>
    public ControlNetBuilder WithWeight(decimal weight)
        => this with { Weight = weight };

    /// <summary>
    /// Sets the starting step for ControlNet influence.
    /// </summary>
    /// <param name="startStep">The start step. Range: 0.0-1.0.</param>
    /// <returns>A new builder instance with the updated start step.</returns>
    public ControlNetBuilder WithStartStep(decimal startStep)
        => this with { StartStep = startStep };

    /// <summary>
    /// Sets the ending step for ControlNet influence.
    /// </summary>
    /// <param name="endStep">The end step. Range: 0.0-1.0.</param>
    /// <returns>A new builder instance with the updated end step.</returns>
    public ControlNetBuilder WithEndStep(decimal endStep)
        => this with { EndStep = endStep };

    /// <summary>
    /// Sets the step range for ControlNet influence.
    /// </summary>
    /// <param name="startStep">The start step. Range: 0.0-1.0.</param>
    /// <param name="endStep">The end step. Range: 0.0-1.0.</param>
    /// <returns>A new builder instance with the updated step range.</returns>
    public ControlNetBuilder WithStepRange(decimal startStep, decimal endStep)
        => this with { StartStep = startStep, EndStep = endStep };

    /// <summary>
    /// Builds the <see cref="ImageJobControlNet"/> instance.
    /// </summary>
    /// <returns>The configured <see cref="ImageJobControlNet"/>.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when neither or both image source properties are provided.
    /// </exception>
    public ImageJobControlNet Build()
    {
        var hasUrl = !string.IsNullOrWhiteSpace(ImageUrl);
        var hasBlobKey = !string.IsNullOrWhiteSpace(BlobKey);

        if (!hasUrl && !hasBlobKey)
        {
            throw new InvalidOperationException(
                "Either ImageUrl or BlobKey must be provided. Use WithImageUrl() or WithBlobKey().");
        }

        if (hasUrl && hasBlobKey)
        {
            throw new InvalidOperationException(
                "Cannot provide both ImageUrl and BlobKey. Use only one method.");
        }

        return new ImageJobControlNet
        {
            ImageUrl = ImageUrl,
            BlobKey = BlobKey,
            Preprocessor = Preprocessor,
            Weight = Weight,
            StartStep = StartStep,
            EndStep = EndStep
        };
    }
}
