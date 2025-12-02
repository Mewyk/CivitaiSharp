namespace CivitaiSharp.Core.Response;

using System;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using CivitaiSharp.Core.Json;

/// <summary>
/// Provides utilities for mapping HTTP responses to <see cref="Error"/> instances. Centralizes HTTP error
/// handling logic including RFC 7807 problem details parsing for consistent behavior across CivitaiSharp libraries.
/// </summary>
/// <remarks>
/// <para>
/// This class is public to allow custom HTTP client implementations to use the same error mapping logic.
/// </para>
/// <para>
/// When parsing RFC 7807 error responses, only the standard fields useful for client-side error handling
/// are extracted (type, title, status, detail, instance, errors, traceId). Custom RFC 7807 extension fields
/// are intentionally not exposed as they are typically server-implementation-specific and not actionable
/// by client code.
/// </para>
/// </remarks>
public static class HttpErrorMapper
{
    /// <summary>
    /// Maps an HTTP status code to the corresponding <see cref="ErrorCode"/>.
    /// </summary>
    /// <param name="statusCode">The HTTP status code to map.</param>
    /// <returns>The corresponding error code.</returns>
    public static ErrorCode MapStatusCode(HttpStatusCode statusCode) => statusCode switch
    {
        // 2xx - Success codes that indicate issues
        HttpStatusCode.NoContent => ErrorCode.NoContent,

        // 3xx - Redirection
        HttpStatusCode.MovedPermanently => ErrorCode.MovedPermanently,
        HttpStatusCode.TemporaryRedirect => ErrorCode.TemporaryRedirect,
        HttpStatusCode.PermanentRedirect => ErrorCode.PermanentRedirect,

        // 4xx - Client errors
        HttpStatusCode.BadRequest => ErrorCode.BadRequest,
        HttpStatusCode.Unauthorized => ErrorCode.Unauthorized,
        HttpStatusCode.PaymentRequired => ErrorCode.InsufficientCredits,
        HttpStatusCode.Forbidden => ErrorCode.Forbidden,
        HttpStatusCode.NotFound => ErrorCode.NotFound,
        HttpStatusCode.MethodNotAllowed => ErrorCode.MethodNotAllowed,
        HttpStatusCode.NotAcceptable => ErrorCode.NotAcceptable,
        HttpStatusCode.Conflict => ErrorCode.Conflict,
        HttpStatusCode.UnprocessableEntity => ErrorCode.ValidationFailed,
        HttpStatusCode.TooManyRequests => ErrorCode.RateLimited,
        HttpStatusCode.RequestHeaderFieldsTooLarge => ErrorCode.RequestHeaderFieldsTooLarge,
        HttpStatusCode.UnavailableForLegalReasons => ErrorCode.UnavailableForLegalReasons,

        // 5xx - Server errors
        HttpStatusCode.InternalServerError => ErrorCode.ServerError,
        HttpStatusCode.NotImplemented => ErrorCode.NotImplemented,
        HttpStatusCode.BadGateway => ErrorCode.BadGateway,
        HttpStatusCode.ServiceUnavailable => ErrorCode.ServiceUnavailable,
        HttpStatusCode.GatewayTimeout => ErrorCode.GatewayTimeout,
        HttpStatusCode.HttpVersionNotSupported => ErrorCode.HttpVersionNotSupported,
        HttpStatusCode.LoopDetected => ErrorCode.LoopDetected,

        _ => ErrorCode.HttpError
    };

    /// <summary>
    /// Extracts the Retry-After duration from response headers.
    /// </summary>
    /// <param name="retryAfter">The Retry-After header value.</param>
    /// <returns>The retry delay, or null if not specified.</returns>
    public static TimeSpan? GetRetryAfter(RetryConditionHeaderValue? retryAfter)
    {
        if (retryAfter is null)
        {
            return null;
        }

        if (retryAfter.Delta.HasValue)
        {
            return retryAfter.Delta.Value;
        }

        if (retryAfter.Date.HasValue)
        {
            var delay = retryAfter.Date.Value - DateTimeOffset.UtcNow;
            return delay > TimeSpan.Zero ? delay : TimeSpan.Zero;
        }

        return null;
    }

    /// <summary>
    /// Creates an <see cref="Error"/> from an HTTP response.
    /// </summary>
    /// <param name="statusCode">The HTTP status code.</param>
    /// <param name="content">The response body content, if available.</param>
    /// <param name="reasonPhrase">The HTTP reason phrase, if available.</param>
    /// <param name="retryAfter">The Retry-After header value, if available.</param>
    /// <returns>An error representing the HTTP response.</returns>
    public static Error FromHttpResponse(
        HttpStatusCode statusCode,
        string? content,
        string? reasonPhrase,
        RetryConditionHeaderValue? retryAfter)
    {
        var errorCode = MapStatusCode(statusCode);
        var retryDelay = GetRetryAfter(retryAfter);

        // Try to parse as RFC 7807 API error response
        if (!string.IsNullOrWhiteSpace(content))
        {
            try
            {
                var apiError = JsonSerializer.Deserialize(content, CivitaiJsonContext.Default.ApiErrorResponse);
                if (apiError is not null)
                {
                    return FromApiErrorResponse(apiError, statusCode, retryDelay);
                }
            }
            catch (JsonException)
            {
                // Not valid JSON or not API error format; fall through to plain text handling
            }

            // Use raw content as message
            return new Error(
                errorCode,
                content,
                HttpStatusCode: statusCode,
                RetryAfter: retryDelay);
        }

        // Fall back to reason phrase or generic message
        var message = BuildFallbackMessage(statusCode, reasonPhrase, retryDelay);
        return new Error(
            errorCode,
            message,
            HttpStatusCode: statusCode,
            RetryAfter: retryDelay);
    }

