namespace CivitaiSharp.Sdk.Http;

using System;
using Microsoft.Extensions.Logging;

/// <summary>
/// High-performance, AOT-compatible logging messages for the SDK HTTP client.
/// Uses source-generated LoggerMessage for zero-allocation logging at runtime.
/// </summary>
internal static partial class LogMessages
{
    // Debug level - HTTP request tracking
    [LoggerMessage(
        EventId = 1001,
        Level = LogLevel.Debug,
        Message = "{Method} {Uri}")]
    public static partial void LogRequest(ILogger logger, string method, string uri);

    // Warning level - Timeouts
    [LoggerMessage(
        EventId = 2001,
        Level = LogLevel.Warning,
        Message = "Request timed out for {Method} {Uri}")]
    public static partial void LogRequestTimeout(ILogger logger, Exception exception, string method, string uri);

    [LoggerMessage(
        EventId = 2002,
        Level = LogLevel.Warning,
        Message = "Resilience timeout exceeded for {Method} {Uri}")]
    public static partial void LogResilienceTimeout(ILogger logger, Exception exception, string method, string uri);

    // Error level - HTTP failures
    [LoggerMessage(
        EventId = 3001,
        Level = LogLevel.Error,
        Message = "HTTP request failed for {Method} {Uri}")]
    public static partial void LogHttpRequestFailed(ILogger logger, Exception exception, string method, string uri);

    // Warning level - Cloudflare/CDN errors
    [LoggerMessage(
        EventId = 2003,
        Level = LogLevel.Warning,
        Message = "Cloudflare error page received for {Method} {Uri}. Status: {StatusCode}, Type: {ErrorType}. The origin server may be unreachable or rate limiting is active at the CDN level.")]
    public static partial void LogCloudflareError(ILogger logger, string method, string uri, int statusCode, string errorType);

    [LoggerMessage(
        EventId = 2004,
        Level = LogLevel.Warning,
        Message = "Received non-JSON response for {Method} {Uri}. Content-Type: {ContentType}. Expected JSON response from API.")]
    public static partial void LogUnexpectedContentType(ILogger logger, string method, string uri, string contentType);
}
