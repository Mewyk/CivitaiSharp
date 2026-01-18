namespace CivitaiSharp.Sdk.Services;

using System;
using System.Threading;
using System.Threading.Tasks;
using CivitaiSharp.Core.Response;
using CivitaiSharp.Sdk.Models.Usage;

/// <summary>
/// Service for monitoring account usage and consumption.
/// </summary>
public interface IUsageService
{
    /// <summary>
    /// Gets account consumption statistics for the specified period.
    /// </summary>
    /// <param name="startDate">Start date for the reporting period (UTC). Serialized as ISO 8601.</param>
    /// <param name="endDate">End date for the reporting period (UTC). Serialized as ISO 8601.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>Consumption details.</returns>
    Task<Result<ConsumptionDetails>> GetConsumptionAsync(
        DateTime? startDate = null,
        DateTime? endDate = null,
        CancellationToken cancellationToken = default);
}
