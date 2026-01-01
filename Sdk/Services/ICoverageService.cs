namespace CivitaiSharp.Sdk.Services;

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CivitaiSharp.Core.Response;
using CivitaiSharp.Sdk.Air;
using CivitaiSharp.Sdk.Models.Coverage;

/// <summary>
/// Service for checking model availability on the generation infrastructure.
/// </summary>
public interface ICoverageService
{
    /// <summary>
    /// Checks the availability of one or more AIR identifiers.
    /// </summary>
    /// <param name="airIdentifiers">The AIR identifiers to check.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>Dictionary mapping AIR identifiers to their availability information.</returns>
    Task<Result<IReadOnlyDictionary<AirIdentifier, ProviderAssetAvailability>>> GetAsync(
        IEnumerable<AirIdentifier> airIdentifiers,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks the availability of a single AIR identifier.
    /// </summary>
    /// <param name="air">The AIR identifier to check.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>Availability information for the AIR identifier.</returns>
    Task<Result<ProviderAssetAvailability>> GetAsync(
        AirIdentifier air,
        CancellationToken cancellationToken = default);
}