    /// <summary>
    /// Creates an <see cref="Error"/> from an <see cref="HttpRequestException"/>.
    /// </summary>
    /// <param name="exception">The HTTP request exception.</param>
    /// <returns>An error representing the exception.</returns>
    public static Error FromHttpRequestException(HttpRequestException exception)
    {
        ArgumentNullException.ThrowIfNull(exception);

        return new Error(
            ErrorCode.HttpError,
            $"HTTP request failed: {exception.Message}",
            InnerException: exception,
            HttpStatusCode: exception.StatusCode);
    }

    /// <summary>
    /// Creates an <see cref="Error"/> for a timeout.
    /// </summary>
    /// <param name="exception">The exception that caused the timeout.</param>
    /// <returns>An error representing the timeout.</returns>
    public static Error FromTimeout(Exception? exception = null) =>
        new(ErrorCode.Timeout, "The request timed out.", InnerException: exception);

    private static Error FromApiErrorResponse(
        ApiErrorResponse apiError,
        HttpStatusCode statusCode,
        TimeSpan? retryAfter)
    {
        var errorCode = MapStatusCode(statusCode);
        var message = apiError.Detail ?? apiError.Title ?? "Unknown error";

        // Use ValidationFailed if there are field-level errors
        if (apiError.Errors is { Count: > 0 })
        {
            errorCode = ErrorCode.ValidationFailed;
        }

        return new Error(
            errorCode,
            message,
            Details: apiError.Errors,
            HttpStatusCode: statusCode,
            RetryAfter: retryAfter,
            TraceId: apiError.TraceId);
    }

    private static string BuildFallbackMessage(
        HttpStatusCode statusCode,
        string? reasonPhrase,
        TimeSpan? retryAfter)
    {
        var baseMessage = !string.IsNullOrWhiteSpace(reasonPhrase)
            ? reasonPhrase
            : GetDefaultMessageForStatusCode(statusCode);

        if (retryAfter.HasValue && statusCode == HttpStatusCode.TooManyRequests)
        {
            var seconds = retryAfter.Value.TotalSeconds;
            return $"{baseMessage.TrimEnd('.')} Retry after {seconds:F0}s.";
        }

        return baseMessage;
    }

    private static string GetDefaultMessageForStatusCode(HttpStatusCode statusCode) => statusCode switch
    {
        // 2xx
        HttpStatusCode.NoContent => "The server returned no content (204).",

        // 3xx
        HttpStatusCode.MovedPermanently => "The resource has been permanently moved (301).",
        HttpStatusCode.TemporaryRedirect => "The resource has been temporarily redirected (307).",
        HttpStatusCode.PermanentRedirect => "The resource has been permanently redirected (308).",

        // 4xx
        HttpStatusCode.BadRequest => "Bad request. The server could not understand the request.",
        HttpStatusCode.Unauthorized => "Unauthorized. Check API key or permissions.",
        HttpStatusCode.PaymentRequired => "Payment required. Insufficient credits.",
        HttpStatusCode.Forbidden => "Forbidden. You do not have permission to access this resource.",
        HttpStatusCode.NotFound => "Not found. The requested resource does not exist.",
        HttpStatusCode.MethodNotAllowed => "Method not allowed. The HTTP method is not supported for this endpoint.",
        HttpStatusCode.NotAcceptable => "Not acceptable. The server cannot produce a response matching the Accept headers.",
        HttpStatusCode.Conflict => "Conflict. The request conflicts with the current state of the resource.",
        HttpStatusCode.UnprocessableEntity => "Validation failed. Check the request parameters.",
        HttpStatusCode.TooManyRequests => "Rate limit exceeded.",
        HttpStatusCode.RequestHeaderFieldsTooLarge => "Request header fields too large.",
        HttpStatusCode.UnavailableForLegalReasons => "Unavailable for legal reasons (451).",

        // 5xx
        HttpStatusCode.InternalServerError => "Internal server error.",
        HttpStatusCode.NotImplemented => "Not implemented. The server does not support the requested functionality.",
        HttpStatusCode.BadGateway => "Bad gateway. The server received an invalid response from an upstream server.",
        HttpStatusCode.ServiceUnavailable => "Service unavailable. The server is temporarily overloaded or under maintenance.",
        HttpStatusCode.GatewayTimeout => "Gateway timeout. The server did not receive a timely response from an upstream server.",
        HttpStatusCode.HttpVersionNotSupported => "HTTP version not supported.",
        HttpStatusCode.LoopDetected => "Loop detected. The server detected an infinite loop while processing the request.",

        _ => $"HTTP error {(int)statusCode}."
    };

