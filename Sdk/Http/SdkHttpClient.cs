namespace CivitaiSharp.Sdk.Http;

using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using CivitaiSharp.Core.Response;
using Microsoft.Extensions.Logging;
using Polly.Timeout;

/// <summary>
/// HTTP client implementation for the Civitai SDK. Handles AOT-compatible JSON serialization,
/// RFC 7807 error parsing, and result wrapping.
/// </summary>
internal sealed class SdkHttpClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<SdkHttpClient>? _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="SdkHttpClient"/> class.
    /// </summary>
    /// <param name="httpClient">The underlying HTTP client configured with base address and headers.</param>
    /// <param name="logger">Optional logger for request tracking and diagnostics.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="httpClient"/> is null.</exception>
    internal SdkHttpClient(
        HttpClient httpClient,
        ILogger<SdkHttpClient>? logger = null)
    {
        ArgumentNullException.ThrowIfNull(httpClient);

        _httpClient = httpClient;
        _logger = logger;
    }

    /// <summary>
    /// Performs an HTTP GET request and deserializes the response.
    /// </summary>
    /// <typeparam name="TResponse">The type to deserialize the response body into.</typeparam>
    /// <param name="requestUri">The relative URI to request.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>A task containing either the deserialized response or an error.</returns>
    public Task<Result<TResponse>> GetAsync<TResponse>(
        string requestUri,
        CancellationToken cancellationToken = default)
    {
        if (_logger is not null)
        {
            LogMessages.LogRequest(_logger, "GET", requestUri);
        }

        return ExecuteRequestAsync<TResponse>(
            "GET",
            requestUri,
            async () => await _httpClient.GetAsync(requestUri, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false),
            cancellationToken);
    }

    /// <summary>
    /// Performs an HTTP POST request with a JSON body and deserializes the response.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request body.</typeparam>
    /// <typeparam name="TResponse">The type to deserialize the response body into.</typeparam>
    /// <param name="requestUri">The relative URI to request.</param>
    /// <param name="content">The content to serialize as JSON in the request body.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>A task containing either the deserialized response or an error.</returns>
    public Task<Result<TResponse>> PostAsync<TRequest, TResponse>(
        string requestUri,
        TRequest content,
        CancellationToken cancellationToken = default)
    {
        if (_logger is not null)
        {
            LogMessages.LogRequest(_logger, "POST", requestUri);
        }

        return ExecuteRequestAsync<TResponse>(
            "POST",
            requestUri,
            async () =>
            {
                var jsonContent = SdkJsonTypeResolver.CreateJsonContent(content);
                return await _httpClient.PostAsync(requestUri, jsonContent, cancellationToken).ConfigureAwait(false);
            },
            cancellationToken);
    }

    /// <summary>
    /// Performs an HTTP PUT request and deserializes the response.
    /// </summary>
    /// <typeparam name="TResponse">The type to deserialize the response body into.</typeparam>
    /// <param name="requestUri">The relative URI to request.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>A task containing either the deserialized response or an error.</returns>
    public Task<Result<TResponse>> PutAsync<TResponse>(
        string requestUri,
        CancellationToken cancellationToken = default)
    {
        if (_logger is not null)
        {
            LogMessages.LogRequest(_logger, "PUT", requestUri);
        }

        return ExecuteRequestAsync<TResponse>(
            "PUT",
            requestUri,
            async () => await _httpClient.PutAsync(requestUri, null, cancellationToken).ConfigureAwait(false),
            cancellationToken);
    }

    /// <summary>
    /// Performs an HTTP DELETE request and deserializes the response.
    /// </summary>
    /// <typeparam name="TResponse">The type to deserialize the response body into.</typeparam>
    /// <param name="requestUri">The relative URI to request.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>A task containing either the deserialized response or an error.</returns>
    public Task<Result<TResponse>> DeleteAsync<TResponse>(
        string requestUri,
        CancellationToken cancellationToken = default)
    {
        if (_logger is not null)
        {
            LogMessages.LogRequest(_logger, "DELETE", requestUri);
        }

        return ExecuteRequestAsync<TResponse>(
            "DELETE",
            requestUri,
            async () => await _httpClient.DeleteAsync(requestUri, cancellationToken).ConfigureAwait(false),
            cancellationToken);
    }

    private async Task<Result<TResponse>> ExecuteRequestAsync<TResponse>(
        string method,
        string requestUri,
        Func<Task<HttpResponseMessage>> sendRequest,
        CancellationToken cancellationToken)
    {
        try
        {
            var response = await sendRequest().ConfigureAwait(false);
            return await HandleResponseAsync<TResponse>(response, method, requestUri, _logger, cancellationToken).ConfigureAwait(false);
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
    }

    private static async Task<Result<TResponse>> HandleResponseAsync<TResponse>(
        HttpResponseMessage response,
        string method,
        string requestUri,
        ILogger? logger,
        CancellationToken cancellationToken)
    {
        using (response)
        {
            // Check content type first - even on success, non-JSON responses indicate a problem
            // (e.g., Cloudflare HTML error pages can return 200 status)
            if (!IsJsonContentType(response.Content.Headers.ContentType))
            {
                return await HandleNonJsonResponseAsync<TResponse>(response, method, requestUri, logger, cancellationToken)
                    .ConfigureAwait(false);
            }

            if (response.IsSuccessStatusCode)
            {
                return await DeserializeSuccessResponseAsync<TResponse>(response, cancellationToken)
                    .ConfigureAwait(false);
            }

            return await ParseErrorResponseAsync<TResponse>(response, cancellationToken)
                .ConfigureAwait(false);
        }
    }

    private static bool IsJsonContentType(System.Net.Http.Headers.MediaTypeHeaderValue? contentType)
    {
        if (contentType?.MediaType is null)
        {
            // No content type header - assume JSON as a reasonable default
            return true;
        }

        // Check for JSON content types: application/json, application/problem+json, etc.
        return contentType.MediaType.Contains("json", StringComparison.OrdinalIgnoreCase);
    }

    private static async Task<Result<TResponse>> HandleNonJsonResponseAsync<TResponse>(
        HttpResponseMessage response,
        string method,
        string requestUri,
        ILogger? logger,
        CancellationToken cancellationToken)
    {
        var content = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
        var contentType = response.Content.Headers.ContentType?.MediaType ?? "unknown";
        var statusCode = (int)response.StatusCode;

        // Detect and categorize Cloudflare error pages using the shared mapper from Core
        var cloudflareErrorType = HttpErrorMapper.DetectCloudflareErrorType(content);

        if (cloudflareErrorType is not null)
        {
            if (logger is not null)
            {
                LogMessages.LogCloudflareError(logger, method, requestUri, statusCode, cloudflareErrorType);
            }

            var message = HttpErrorMapper.BuildCloudflareErrorMessage(cloudflareErrorType, statusCode, contentType);

            return new Result<TResponse>.Failure(new Error(
                ErrorCode.CloudflareError,
                message,
                HttpStatusCode: response.StatusCode));
        }

        // Non-Cloudflare unexpected content type
        if (logger is not null)
        {
            LogMessages.LogUnexpectedContentType(logger, method, requestUri, contentType);
        }

        var genericMessage = $"Received unexpected content type: {contentType}. Expected JSON response from API.";

        // Use the HTTP status code mapping if it's an error status
        var errorCode = response.IsSuccessStatusCode
            ? ErrorCode.UnexpectedContentType
            : HttpErrorMapper.MapStatusCode(response.StatusCode);

        return new Result<TResponse>.Failure(new Error(
            errorCode,
            genericMessage,
            HttpStatusCode: response.StatusCode));
    }

    private static async Task<Result<TResponse>> DeserializeSuccessResponseAsync<TResponse>(
        HttpResponseMessage response,
        CancellationToken cancellationToken)
    {
        // Handle Unit type for void responses (DELETE, some PUT operations)
        if (typeof(TResponse) == typeof(Unit))
        {
            return new Result<TResponse>.Success((TResponse)(object)Unit.Value);
        }

        try
        {
            await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken)
                .ConfigureAwait(false);

            var result = await SdkJsonTypeResolver.DeserializeAsync<TResponse>(stream, cancellationToken)
                .ConfigureAwait(false);

            return result is null
                ? new Result<TResponse>.Failure(Error.Create(
                    ErrorCode.EmptyResponse,
                    "JSON deserialization returned null."))
                : new Result<TResponse>.Success(result);
        }
        catch (JsonException exception)
        {
            return new Result<TResponse>.Failure(Error.Create(
                ErrorCode.DeserializationFailed,
                $"Failed to deserialize JSON response: {exception.Message}",
                exception));
        }
    }

    private static async Task<Result<TResponse>> ParseErrorResponseAsync<TResponse>(
        HttpResponseMessage response,
        CancellationToken cancellationToken)
    {
        try
        {
            var content = await response.Content.ReadAsStringAsync(cancellationToken)
                .ConfigureAwait(false);

            var error = HttpErrorMapper.FromHttpResponse(
                response.StatusCode,
                content,
                response.ReasonPhrase,
                response.Headers.RetryAfter);

            return new Result<TResponse>.Failure(error);
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            return new Result<TResponse>.Failure(Error.Create(ErrorCode.HttpError, exception.Message, exception));
        }
    }
}
