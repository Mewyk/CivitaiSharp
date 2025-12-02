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
    /// <param name="startDate">Optional start date for the reporting period (ISO 8601 format).</param>
    /// <param name="endDate">Optional end date for the reporting period (ISO 8601 format).</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>A task containing the consumption details.</returns>
    Task<Result<ConsumptionDetails>> GetConsumptionAsync(
        DateTime? startDate = null,
        DateTime? endDate = null,
        CancellationToken cancellationToken = default);
}
