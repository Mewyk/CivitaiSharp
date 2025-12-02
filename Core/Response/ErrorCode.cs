namespace CivitaiSharp.Core.Response;

/// <summary>
/// Categorized error codes for operations across the CivitaiSharp libraries.
/// </summary>
/// <remarks>
/// <para>
/// Error codes are organized by category using numeric ranges:
/// <list type="bullet">
/// <item><strong>0-99:</strong> General/Unknown errors</item>
/// <item><strong>100-199:</strong> HTTP and network errors</item>
/// <item><strong>200-299:</strong> Authentication and authorization errors</item>
/// <item><strong>300-399:</strong> Rate limiting and quota errors</item>
/// <item><strong>400-499:</strong> Validation and input errors</item>
/// <item><strong>500-599:</strong> Serialization and parsing errors</item>
/// <item><strong>600-699:</strong> File and I/O errors</item>
/// <item><strong>700-799:</strong> Resource state errors</item>
/// </list>
/// </para>
/// </remarks>
public enum ErrorCode
{
    /// <summary>
    /// An unknown or unclassified error occurred.
    /// </summary>
    Unknown = 0,

    /// <summary>
    /// A general HTTP communication error occurred.
    /// </summary>
    HttpError = 100,

    /// <summary>
    /// The HTTP request timed out before completing.
    /// </summary>
    Timeout = 101,

    /// <summary>
    /// The server returned a bad request (400) response.
    /// </summary>
    BadRequest = 102,

    /// <summary>
    /// The requested resource was not found (404).
    /// </summary>
    NotFound = 103,

    /// <summary>
    /// A conflict occurred with the current state of the resource (409).
    /// </summary>
    Conflict = 104,

    /// <summary>
    /// The server encountered an internal error (500).
    /// </summary>
    ServerError = 105,

    /// <summary>
    /// The server returned a bad gateway response (502).
    /// </summary>
    BadGateway = 106,

    /// <summary>
    /// The service is temporarily unavailable (503).
    /// </summary>
    ServiceUnavailable = 107,

    /// <summary>
    /// The gateway timed out waiting for a response (504).
    /// </summary>
    GatewayTimeout = 108,

    /// <summary>
    /// The HTTP method is not allowed for the requested resource (405).
    /// </summary>
    MethodNotAllowed = 109,

    /// <summary>
    /// The server cannot produce a response matching the Accept headers (406).
    /// </summary>
    NotAcceptable = 110,

    /// <summary>
    /// The request was successful but the response contains no content (204).
    /// </summary>
    NoContent = 111,

    /// <summary>
    /// The resource has been permanently moved to a new location (301).
    /// </summary>
    MovedPermanently = 112,

    /// <summary>
    /// The resource has been temporarily redirected (307).
    /// </summary>
    TemporaryRedirect = 113,

    /// <summary>
    /// The resource has been permanently redirected (308).
    /// </summary>
    PermanentRedirect = 114,

    /// <summary>
    /// The resource is unavailable for legal reasons (451).
    /// </summary>
    UnavailableForLegalReasons = 115,

    /// <summary>
    /// The request header fields are too large (431).
    /// </summary>
    RequestHeaderFieldsTooLarge = 116,

    /// <summary>
    /// The server does not support the requested functionality (501).
    /// </summary>
    NotImplemented = 117,

    /// <summary>
    /// The HTTP version used in the request is not supported (505).
    /// </summary>
    HttpVersionNotSupported = 118,

    /// <summary>
    /// A loop was detected while processing the request (508).
    /// </summary>
    LoopDetected = 119,

    /// <summary>
    /// Authentication failed or credentials are missing/invalid (401).
    /// </summary>
    Unauthorized = 200,

    /// <summary>
    /// The authenticated user lacks permission for this operation (403).
    /// </summary>
    Forbidden = 201,

    /// <summary>
    /// The account has insufficient credits to complete the operation (402).
    /// </summary>
    InsufficientCredits = 202,

    /// <summary>
    /// Too many requests have been made; rate limit exceeded (429).
    /// </summary>
    RateLimited = 300,

    /// <summary>
    /// A provided URL was invalid or malformed.
    /// </summary>
    InvalidUrl = 400,

    /// <summary>
    /// A required parameter was missing or invalid.
    /// </summary>
    InvalidParameter = 401,

    /// <summary>
    /// Request validation failed. Check <see cref="Error.Details"/> for field-level errors.
    /// </summary>
    ValidationFailed = 402,

    /// <summary>
    /// Failed to deserialize a response from JSON.
    /// </summary>
    DeserializationFailed = 500,

    /// <summary>
    /// The response was empty or null when data was expected.
    /// </summary>
    EmptyResponse = 501,

    /// <summary>
    /// The response was not in JSON format (possibly an HTML response).
    /// </summary>
    UnexpectedContentType = 502,

    /// <summary>
    /// The response was a Cloudflare error page, indicating the origin server is unreachable or blocked.
    /// This typically occurs during Civitai outages, DDoS protection triggers, or when rate limited at the CDN level.
    /// </summary>
    CloudflareError = 503,

    /// <summary>
    /// The specified file was not found.
    /// </summary>
    FileNotFound = 600,

    /// <summary>
    /// A file I/O operation failed.
    /// </summary>
    IoError = 601,

    /// <summary>
    /// A stream was not readable when read access was required.
    /// </summary>
    StreamNotReadable = 602,

    /// <summary>
    /// A hash computation operation failed.
    /// </summary>
    HashComputationFailed = 603,

    /// <summary>
    /// Failed to write data to a file.
    /// </summary>
    FileWriteFailed = 604,

    /// <summary>
    /// A requested resource is not available or not ready.
    /// </summary>
    ResourceUnavailable = 700
}
