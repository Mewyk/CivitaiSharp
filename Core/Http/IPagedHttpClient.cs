namespace CivitaiSharp.Core.Http;

using System.Threading;
using System.Threading.Tasks;
using CivitaiSharp.Core.Response;

/// <summary>
/// Extended HTTP client interface with pagination support specific to the Civitai public API response format.
/// </summary>
internal interface IPagedHttpClient : ICivitaiHttpClient
{
    /// <summary>
    /// Gets the API client configuration options.
    /// </summary>
    ApiClientOptions Options { get; }

    /// <summary>
    /// Performs an HTTP GET request with query string parameters and deserializes a paginated response.
    /// </summary>
    /// <typeparam name="T">The type of items in the paginated result.</typeparam>
    /// <param name="endpoint">The endpoint path (without query string).</param>
    /// <param name="queryString">Optional query string (including leading '?'). Empty string for no parameters.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The result contains either the paged items with metadata or an error.
    /// </returns>
    Task<Result<PagedResult<T>>> GetPageAsync<T>(
        string endpoint,
        string queryString = "",
        CancellationToken cancellationToken = default);
}
