namespace CivitaiSharp.Core.Http;

using System.Threading;
using System.Threading.Tasks;
using CivitaiSharp.Core.Response;

/// <summary>
/// Abstraction for HTTP communication with Civitai APIs. Provides strongly-typed async methods
/// for standard HTTP verbs with JSON serialization/deserialization and error response parsing.
/// Results are wrapped in <see cref="Result{T}"/>.
/// </summary>
public interface ICivitaiHttpClient
{
    /// <summary>
    /// Performs an HTTP GET request and deserializes the response.
    /// </summary>
    /// <typeparam name="TResponse">The type to deserialize the response body into.</typeparam>
    /// <param name="requestUri">The relative URI to request.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>A task containing either the deserialized response or an error.</returns>
    Task<Result<TResponse>> GetAsync<TResponse>(
        string requestUri,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Performs an HTTP POST request with a JSON body and deserializes the response.
    /// </summary>
    /// <typeparam name="TResponse">The type to deserialize the response body into.</typeparam>
    /// <param name="requestUri">The relative URI to request.</param>
    /// <param name="content">The content to serialize as JSON in the request body. Can be null.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>A task containing either the deserialized response or an error.</returns>
    Task<Result<TResponse>> PostAsync<TResponse>(
        string requestUri,
        object? content,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Performs an HTTP PUT request with a JSON body and deserializes the response.
    /// </summary>
    /// <typeparam name="TResponse">The type to deserialize the response body into.</typeparam>
    /// <param name="requestUri">The relative URI to request.</param>
    /// <param name="content">The content to serialize as JSON in the request body. Can be null.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>A task containing either the deserialized response or an error.</returns>
    Task<Result<TResponse>> PutAsync<TResponse>(
        string requestUri,
        object? content,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Performs an HTTP DELETE request and deserializes the response.
    /// </summary>
    /// <typeparam name="TResponse">The type to deserialize the response body into.</typeparam>
    /// <param name="requestUri">The relative URI to request.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>A task containing either the deserialized response or an error.</returns>
    Task<Result<TResponse>> DeleteAsync<TResponse>(
        string requestUri,
        CancellationToken cancellationToken = default);
}
