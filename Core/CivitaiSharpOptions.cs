namespace CivitaiSharp.Core;

using System;

/// <summary>
/// Configuration options for the Core library (public API).
/// </summary>
public sealed class ApiOptions
{
    /// <summary>
    /// Default base URL for the Civitai API.
    /// </summary>
    public const string DefaultBaseUrl = "https://civitai.com";

    /// <summary>
    /// Default API version.
    /// </summary>
    public const string DefaultVersion = "v1";

    /// <summary>
    /// Default timeout in seconds for HTTP requests.
    /// </summary>
    public const int DefaultTimeoutSeconds = 30;

    /// <summary>
    /// Maximum allowed timeout in seconds.
    /// </summary>
    public const int MaxTimeoutSeconds = 300;

    /// <summary>
    /// Optional API key for authenticated requests. Required for favorites, and hidden models, and NSFW models.
    /// Obtain from: <see href="https://civitai.com/user/account"/>
    /// </summary>
    public string? Key { get; set; }

    /// <summary>
    /// API version path segment (e.g., "v1").
    /// </summary>
    public string Version { get; set; } = DefaultVersion;

    /// <summary>
    /// Timeout in seconds for HTTP requests. Must be between 1 and 300 seconds.
    /// </summary>
    public int TimeoutSeconds { get; set; } = DefaultTimeoutSeconds;

    /// <summary>
    /// Gets the timeout as a <see cref="TimeSpan"/>.
    /// </summary>
    public TimeSpan Timeout => TimeSpan.FromSeconds(TimeoutSeconds);

    /// <summary>
    /// Validates configuration and normalizes values.
    /// </summary>
    public void Validate()
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(Version);
        ArgumentOutOfRangeException.ThrowIfLessThan(TimeoutSeconds, 1);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(TimeoutSeconds, MaxTimeoutSeconds);
    }

    /// <summary>
    /// Constructs the full API path for a relative endpoint.
    /// </summary>
    public string GetApiPath(string relativePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(relativePath);
        return $"/api/{Version}/{relativePath.TrimStart('/')}";
    }
}

/// <summary>
/// Configuration options for the Sdk library (Generator/Orchestration API).
/// </summary>
public sealed class SdkOptions
{
    /// <summary>
    /// Default base URL for the Civitai Orchestration API.
    /// </summary>
    public const string DefaultBaseUrl = "https://orchestration.civitai.com";

    /// <summary>
    /// Default API version path segment.
    /// </summary>
    public const string DefaultVersion = "v1";

    /// <summary>
    /// Default timeout in seconds for HTTP requests. Set to 10 minutes to accommodate long-running generation jobs.
    /// </summary>
    public const int DefaultTimeoutSeconds = 600;

    /// <summary>
    /// Maximum allowed timeout in seconds.
    /// </summary>
    public const int MaxTimeoutSeconds = 1800;

    /// <summary>
    /// API token for authentication. Required for all Sdk operations.
    /// Obtain from: <see href="https://civitai.com/user/account"/>
    /// </summary>
    public string? Key { get; set; }

    /// <summary>
    /// API version path segment (e.g., "v1").
    /// </summary>
    public string Version { get; set; } = DefaultVersion;

    /// <summary>
    /// Timeout in seconds for HTTP requests. Must be between 1 and 1800 seconds.
    /// Default is 600 seconds (10 minutes) to accommodate long-running generation jobs.
    /// </summary>
    public int TimeoutSeconds { get; set; } = DefaultTimeoutSeconds;

    /// <summary>
    /// Gets the timeout as a <see cref="TimeSpan"/>.
    /// </summary>
    public TimeSpan Timeout => TimeSpan.FromSeconds(TimeoutSeconds);

    /// <summary>
    /// Validates configuration and normalizes values.
    /// </summary>
    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(Key))
        {
            throw new InvalidOperationException(
                "CivitaiSharp:Sdk:Key is required. Obtain your API token from https://civitai.com/user/account");
        }

        ArgumentException.ThrowIfNullOrWhiteSpace(Version);
        ArgumentOutOfRangeException.ThrowIfLessThan(TimeoutSeconds, 1);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(TimeoutSeconds, MaxTimeoutSeconds);
    }

    /// <summary>
    /// Constructs the full API path for a relative endpoint.
    /// </summary>
    public string GetApiPath(string relativePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(relativePath);
        return $"/{Version}/consumer/{relativePath.TrimStart('/')}";
    }
}

/// <summary>
/// Unified configuration options for CivitaiSharp libraries.
/// </summary>
/// <remarks>
/// <para>
/// This class centralizes configuration for both Core and Sdk libraries.
/// Configure this class in the <c>CivitaiSharp</c> section of your configuration file.
/// </para>
/// </remarks>
/// <example>
/// <code>
/// // appsettings.json:
/// {
///   "CivitaiSharp": {
///     "Api": {
///       "Key": "your-public-api-key",
///       "Version": "v1",
///       "TimeoutSeconds": 30
///     },
///     "Sdk": {
///       "Key": "your-generator-api-token",
///       "Version": "v1",
///       "TimeoutSeconds": 600
///     }
///   }
/// }
/// </code>
/// </example>
public sealed class CivitaiSharpOptions
{
    /// <summary>
    /// Configuration for the Core library (public API).
    /// </summary>
    public ApiOptions Api { get; set; } = new();

    /// <summary>
    /// Configuration for the Sdk library (Generator/Orchestration API).
    /// </summary>
    public SdkOptions Sdk { get; set; } = new();
}