    /// <summary>
    /// Detects the type of Cloudflare error page from the response content.
    /// </summary>
    /// <param name="content">The HTML content of the response.</param>
    /// <returns>The detected Cloudflare error type, or null if not a Cloudflare error.</returns>
    public static string? DetectCloudflareErrorType(string content)
    {
        // Check for Cloudflare-specific markers
        if (!content.Contains("cloudflare", StringComparison.OrdinalIgnoreCase) &&
            !content.Contains("cf-ray", StringComparison.OrdinalIgnoreCase) &&
            !content.Contains("__cf_", StringComparison.OrdinalIgnoreCase))
        {
            // Check for generic HTML that might indicate a proxy/CDN error
            if (content.Contains("<!DOCTYPE", StringComparison.OrdinalIgnoreCase) ||
                content.Contains("<html", StringComparison.OrdinalIgnoreCase))
            {
                return "HTML response (possible proxy or CDN error)";
            }

            return null;
        }

        // Detect specific Cloudflare error types
        if (content.Contains("Error 520", StringComparison.OrdinalIgnoreCase) ||
            content.Contains("Web server is returning an unknown error", StringComparison.OrdinalIgnoreCase))
        {
            return "520 - Web server returned an unknown error";
        }

        if (content.Contains("Error 521", StringComparison.OrdinalIgnoreCase) ||
            content.Contains("Web server is down", StringComparison.OrdinalIgnoreCase))
        {
            return "521 - Origin server is down";
        }

        if (content.Contains("Error 522", StringComparison.OrdinalIgnoreCase) ||
            content.Contains("Connection timed out", StringComparison.OrdinalIgnoreCase))
        {
            return "522 - Connection to origin server timed out";
        }

        if (content.Contains("Error 523", StringComparison.OrdinalIgnoreCase) ||
            content.Contains("Origin is unreachable", StringComparison.OrdinalIgnoreCase))
        {
            return "523 - Origin server is unreachable";
        }

        if (content.Contains("Error 524", StringComparison.OrdinalIgnoreCase) ||
            content.Contains("A timeout occurred", StringComparison.OrdinalIgnoreCase))
        {
            return "524 - Origin server response timed out";
        }

        if (content.Contains("Error 525", StringComparison.OrdinalIgnoreCase) ||
            content.Contains("SSL handshake failed", StringComparison.OrdinalIgnoreCase))
        {
            return "525 - SSL handshake with origin server failed";
        }

        if (content.Contains("Error 526", StringComparison.OrdinalIgnoreCase) ||
            content.Contains("Invalid SSL certificate", StringComparison.OrdinalIgnoreCase))
        {
            return "526 - Invalid SSL certificate on origin server";
        }

        if (content.Contains("Error 527", StringComparison.OrdinalIgnoreCase) ||
            content.Contains("Railgun Listener", StringComparison.OrdinalIgnoreCase))
        {
            return "527 - Railgun connection error";
        }

        if (content.Contains("Error 530", StringComparison.OrdinalIgnoreCase))
        {
            return "530 - Origin DNS error or Cloudflare error";
        }

        if (content.Contains("Checking your browser", StringComparison.OrdinalIgnoreCase) ||
            content.Contains("challenge-platform", StringComparison.OrdinalIgnoreCase) ||
            content.Contains("Just a moment", StringComparison.OrdinalIgnoreCase))
        {
            return "Browser challenge - DDoS protection or bot detection triggered";
        }

        if (content.Contains("Access denied", StringComparison.OrdinalIgnoreCase) ||
            content.Contains("Error 1015", StringComparison.OrdinalIgnoreCase))
        {
            return "1015 - Rate limited by Cloudflare";
        }

        if (content.Contains("Error 1020", StringComparison.OrdinalIgnoreCase) ||
            content.Contains("firewall", StringComparison.OrdinalIgnoreCase))
        {
            return "1020 - Blocked by Cloudflare firewall rules";
        }

        // Generic Cloudflare error
        return "Cloudflare error page";
    }

    /// <summary>
    /// Builds a detailed error message for Cloudflare errors.
    /// </summary>
    /// <param name="errorType">The detected Cloudflare error type.</param>
    /// <param name="statusCode">The HTTP status code.</param>
    /// <param name="contentType">The content type of the response.</param>
    /// <returns>A formatted error message.</returns>
    public static string BuildCloudflareErrorMessage(string errorType, int statusCode, string contentType)
    {
        return $"Cloudflare error: {errorType}. " +
               $"HTTP Status: {statusCode}, Content-Type: {contentType}. " +
               "The Civitai origin server may be experiencing issues. Please try again later.";
    }
}
