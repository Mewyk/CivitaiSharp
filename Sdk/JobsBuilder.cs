namespace CivitaiSharp.Sdk;

using System;
using CivitaiSharp.Sdk.Http;
using CivitaiSharp.Sdk.Request;

/// <summary>
/// Entry point for job creation and querying operations.
/// Provides factory methods for creating new jobs and a fluent query builder for retrieving job status.
/// </summary>
public sealed record JobsBuilder
{
    private readonly SdkHttpClient _httpClient;
    private readonly SdkClientOptions _options;
    private readonly JobQueryBuilder _queryBuilder;

    /// <summary>
    /// Initializes a new instance of the <see cref="JobsBuilder"/> class.
    /// Internal to enforce creation through SdkClient.
    /// </summary>
    /// <param name="httpClient">The HTTP client used to execute requests.</param>
    /// <param name="options">The SDK client options.</param>
    /// <exception cref="ArgumentNullException">Thrown when httpClient or options is null.</exception>
    internal JobsBuilder(SdkHttpClient httpClient, SdkClientOptions options)
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
    /// Creates a new text-to-image job builder for submitting generation requests.
    /// </summary>
    /// <returns>A new <see cref="TextToImageBuilder"/> instance.</returns>
    /// <remarks>
    /// Use the returned builder to configure generation parameters (model, prompt, size, etc.)
    /// and call <see cref="TextToImageBuilder.ExecuteAsync"/> to submit the job.
    /// </remarks>
    /// <example>
    /// <code>
    /// var result = await sdkClient.Jobs
    ///     .CreateTextToImage()
    ///     .WithModel(AirIdentifier.Parse("urn:air:sdxl:checkpoint:civitai:4201@130072"))
    ///     .WithPrompt("a beautiful landscape")
    ///     .WithSize(1024, 1024)
    ///     .ExecuteAsync();
    /// </code>
    /// </example>
    public TextToImageBuilder CreateTextToImage()
        => new(_httpClient, _options);
}
