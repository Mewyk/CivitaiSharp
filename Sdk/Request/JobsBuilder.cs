namespace CivitaiSharp.Sdk.Request;

using System;
using CivitaiSharp.Core;
using CivitaiSharp.Sdk;
using CivitaiSharp.Sdk.Http;

/// <summary>
/// Entry point for job creation and querying operations.
/// Provides factory methods for creating new jobs and a fluent query builder for retrieving job status.
/// </summary>
public sealed record JobsBuilder
{
    private readonly SdkHttpClient _httpClient;
    private readonly SdkOptions _options;
    private readonly JobQueryBuilder _queryBuilder;

    /// <summary>
    /// Initializes a new instance of the <see cref="JobsBuilder"/> class.
    /// Internal to enforce creation through SdkClient.
    /// </summary>
    /// <param name="httpClient">The HTTP client used to execute requests.</param>
    /// <param name="options">The SDK client options.</param>
    /// <exception cref="ArgumentNullException">Thrown when httpClient or options is null.</exception>
    internal JobsBuilder(SdkHttpClient httpClient, SdkOptions options)
    {
        ArgumentNullException.ThrowIfNull(httpClient);
        ArgumentNullException.ThrowIfNull(options);

        _httpClient = httpClient;
        _options = options;
        _queryBuilder = new JobQueryBuilder(httpClient, options);
    }

    /// <summary>
    /// Gets a cached, immutable, thread-safe query builder for retrieving and managing job status.
    /// </summary>
    /// <remarks>
    /// This property returns a cached builder instance that can be safely reused.
    /// Each fluent method on the builder returns a new instance with the updated configuration.
    /// </remarks>
    /// <example>
    /// <code>
    /// // Query by ID with detailed information
    /// var result = await sdkClient.Jobs.Query
    ///     .WithDetailed()
    ///     .GetByIdAsync(jobId);
    /// 
    /// // Wait for completion
    /// var result = await sdkClient.Jobs.Query
    ///     .WithWait()
    ///     .GetByTokenAsync(token);
    /// 
    /// // Query by custom properties
    /// var result = await sdkClient.Jobs.Query
    ///     .WhereProperty("userId", JsonSerializer.SerializeToElement("12345"))
    ///     .ExecuteAsync();
    /// </code>
    /// </example>
    public JobQueryBuilder Query => _queryBuilder;

    /// <summary>
    /// Creates a new image generation job builder for submitting generation requests.
    /// Supports both text-to-image and image-to-image generation modes.
    /// </summary>
    /// <returns>A new <see cref="ImageGenerationBuilder"/> instance.</returns>
    /// <remarks>
    /// Use the returned builder to configure generation parameters (AIR identifier, positive prompt, dimensions, etc.)
    /// and call <see cref="ImageGenerationBuilder.ExecuteAsync"/> to submit the job.
    /// For image-to-image generation, also use <see cref="ImageGenerationBuilder.WithSourceImageUrl"/> and
    /// <see cref="ImageGenerationBuilder.WithDenoisingStrength"/>.
    /// </remarks>
    /// <example>
    /// <code>
    /// // Text-to-image
    /// var result = await sdkClient.Jobs
    ///     .CreateImage()
    ///     .WithAir(new AirIdentifier("sdxl", AirAssetType.Checkpoint, "civitai", 4201, 130072))
    ///     .WithPositivePrompt("a beautiful landscape")
    ///     .WithDimensions(1024, 1024)
    ///     .ExecuteAsync();
    /// 
    /// // Image-to-image
    /// var result = await sdkClient.Jobs
    ///     .CreateImage()
    ///     .WithAir(new AirIdentifier("sdxl", AirAssetType.Checkpoint, "civitai", 4201, 130072))
    ///     .WithPositivePrompt("add dramatic sunset lighting")
    ///     .WithSourceImageUrl("https://example.com/source.jpg")
    ///     .WithDenoisingStrength(0.7m)
    ///     .ExecuteAsync();
    /// </code>
    /// </example>
    public ImageGenerationBuilder CreateImage()
        => new(_httpClient, _options);
}
