namespace CivitaiSharp.Sdk.Services;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CivitaiSharp.Core;
using CivitaiSharp.Core.Response;
using CivitaiSharp.Sdk.Air;
using CivitaiSharp.Sdk.Http;
using CivitaiSharp.Sdk.Models.Coverage;

/// <summary>
/// Implementation of the Coverage service for checking model availability.
/// </summary>
/// <remarks>
/// This service is registered as a singleton and holds references to the HTTP client and options.
/// It is created via dependency injection and should not be instantiated directly.
/// </remarks>
/// <param name="httpClient">The HTTP client for API requests.</param>
/// <param name="options">The SDK client options.</param>
/// <exception cref="ArgumentNullException">Thrown when <paramref name="httpClient"/> or <paramref name="options"/> is null.</exception>
internal sealed class CoverageService(SdkHttpClient httpClient, SdkOptions options) : ICoverageService
{
    /// <inheritdoc />
    public Task<Result<IReadOnlyDictionary<AirIdentifier, ProviderAssetAvailability>>> GetAsync(
        IEnumerable<AirIdentifier> airIdentifiers,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(airIdentifiers);

        // Avoid double enumeration by checking if already a list
        var airList = airIdentifiers as IReadOnlyList<AirIdentifier> ?? [.. airIdentifiers];
        if (airList.Count == 0)
        {
            throw new ArgumentException("At least one AIR identifier is required.", nameof(airIdentifiers));
        }

        var uri = BuildCoverageUri(airList);
        return httpClient.GetAsync<IReadOnlyDictionary<AirIdentifier, ProviderAssetAvailability>>(uri, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Result<ProviderAssetAvailability>> GetAsync(
        AirIdentifier air,
        CancellationToken cancellationToken = default)
    {
        var result = await GetAsync([air], cancellationToken).ConfigureAwait(false);

        return result.Match<Result<ProviderAssetAvailability>>(
            onSuccess: data =>
            {
                if (data.TryGetValue(air, out var availability))
                {
                    return new Result<ProviderAssetAvailability>.Success(availability);
                }

                return new Result<ProviderAssetAvailability>.Failure(
                    Error.Create(ErrorCode.NotFound, $"AIR identifier '{air}' not found in coverage response."));
            },
            onFailure: error => new Result<ProviderAssetAvailability>.Failure(error));
    }

    private string BuildCoverageUri(IReadOnlyList<AirIdentifier> airList)
    {
        var path = options.GetApiPath("coverage");
        var query = new QueryStringBuilder();

        foreach (var air in airList)
        {
            query.Append("model", air.ToString());
        }

        return query.BuildUri(path);
    }
}
