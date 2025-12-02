namespace CivitaiSharp.Core.Services;

using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using CivitaiSharp.Core.Json;
using CivitaiSharp.Core.Response;

/// <summary>
/// Handles deserialization and error processing of HTTP responses from the API.
/// Uses source-generated JSON serialization for AOT compatibility.
/// </summary>
internal sealed class ApiResponseHandler
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ApiResponseHandler"/> class.
    /// </summary>
    internal ApiResponseHandler()
    {
    }

    /// <summary>
    /// Processes an HTTP response from the API and deserializes it into a typed result.
    /// Handles specific error codes (unauthorized, rate limit) and general deserialization errors.
    /// </summary>
    /// <typeparam name="T">The type to deserialize the response into. Must be registered in <see cref="CivitaiJsonContext"/>.</typeparam>
    /// <param name="response">The HTTP response message to process.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous operation. The result contains either the deserialized data or an error.</returns>
    /// <exception cref="ArgumentNullException">Thrown when response is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown when type T is not registered in <see cref="CivitaiJsonContext"/>.</exception>
    /// <remarks>
    /// This method uses the source-generated <see cref="CivitaiJsonContext"/> for AOT-compatible deserialization.
    /// All types used with this method must be registered in that context via [JsonSerializable] attributes.
    /// </remarks>
    public async Task<Result<T>> HandleResponseAsync<T>(HttpResponseMessage response, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(response);

        // Check content type first - even on success, non-JSON responses indicate a problem
        // (e.g., Cloudflare HTML error pages can return 200 status)
        if (!IsJsonContentType(response.Content.Headers.ContentType))
        {
            return await HandleNonJsonResponseAsync<T>(response, cancellationToken).ConfigureAwait(false);
        }

        // Handle success responses
        if (response.IsSuccessStatusCode)
        {
            return await DeserializeSuccessResponseAsync<T>(response, cancellationToken).ConfigureAwait(false);
        }

        // Handle error responses using the shared mapper
        return await ParseErrorResponseAsync<T>(response, cancellationToken).ConfigureAwait(false);
    }

    private static bool IsJsonContentType(MediaTypeHeaderValue? contentType)
    {
        if (contentType?.MediaType is null)
        {
            // No content type header - assume JSON as a reasonable default
            return true;
        }

        // Check for JSON content types: application/json, application/problem+json, etc.
        return contentType.MediaType.Contains("json", StringComparison.OrdinalIgnoreCase);
    }

    private static async Task<Result<T>> HandleNonJsonResponseAsync<T>(
        HttpResponseMessage response,
        CancellationToken cancellationToken)
    {
        var content = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
        var contentType = response.Content.Headers.ContentType?.MediaType ?? "unknown";

        // Detect and categorize Cloudflare error pages using the shared mapper
        var cloudflareErrorType = HttpErrorMapper.DetectCloudflareErrorType(content);

        if (cloudflareErrorType is not null)
        {
            var message = HttpErrorMapper.BuildCloudflareErrorMessage(cloudflareErrorType, (int)response.StatusCode, contentType);

            return new Result<T>.Failure(new Error(
                ErrorCode.CloudflareError,
                message,
                HttpStatusCode: response.StatusCode));
        }

        // Non-Cloudflare unexpected content type
        var genericMessage = $"Received unexpected content type: {contentType}. Expected JSON response from API.";

        // Use the HTTP status code mapping if it's an error status
        var errorCode = response.IsSuccessStatusCode
            ? ErrorCode.UnexpectedContentType
            : HttpErrorMapper.MapStatusCode(response.StatusCode);

        return new Result<T>.Failure(new Error(
            errorCode,
            genericMessage,
            HttpStatusCode: response.StatusCode));
    }

    private static async Task<Result<T>> DeserializeSuccessResponseAsync<T>(
        HttpResponseMessage response,
        CancellationToken cancellationToken)
    {
        try
        {
            await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);

            // Use AOT-compatible deserialization via JsonSerializerContext
            // This method uses typeof(T) with the source-generated context to resolve JsonTypeInfo
            var deserializedObject = await JsonSerializer.DeserializeAsync(
                stream,
                typeof(T),
                CivitaiJsonContext.Default,
                cancellationToken).ConfigureAwait(false);

            return deserializedObject is null
                ? new Result<T>.Failure(Error.Create(
                    ErrorCode.EmptyResponse,
                    "JSON deserialization returned null (possible empty or null response)."))
                : new Result<T>.Success((T)deserializedObject);
        }
        catch (JsonException jsonException)
        {
            return new Result<T>.Failure(Error.Create(
                ErrorCode.DeserializationFailed,
                $"Failed to deserialize JSON response: {jsonException.Message}",
                jsonException));
        }
    }

    private static async Task<Result<T>> ParseErrorResponseAsync<T>(
        HttpResponseMessage response,
        CancellationToken cancellationToken)
    {
        var content = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
        var error = HttpErrorMapper.FromHttpResponse(
            response.StatusCode,
            content,
            response.ReasonPhrase,
            response.Headers.RetryAfter);

        return new Result<T>.Failure(error);
    }
}
