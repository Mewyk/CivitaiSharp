namespace CivitaiSharp.Core.Services;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using CivitaiSharp.Core.Http;
using CivitaiSharp.Core.Json;
using CivitaiSharp.Core.Models.Common;
using CivitaiSharp.Core.Response;
using Microsoft.Extensions.Logging;
using Polly.Timeout;

/// <summary>
/// Thread-safe HTTP client wrapper for the Civitai API. Handles JSON deserialization, error wrapping,
/// and optional logging. The underlying <see cref="HttpClient"/> is designed to be reused for multiple requests.
/// </summary>
internal sealed class ApiHttpClient : IPagedHttpClient
{
    private const string NextCursorHeaderName = "X-Next-Cursor";

    private readonly HttpClient _httpClient;
    private readonly ApiResponseHandler _responseHandler;
    private readonly ApiClientOptions _options;
    private readonly ILogger<ApiHttpClient>? _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ApiHttpClient"/> class.
    /// </summary>
    /// <param name="httpClient">The underlying HTTP client configured with the API base address and headers.</param>
    /// <param name="responseHandler">Handler for deserializing and processing HTTP responses.</param>
    /// <param name="options">Configuration options for building API paths.</param>
    /// <param name="logger">Optional logger for request tracking and diagnostics.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="httpClient"/>, <paramref name="responseHandler"/>, or <paramref name="options"/> is null.</exception>
    internal ApiHttpClient(
        HttpClient httpClient,
        ApiResponseHandler responseHandler,
        ApiClientOptions options,
        ILogger<ApiHttpClient>? logger = null)
    {
        ArgumentNullException.ThrowIfNull(httpClient);
        ArgumentNullException.ThrowIfNull(responseHandler);
        ArgumentNullException.ThrowIfNull(options);

        _httpClient = httpClient;
        _responseHandler = responseHandler;
        _options = options;
        _logger = logger;
    }

    /// <inheritdoc />
    public ApiClientOptions Options => _options;

