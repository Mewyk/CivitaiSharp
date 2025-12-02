namespace CivitaiSharp.Sdk;

using System;
using CivitaiSharp.Sdk.Http;
using CivitaiSharp.Sdk.Services;

/// <summary>
/// Primary client facade for the Civitai Generator SDK. Provides access to image generation,
/// model availability checking, and usage tracking via the orchestration endpoints.
/// Obtain an instance through dependency injection using
/// <see cref="Extensions.ServiceCollectionExtensions.AddCivitaiSdk(Microsoft.Extensions.DependencyInjection.IServiceCollection, System.Action{CivitaiSdkClientOptions})"/>.
/// </summary>
public sealed class CivitaiSdkClient : ICivitaiSdkClient
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CivitaiSdkClient"/> class.
    /// Internal to enforce dependency injection usage.
    /// </summary>
    /// <param name="httpClient">The HTTP client used to make API requests.</param>
    /// <param name="options">The SDK client options.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="httpClient"/> or <paramref name="options"/> is null.</exception>
    internal CivitaiSdkClient(SdkHttpClient httpClient, CivitaiSdkClientOptions options)
    {
        ArgumentNullException.ThrowIfNull(httpClient);
        ArgumentNullException.ThrowIfNull(options);

        Jobs = new JobsService(httpClient, options);
        Coverage = new CoverageService(httpClient, options);
        Usage = new UsageService(httpClient, options);
    }

    /// <inheritdoc />
    public IJobsService Jobs { get; }

    /// <inheritdoc />
    public ICoverageService Coverage { get; }

    /// <inheritdoc />
    public IUsageService Usage { get; }
}
