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
public sealed record ImageJobControlNetBuilder(
    string? ImageUrl = null,
    string? Image = null,
    ControlNetPreprocessor? Preprocessor = null,
    decimal? Weight = null,
    decimal? StartStep = null,
    decimal? EndStep = null)
{

    /// <summary>
    /// Creates a new <see cref="ImageJobControlNetBuilder"/> instance.
    /// </summary>
    /// <returns>A new builder instance.</returns>
    public static ImageJobControlNetBuilder Create() => new();

    /// <summary>
    /// Sets the URL of the control image.
    /// </summary>
    /// <param name="imageUrl">The control image URL.</param>
    /// <returns>A new builder instance with the updated image URL.</returns>
    /// <remarks>
    /// Provide either this or <see cref="WithImageData"/>, not both.
    /// </remarks>
    public ImageJobControlNetBuilder WithImageUrl(string imageUrl)
        => this with { ImageUrl = imageUrl, Image = null };

    /// <summary>
    /// Sets the base64-encoded control image data.
    /// </summary>
    /// <param name="imageData">The base64-encoded image data.</param>
    /// <returns>A new builder instance with the updated image data.</returns>
    /// <remarks>
    /// <para>
    /// Provide either this or <see cref="WithImageUrl"/>, not both.
    /// </para>
    /// <para>
    /// Format: <c>data:image/png;base64,iVBORw0KGgo...</c> or raw base64 string.
    /// </para>
    /// </remarks>
    public ImageJobControlNetBuilder WithImageData(string imageData)
        => this with { Image = imageData, ImageUrl = null };

    /// <summary>
    /// Sets the preprocessor to apply to the control image.
    /// </summary>
    /// <param name="preprocessor">The preprocessor type.</param>
    /// <returns>A new builder instance with the updated preprocessor.</returns>
    public ImageJobControlNetBuilder WithPreprocessor(ControlNetPreprocessor preprocessor)
        => this with { Preprocessor = preprocessor };

    /// <summary>
    /// Sets the weight/influence of this ControlNet.
    /// </summary>
    /// <param name="weight">The weight value. Range: 0.0-2.0, default: 1.0.</param>
    /// <returns>A new builder instance with the updated weight.</returns>
    public ImageJobControlNetBuilder WithWeight(decimal weight)
        => this with { Weight = weight };

    /// <summary>
    /// Sets the starting step for ControlNet influence.
    /// </summary>
    /// <param name="startStep">The start step. Range: 0.0-1.0.</param>
    /// <returns>A new builder instance with the updated start step.</returns>
    public ImageJobControlNetBuilder WithStartStep(decimal startStep)
        => this with { StartStep = startStep };

    /// <summary>
    /// Sets the ending step for ControlNet influence.
    /// </summary>
    /// <param name="endStep">The end step. Range: 0.0-1.0.</param>
    /// <returns>A new builder instance with the updated end step.</returns>
    public ImageJobControlNetBuilder WithEndStep(decimal endStep)
        => this with { EndStep = endStep };

    /// <summary>
    /// Sets the step range for ControlNet influence.
    /// </summary>
    /// <param name="startStep">The start step. Range: 0.0-1.0.</param>
    /// <param name="endStep">The end step. Range: 0.0-1.0.</param>
    /// <returns>A new builder instance with the updated step range.</returns>
    public ImageJobControlNetBuilder WithStepRange(decimal startStep, decimal endStep)
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
        var hasImage = !string.IsNullOrWhiteSpace(Image);

        if (!hasUrl && !hasImage)
        {
            throw new InvalidOperationException(
                "Either ImageUrl or ImageData must be provided. Use WithImageUrl() or WithImageData().");
        }

        if (hasUrl && hasImage)
        {
            throw new InvalidOperationException(
                "Cannot provide both ImageUrl and ImageData. Use only one method.");
        }

        return new ImageJobControlNet
        {
            ImageUrl = ImageUrl,
            Image = Image,
            Preprocessor = Preprocessor,
            Weight = Weight,
            StartStep = StartStep,
            EndStep = EndStep
        };
    }
}