    /// <summary>
    /// Executes a GET request with query string parameters to retrieve a page of items with pagination metadata.
    /// All parameters must be passed as query string parameters, as the Civitai API ignores JSON request bodies on GET requests.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This method uses <see cref="HttpCompletionOption.ResponseHeadersRead"/> for optimal memory usage with large responses.
    /// </para>
    /// <para>
    /// <strong>Important:</strong> The Civitai API requires all filter and pagination parameters to be passed as query string
    /// parameters. JSON request bodies are completely ignored for GET requests. Array parameters must be sent as repeated
    /// query parameters (e.g., <c>ids=1&amp;ids=2</c>) rather than comma-separated values.
    /// </para>
    /// </remarks>
    /// <typeparam name="T">The type of items to retrieve.</typeparam>
    /// <param name="endpoint">The endpoint path (without query string).</param>
    /// <param name="queryString">Optional query string (including leading '?'). Empty string for no parameters.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous operation. The result contains either the paged items or an error.</returns>
    /// <exception cref="OperationCanceledException">Thrown when the operation is cancelled via the cancellation token.</exception>
    public async Task<Result<PagedResult<T>>> GetPageAsync<T>(
        string endpoint,
        string queryString = "",
        CancellationToken cancellationToken = default)
    {
        const string method = "GET";
        var uri = endpoint + queryString;
        if (_logger is not null)
        {
            LogMessages.LogRequest(_logger, method, uri);
        }

        HttpResponseMessage response;
        try
        {
            response = await _httpClient.GetAsync(
                uri,
                HttpCompletionOption.ResponseHeadersRead,
                cancellationToken).ConfigureAwait(false);
        }
        catch (HttpRequestException exception)
        {
            if (_logger is not null)
            {
                LogMessages.LogHttpRequestFailed(_logger, exception, method, uri);
            }

            return new Result<PagedResult<T>>.Failure(HttpErrorMapper.FromHttpRequestException(exception));
        }
        catch (TaskCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (TaskCanceledException exception)
        {
            // TaskCanceledException can also be thrown on timeout
            if (_logger is not null)
            {
                LogMessages.LogRequestTimeout(_logger, exception, method, uri);
            }

            return new Result<PagedResult<T>>.Failure(HttpErrorMapper.FromTimeout(exception));
        }
        catch (TimeoutRejectedException exception)
        {
            // Thrown by Polly resilience handler when timeout policy is exceeded
            if (_logger is not null)
            {
                LogMessages.LogResilienceTimeout(_logger, exception, method, uri);
            }

            return new Result<PagedResult<T>>.Failure(HttpErrorMapper.FromTimeout(exception));
        }

        using (response)
        {
            var result = await _responseHandler.HandleResponseAsync<PagedApiResponse<T>>(
                response,
                cancellationToken).ConfigureAwait(false);

            if (result is Result<PagedApiResponse<T>>.Failure failure)
            {
                return new Result<PagedResult<T>>.Failure(failure.Error);
            }

            var data = ((Result<PagedApiResponse<T>>.Success)result).Data;
            var items = data.Items;
            var metadata = data.Metadata;

            if (metadata is null && response.Headers.TryGetValues(NextCursorHeaderName, out var cursorValues))
            {
                metadata = new PaginationMetadata(NextCursor: cursorValues.FirstOrDefault());
            }

            return new Result<PagedResult<T>>.Success(new PagedResult<T>(items, metadata));
        }
    }

    /// <inheritdoc />
    public Task<Result<TResponse>> GetAsync<TResponse>(
        string requestUri,
        CancellationToken cancellationToken = default)
    {
        const string method = "GET";
        if (_logger is not null)
        {
            LogMessages.LogRequest(_logger, method, requestUri);
        }

        return ExecuteWithExceptionHandlingAsync<TResponse>(
            method,
            requestUri,
            () => _httpClient.GetAsync(
                requestUri,
                HttpCompletionOption.ResponseHeadersRead,
                cancellationToken),
            cancellationToken);
    }

    /// <inheritdoc />
    public Task<Result<TResponse>> PostAsync<TResponse>(
        string requestUri,
        object? content,
        CancellationToken cancellationToken = default)
    {
        const string method = "POST";
        if (_logger is not null)
        {
            LogMessages.LogRequest(_logger, method, requestUri);
        }

        return ExecuteWithExceptionHandlingAsync<TResponse>(
            method,
            requestUri,
            () =>
            {
                var jsonContent = content is null ? null : CreateJsonContent(content);
                return _httpClient.PostAsync(requestUri, jsonContent, cancellationToken);
            },
            cancellationToken);
    }

    /// <inheritdoc />
    public Task<Result<TResponse>> PutAsync<TResponse>(
        string requestUri,
        object? content,
        CancellationToken cancellationToken = default)
    {
        const string method = "PUT";
        if (_logger is not null)
        {
            LogMessages.LogRequest(_logger, method, requestUri);
        }

        return ExecuteWithExceptionHandlingAsync<TResponse>(
            method,
            requestUri,
            () =>
            {
                var jsonContent = content is null ? null : CreateJsonContent(content);
                return _httpClient.PutAsync(requestUri, jsonContent, cancellationToken);
            },
            cancellationToken);
    }

    /// <inheritdoc />
    public Task<Result<TResponse>> DeleteAsync<TResponse>(
        string requestUri,
        CancellationToken cancellationToken = default)
    {
        const string method = "DELETE";
        if (_logger is not null)
        {
            LogMessages.LogRequest(_logger, method, requestUri);
        }

        return ExecuteWithExceptionHandlingAsync<TResponse>(
            method,
            requestUri,
            () => _httpClient.DeleteAsync(requestUri, cancellationToken),
            cancellationToken);
    }

    private async Task<Result<TResponse>> ExecuteWithExceptionHandlingAsync<TResponse>(
        string method,
        string requestUri,
        Func<Task<HttpResponseMessage>> sendRequest,
        CancellationToken cancellationToken)
    {
        HttpResponseMessage response;
        try
        {
            response = await sendRequest().ConfigureAwait(false);
        }
        catch (HttpRequestException exception)
        {
            if (_logger is not null)
            {
                LogMessages.LogHttpRequestFailed(_logger, exception, method, requestUri);
            }

            return new Result<TResponse>.Failure(HttpErrorMapper.FromHttpRequestException(exception));
        }
        catch (TaskCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (TaskCanceledException exception)
        {
            if (_logger is not null)
            {
                LogMessages.LogRequestTimeout(_logger, exception, method, requestUri);
            }

            return new Result<TResponse>.Failure(HttpErrorMapper.FromTimeout(exception));
        }
        catch (TimeoutRejectedException exception)
        {
            // Thrown by Polly resilience handler when timeout policy is exceeded
            if (_logger is not null)
            {
                LogMessages.LogResilienceTimeout(_logger, exception, method, requestUri);
            }

            return new Result<TResponse>.Failure(HttpErrorMapper.FromTimeout(exception));
        }

        using (response)
        {
            return await _responseHandler.HandleResponseAsync<TResponse>(response, cancellationToken).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Creates JSON content for HTTP requests using the source-generated serialization context.
    /// This is AOT-compatible as it uses the typed JsonSerializer.Serialize overload with JsonSerializerContext.
    /// </summary>
    /// <param name="content">The content to serialize to JSON.</param>
    /// <returns>A StringContent with application/json media type.</returns>
    /// <remarks>
    /// Note: The Civitai public API endpoints currently accessed by Core use query string parameters for GET requests,
    /// not JSON bodies. This method is retained for potential future POST/PUT operations and for use by the Sdk library.
    /// </remarks>
    private static System.Net.Http.StringContent CreateJsonContent(object content)
    {
        // Serialize using the source-generated context for AOT compatibility
        var json = JsonSerializer.Serialize(content, content.GetType(), CivitaiJsonContext.Default);
        return new System.Net.Http.StringContent(json, System.Text.Encoding.UTF8, "application/json");
    }
}
