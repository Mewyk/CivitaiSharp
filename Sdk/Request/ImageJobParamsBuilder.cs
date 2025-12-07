namespace CivitaiSharp.Sdk.Request;

using CivitaiSharp.Sdk.Enums;
using CivitaiSharp.Sdk.Models.Jobs;

/// <summary>
/// Fluent builder for constructing <see cref="ImageJobParams"/> instances.
/// </summary>
/// <remarks>
/// This builder follows an immutable design pattern. Each method returns a new instance
/// with the updated configuration, making it thread-safe and cacheable.
/// </remarks>
public sealed record ImageJobParamsBuilder(
    string? Prompt = null,
    string? NegativePrompt = null,
    Scheduler? Scheduler = null,
    int? Steps = null,
    decimal? CfgScale = null,
    int? Width = null,
    int? Height = null,
    long? Seed = null,
    int? ClipSkip = null,
    string? Image = null,
    decimal? Strength = null)
{
    /// <summary>
    /// Creates a new <see cref="ImageJobParamsBuilder"/> instance.
    /// </summary>
    /// <returns>A new builder instance.</returns>
    public static ImageJobParamsBuilder Create() => new();

    /// <summary>
    /// Sets the positive prompt for image generation.
    /// </summary>
    /// <param name="prompt">The prompt text describing what to generate. Required.</param>
    /// <returns>A new builder instance with the updated prompt.</returns>
    public ImageJobParamsBuilder WithPrompt(string prompt)
        => this with { Prompt = prompt };

    /// <summary>
    /// Sets the negative prompt describing what to avoid in the generated image.
    /// </summary>
    /// <param name="negativePrompt">The negative prompt text.</param>
    /// <returns>A new builder instance with the updated negative prompt.</returns>
    public ImageJobParamsBuilder WithNegativePrompt(string negativePrompt)
        => this with { NegativePrompt = negativePrompt };

    /// <summary>
    /// Sets the sampling algorithm to use.
    /// </summary>
    /// <param name="scheduler">The scheduler/sampler.</param>
    /// <returns>A new builder instance with the updated scheduler.</returns>
    public ImageJobParamsBuilder WithScheduler(Scheduler scheduler)
        => this with { Scheduler = scheduler };

    /// <summary>
    /// Sets the number of sampling steps.
    /// </summary>
    /// <param name="steps">The step count. Range: 1-100, default: 20.</param>
    /// <returns>A new builder instance with the updated steps.</returns>
    public ImageJobParamsBuilder WithSteps(int steps)
        => this with { Steps = steps };

    /// <summary>
    /// Sets the classifier-free guidance scale.
    /// </summary>
    /// <param name="cfgScale">The CFG scale. Range: 1-30, default: 7.0.</param>
    /// <returns>A new builder instance with the updated CFG scale.</returns>
    /// <remarks>
    /// Higher values make the image more closely match the prompt but may reduce quality.
    /// </remarks>
    public ImageJobParamsBuilder WithCfgScale(decimal cfgScale)
        => this with { CfgScale = cfgScale };

    /// <summary>
    /// Sets the image dimensions.
    /// </summary>
    /// <param name="width">The width in pixels. Must be a multiple of 8. Range: 64-2048.</param>
    /// <param name="height">The height in pixels. Must be a multiple of 8. Range: 64-2048.</param>
    /// <returns>A new builder instance with the updated dimensions.</returns>
    public ImageJobParamsBuilder WithSize(int width, int height)
        => this with { Width = width, Height = height };

    /// <summary>
    /// Sets the image width.
    /// </summary>
    /// <param name="width">The width in pixels. Must be a multiple of 8. Range: 64-2048.</param>
    /// <returns>A new builder instance with the updated width.</returns>
    public ImageJobParamsBuilder WithWidth(int width)
        => this with { Width = width };

    /// <summary>
    /// Sets the image height.
    /// </summary>
    /// <param name="height">The height in pixels. Must be a multiple of 8. Range: 64-2048.</param>
    /// <returns>A new builder instance with the updated height.</returns>
    public ImageJobParamsBuilder WithHeight(int height)
        => this with { Height = height };

    /// <summary>
    /// Sets the random seed for reproducible generation.
    /// </summary>
    /// <param name="seed">The seed value.</param>
    /// <returns>A new builder instance with the updated seed.</returns>
    public ImageJobParamsBuilder WithSeed(long seed)
        => this with { Seed = seed };

    /// <summary>
    /// Sets the number of CLIP layers to skip.
    /// </summary>
    /// <param name="clipSkip">The number of layers to skip. Range: 1-12.</param>
    /// <returns>A new builder instance with the updated CLIP skip.</returns>
    /// <remarks>
    /// A value of 2 is commonly used for anime/Pony models.
    /// </remarks>
    public ImageJobParamsBuilder WithClipSkip(int clipSkip)
        => this with { ClipSkip = clipSkip };

    /// <summary>
    /// Sets the source image URL for image-to-image generation.
    /// </summary>
    /// <param name="imageUrl">The source image URL.</param>
    /// <returns>A new builder instance with the updated source image.</returns>
    public ImageJobParamsBuilder WithSourceImage(string imageUrl)
        => this with { Image = imageUrl };

    /// <summary>
    /// Sets the denoising strength for image-to-image generation.
    /// </summary>
    /// <param name="strength">The strength value. Range: 0.0-1.0.</param>
    /// <returns>A new builder instance with the updated strength.</returns>
    /// <remarks>
    /// Lower values preserve more of the source image; higher values allow more change.
    /// </remarks>
    public ImageJobParamsBuilder WithStrength(decimal strength)
        => this with { Strength = strength };

    /// <summary>
    /// Builds the <see cref="ImageJobParams"/> instance.
    /// </summary>
    /// <returns>The configured <see cref="ImageJobParams"/>.</returns>
    /// <exception cref="InvalidOperationException">Thrown when required properties are missing.</exception>
    public ImageJobParams Build()
    {
        if (string.IsNullOrWhiteSpace(Prompt))
        {
            throw new InvalidOperationException("Prompt is required. Use WithPrompt() to set it.");
        }

        return new ImageJobParams
        {
            Prompt = Prompt,
            NegativePrompt = NegativePrompt,
            Scheduler = Scheduler,
            Steps = Steps,
            CfgScale = CfgScale,
            Width = Width,
            Height = Height,
            Seed = Seed,
            ClipSkip = ClipSkip,
            Image = Image,
            Strength = Strength
        };
    }
}
