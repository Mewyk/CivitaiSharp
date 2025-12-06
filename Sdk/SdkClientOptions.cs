namespace CivitaiSharp.Sdk;

using System;

/// <summary>
/// Configuration options for the Civitai Generator SDK client (orchestration endpoints).
/// Unlike the public API used by <c>CivitaiSharp.Core</c>, the Generator API requires authentication
/// for all requests, so the <see cref="ApiToken"/> property must be set before the client can be used.
/// </summary>
public sealed class SdkClientOptions
{
    /// <summary>
    /// Default base URL for the Civitai Orchestration API.
    /// </summary>
    public const string DefaultBaseUrl = "https://orchestration.civitai.com";

    /// <summary>
    /// Default API version path segment.
    /// </summary>
    public const string DefaultApiVersion = "v1";

    /// <summary>
    /// Default timeout in seconds for HTTP requests.
    /// Set to 10 minutes to accommodate long-running generation jobs.
    /// </summary>
    public const int DefaultTimeoutSeconds = 600;

    /// <summary>
    /// Maximum allowed timeout in seconds.
    /// </summary>
    public const int MaxTimeoutSeconds = 1800;

    private string _baseUrl = DefaultBaseUrl;
    private string _apiVersion = DefaultApiVersion;
    private string _apiToken = string.Empty;
    private int _timeoutSeconds = DefaultTimeoutSeconds;

    /// <summary>
    /// Gets or sets the API token for authentication.
    /// This is required for all SDK operations.
    /// </summary>
    /// <remarks>
    /// Obtain your API token from https://civitai.com/user/account
    /// </remarks>
    /// <exception cref="ArgumentNullException">Thrown when value is null.</exception>
    /// <exception cref="ArgumentException">Thrown when value is empty or whitespace.</exception>
    public required string ApiToken
    {
        get => _apiToken;
        set
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(value);
            _apiToken = value;
        }
    }

    /// <summary>
    /// Gets or sets the base URL for the Civitai Orchestration API.
    /// </summary>
    /// <exception cref="ArgumentNullException">Thrown when value is null.</exception>
    /// <exception cref="ArgumentException">Thrown when value is empty, whitespace, or not a valid absolute URI.</exception>
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
    /// Gets or sets the API version path segment (e.g., "v1").
    /// </summary>
    /// <exception cref="ArgumentNullException">Thrown when value is null.</exception>
    /// <exception cref="ArgumentException">Thrown when value is empty or whitespace.</exception>
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
    /// Gets or sets the timeout in seconds for HTTP requests.
    /// Must be between 1 and 1800 seconds (30 minutes).
    /// </summary>
    /// <remarks>
    /// The default is 600 seconds (10 minutes) to accommodate long-running
    /// generation jobs when using the <c>wait=true</c> parameter.
    /// </remarks>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when value is less than 1 or greater than 1800.</exception>
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
    /// Validates that all required options are properly configured.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when required options are missing or invalid.</exception>
    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(_apiToken))
        {
            throw new InvalidOperationException(
                $"{nameof(ApiToken)} is required. Obtain your API token from https://civitai.com/user/account");
        }
    }

    /// <summary>
    /// Constructs the full API path for a relative endpoint.
    /// </summary>
    /// <param name="relativePath">The relative endpoint path (e.g., "jobs", "coverage").</param>
    /// <returns>The full API path including version prefix and consumer path.</returns>
    /// <exception cref="ArgumentNullException">Thrown if relativePath is null.</exception>
    /// <exception cref="ArgumentException">Thrown if relativePath is empty or whitespace.</exception>
    public string GetApiPath(string relativePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(relativePath);
        return $"/{_apiVersion}/consumer/{relativePath.TrimStart('/')}";
    }
}
