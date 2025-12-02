namespace CivitaiSharp.Core;

using System;

/// <summary>
/// Options for configuring the API client.
/// </summary>
public sealed class ApiClientOptions
{
    /// <summary>
    /// Default base URL for the Civitai API.
    /// </summary>
    public const string DefaultBaseUrl = "https://civitai.com";

    /// <summary>
    /// Default API version.
    /// </summary>
    public const string DefaultApiVersion = "v1";

    /// <summary>
    /// Default timeout in seconds for HTTP requests.
    /// </summary>
    public const int DefaultTimeoutSeconds = 30;

    /// <summary>
    /// Maximum allowed timeout in seconds.
    /// </summary>
    public const int MaxTimeoutSeconds = 300;

    private string _baseUrl = DefaultBaseUrl;
    private string _apiVersion = DefaultApiVersion;
    private int _timeoutSeconds = DefaultTimeoutSeconds;

    /// <summary>
    /// Base URL for the Civitai API.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown when value is null, whitespace, or not a valid absolute URI.</exception>
    public string BaseUrl
    {
        get => _baseUrl;
        set
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(value);
            if (!Uri.TryCreate(value, UriKind.Absolute, out var uri) ||
                (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
            {
                throw new ArgumentException("BaseUrl must be a valid absolute HTTP or HTTPS URI.", nameof(value));
            }
            _baseUrl = value.TrimEnd('/');
        }
    }

    /// <summary>
    /// API version path segment (e.g., "v1").
    /// </summary>
    /// <exception cref="ArgumentException">Thrown when value is null or whitespace.</exception>
    public string ApiVersion
    {
        get => _apiVersion;
        set
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(value);
            _apiVersion = value;
        }
    }

    /// <summary>
    /// Optional API key for authenticated requests.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The Core library can query public endpoints (models, images, tags, creators) without an API key.
    /// An API key is only required for authenticated features like favorites, hidden models, and higher rate limits.
    /// </para>
    /// <para>
    /// This is different from <c>CivitaiSharp.Sdk</c> which always requires an API token for all operations.
    /// </para>
    /// </remarks>
    public string? ApiKey { get; set; }

    /// <summary>
    /// Timeout in seconds for HTTP requests. Must be between 1 and 300 seconds.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when value is less than 1 or greater than 300.</exception>
    public int TimeoutSeconds
    {
        get => _timeoutSeconds;
        set
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(value, 1);
            ArgumentOutOfRangeException.ThrowIfGreaterThan(value, MaxTimeoutSeconds);
            _timeoutSeconds = value;
        }
    }

    /// <summary>
    /// Gets the timeout as a <see cref="TimeSpan"/>.
    /// </summary>
    public TimeSpan Timeout => TimeSpan.FromSeconds(_timeoutSeconds);

    /// <summary>
    /// Constructs the full API path for a relative endpoint.
    /// </summary>
    /// <param name="relativePath">The relative endpoint path (e.g., "models", "images").</param>
    /// <returns>The full API path including version prefix.</returns>
    /// <exception cref="ArgumentException">Thrown if relativePath is null or whitespace.</exception>
    public string GetApiPath(string relativePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(relativePath);
        return $"/api/{_apiVersion}/{relativePath.TrimStart('/')}";
    }
}
