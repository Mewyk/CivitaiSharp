namespace CivitaiSharp.Sdk.Services;

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CivitaiSharp.Core.Response;
using CivitaiSharp.Sdk.Models.Coverage;

/// <summary>
/// Service for checking model availability on the generation infrastructure.
/// </summary>
public interface ICoverageService
{
    /// <summary>
    /// Checks the availability of one or more models.
    /// </summary>
    /// <param name="models">The AIR identifiers of the models to check.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>
    /// A task containing a dictionary mapping AIR identifiers to their availability information.
    /// </returns>
    Task<Result<IReadOnlyDictionary<string, ProviderAssetAvailability>>> GetAsync(
        IEnumerable<string> models,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks the availability of a single model.
    /// </summary>
    /// <param name="model">The AIR identifier of the model to check.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>A task containing the availability information for the model.</returns>
    Task<Result<ProviderAssetAvailability>> GetAsync(
        string model,
        CancellationToken cancellationToken = default);
}
