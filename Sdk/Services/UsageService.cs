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
/// <remarks>
/// This service is registered as a singleton and holds references to the HTTP client and options.
/// It is created via dependency injection and should not be instantiated directly.
/// </remarks>
/// <param name="httpClient">The HTTP client for API requests.</param>
/// <param name="options">The SDK client options.</param>
/// <exception cref="ArgumentNullException">Thrown when <paramref name="httpClient"/> or <paramref name="options"/> is null.</exception>
internal sealed class UsageService(SdkHttpClient httpClient, SdkClientOptions options) : IUsageService
{
    /// <inheritdoc />
    public Task<Result<ConsumptionDetails>> GetConsumptionAsync(
        DateTime? startDate = null,
        DateTime? endDate = null,
        CancellationToken cancellationToken = default)
    {
        var query = new QueryStringBuilder()
            .Append("startDate", startDate)
            .Append("endDate", endDate);

        var uri = query.BuildUri(options.GetApiPath("consumption"));
        return httpClient.GetAsync<ConsumptionDetails>(uri, cancellationToken);
    }
}
