namespace CivitaiSharp.Sdk.Services;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CivitaiSharp.Core.Response;
using CivitaiSharp.Sdk.Http;
using CivitaiSharp.Sdk.Models.Coverage;

/// <summary>
/// Implementation of the Coverage service for checking model availability.
/// </summary>
internal sealed class CoverageService : ICoverageService
{
    private readonly SdkHttpClient _httpClient;
    private readonly CivitaiSdkClientOptions _options;

    /// <summary>
    /// Initializes a new instance of the <see cref="CoverageService"/> class.
    /// </summary>
    /// <param name="httpClient">The HTTP client for API requests.</param>
    /// <param name="options">The SDK client options.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="httpClient"/> or <paramref name="options"/> is null.</exception>
    internal CoverageService(SdkHttpClient httpClient, CivitaiSdkClientOptions options)
    {
        ArgumentNullException.ThrowIfNull(httpClient);
        ArgumentNullException.ThrowIfNull(options);

        _httpClient = httpClient;
        _options = options;
    }

    /// <inheritdoc />
    public Task<Result<IReadOnlyDictionary<string, ProviderAssetAvailability>>> GetAsync(
        IEnumerable<string> models,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(models);

        // Avoid double enumeration by checking if already a list
        var modelList = models as IReadOnlyList<string> ?? models.ToList();
        if (modelList.Count == 0)
        {
            throw new ArgumentException("At least one model is required.", nameof(models));
        }

        var uri = BuildCoverageUri(modelList);
        return _httpClient.GetAsync<IReadOnlyDictionary<string, ProviderAssetAvailability>>(uri, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Result<ProviderAssetAvailability>> GetAsync(
        string model,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(model);

        var result = await GetAsync([model], cancellationToken).ConfigureAwait(false);

        return result.Match<Result<ProviderAssetAvailability>>(
            onSuccess: data =>
            {
                if (data.TryGetValue(model, out var availability))
                {
                    return new Result<ProviderAssetAvailability>.Success(availability);
                }

                return new Result<ProviderAssetAvailability>.Failure(
                    Error.Create(ErrorCode.NotFound, $"Model '{model}' not found in coverage response."));
            },
            onFailure: error => new Result<ProviderAssetAvailability>.Failure(error));
    }

    private string BuildCoverageUri(IReadOnlyList<string> models)
    {
        var path = _options.GetApiPath("coverage");
        var query = new QueryStringBuilder();

        foreach (var model in models)
        {
            query.Append("model", model);
        }

        return query.BuildUri(path);
    }
}
