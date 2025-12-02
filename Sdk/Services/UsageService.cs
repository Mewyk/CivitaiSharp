namespace CivitaiSharp.Sdk.Services;

using System;
using System.Threading;
using System.Threading.Tasks;
using CivitaiSharp.Core.Response;
using CivitaiSharp.Sdk.Http;
using CivitaiSharp.Sdk.Models.Usage;

/// <summary>
/// Implementation of the Usage service for monitoring account consumption.
/// </summary>
internal sealed class UsageService : IUsageService
{
    private readonly SdkHttpClient _httpClient;
    private readonly CivitaiSdkClientOptions _options;

    /// <summary>
    /// Initializes a new instance of the <see cref="UsageService"/> class.
    /// </summary>
    /// <param name="httpClient">The HTTP client for API requests.</param>
    /// <param name="options">The SDK client options.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="httpClient"/> or <paramref name="options"/> is null.</exception>
    internal UsageService(SdkHttpClient httpClient, CivitaiSdkClientOptions options)
    {
        ArgumentNullException.ThrowIfNull(httpClient);
        ArgumentNullException.ThrowIfNull(options);

        _httpClient = httpClient;
        _options = options;
    }

    /// <inheritdoc />
    public Task<Result<ConsumptionDetails>> GetConsumptionAsync(
        DateTime? startDate = null,
        DateTime? endDate = null,
        CancellationToken cancellationToken = default)
    {
        var query = new QueryStringBuilder()
            .Append("startDate", startDate)
            .Append("endDate", endDate);

        var uri = query.BuildUri(_options.GetApiPath("consumption"));
        return _httpClient.GetAsync<ConsumptionDetails>(uri, cancellationToken);
    }
}
