namespace CivitaiSharp.Sdk.Request;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using CivitaiSharp.Core;
using CivitaiSharp.Core.Response;
using CivitaiSharp.Sdk.Http;
using CivitaiSharp.Sdk.Json;
using CivitaiSharp.Sdk.Models.Jobs;
using CivitaiSharp.Sdk.Models.Results;

/// <summary>
/// Immutable, thread-safe builder for querying and managing job status.
/// Each fluent method returns a new builder instance, allowing safe reuse and caching of base configurations.
/// </summary>
public sealed record JobQueryBuilder
{
    private readonly SdkHttpClient _httpClient;
    private readonly SdkOptions _options;
    private readonly bool _wait;
    private readonly bool _detailed;
    private readonly ImmutableDictionary<string, JsonElement>? _propertyFilters;

    /// <summary>
    /// Initializes a new instance of the <see cref="JobQueryBuilder"/> class.
    /// Internal to enforce creation through JobsBuilder.
    /// </summary>
    /// <param name="httpClient">The HTTP client used to execute requests.</param>
    /// <param name="options">The SDK client options.</param>
    /// <exception cref="ArgumentNullException">Thrown when httpClient or options is null.</exception>
    internal JobQueryBuilder(SdkHttpClient httpClient, SdkOptions options)
        : this(
            httpClient ?? throw new ArgumentNullException(nameof(httpClient)),
            options ?? throw new ArgumentNullException(nameof(options)),
            wait: false,
            detailed: false,
            propertyFilters: null)
    {
    }

    private JobQueryBuilder(
        SdkHttpClient httpClient,
        SdkOptions options,
        bool wait,
        bool detailed,
        ImmutableDictionary<string, JsonElement>? propertyFilters)
    {
        _httpClient = httpClient;
        _options = options;
        _wait = wait;
        _detailed = detailed;
        _propertyFilters = propertyFilters;
    }

    /// <summary>
    /// Configures the query to include detailed job specifications in responses.
    /// </summary>
    /// <returns>A new builder instance with detailed mode enabled.</returns>
    public JobQueryBuilder WithDetailed()
        => new(_httpClient, _options, _wait, detailed: true, _propertyFilters);

    /// <summary>
    /// Configures the query to wait for job completion (blocks up to ~10 minutes).
    /// </summary>
    /// <returns>A new builder instance with wait mode enabled.</returns>
    /// <remarks>
    /// When enabled, the API will block until jobs complete or timeout (~10 minutes).
    /// This is useful when you need immediate results without manual polling.
    /// </remarks>
    public JobQueryBuilder WithWait()
        => new(_httpClient, _options, wait: true, _detailed, _propertyFilters);

    /// <summary>
    /// Adds a custom string property filter to the query.
    /// </summary>
    /// <param name="key">The property key to filter by.</param>
    /// <param name="value">The string value to match.</param>
    /// <returns>A new builder instance with the added property filter.</returns>
    public JobQueryBuilder WhereProperty(string key, string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        var element = JsonSerializer.SerializeToElement(value, SdkJsonContext.Default.String);
        var filters = _propertyFilters ?? [];
        return new(_httpClient, _options, _wait, _detailed, filters.SetItem(key, element));
    }

    /// <summary>
    /// Adds a custom integer property filter to the query.
    /// </summary>
    /// <param name="key">The property key to filter by.</param>
    /// <param name="value">The integer value to match.</param>
    /// <returns>A new builder instance with the added property filter.</returns>
    public JobQueryBuilder WhereProperty(string key, int value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        var element = JsonSerializer.SerializeToElement(value, SdkJsonContext.Default.Int32);
        var filters = _propertyFilters ?? [];
        return new(_httpClient, _options, _wait, _detailed, filters.SetItem(key, element));
    }

    /// <summary>
    /// Adds a custom long property filter to the query.
    /// </summary>
    /// <param name="key">The property key to filter by.</param>
    /// <param name="value">The long value to match.</param>
    /// <returns>A new builder instance with the added property filter.</returns>
    public JobQueryBuilder WhereProperty(string key, long value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        var element = JsonSerializer.SerializeToElement(value, SdkJsonContext.Default.Int64);
        var filters = _propertyFilters ?? [];
        return new(_httpClient, _options, _wait, _detailed, filters.SetItem(key, element));
    }

    /// <summary>
    /// Adds a custom boolean property filter to the query.
    /// </summary>
    /// <param name="key">The property key to filter by.</param>
    /// <param name="value">The boolean value to match.</param>
    /// <returns>A new builder instance with the added property filter.</returns>
    public JobQueryBuilder WhereProperty(string key, bool value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        var element = JsonSerializer.SerializeToElement(value, SdkJsonContext.Default.Boolean);
        var filters = _propertyFilters ?? [];
        return new(_httpClient, _options, _wait, _detailed, filters.SetItem(key, element));
    }

    /// <summary>
    /// Adds a custom decimal property filter to the query.
    /// </summary>
    /// <param name="key">The property key to filter by.</param>
    /// <param name="value">The decimal value to match.</param>
    /// <returns>A new builder instance with the added property filter.</returns>
    public JobQueryBuilder WhereProperty(string key, decimal value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        var element = JsonSerializer.SerializeToElement(value, SdkJsonContext.Default.Decimal);
        var filters = _propertyFilters ?? [];
        return new(_httpClient, _options, _wait, _detailed, filters.SetItem(key, element));
    }

    /// <summary>
    /// Adds a custom GUID property filter to the query.
    /// </summary>
    /// <param name="key">The property key to filter by.</param>
    /// <param name="value">The GUID value to match.</param>
    /// <returns>A new builder instance with the added property filter.</returns>
    public JobQueryBuilder WhereProperty(string key, Guid value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        var element = JsonSerializer.SerializeToElement(value, SdkJsonContext.Default.Guid);
        var filters = _propertyFilters ?? [];
        return new(_httpClient, _options, _wait, _detailed, filters.SetItem(key, element));
    }

    /// <summary>
    /// Adds a custom DateTime property filter to the query.
    /// </summary>
    /// <param name="key">The property key to filter by.</param>
    /// <param name="value">The DateTime value to match.</param>
    /// <returns>A new builder instance with the added property filter.</returns>
    public JobQueryBuilder WhereProperty(string key, DateTime value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        var element = JsonSerializer.SerializeToElement(value, SdkJsonContext.Default.DateTime);
        var filters = _propertyFilters ?? [];
        return new(_httpClient, _options, _wait, _detailed, filters.SetItem(key, element));
    }

    /// <summary>
    /// Adds multiple custom property filters to the query.
    /// </summary>
    /// <param name="properties">Dictionary of property key-value pairs to filter by.</param>
    /// <returns>A new builder instance with the added property filters.</returns>
    /// <remarks>
    /// Multiple property filters are combined with AND logic - all must match.
    /// </remarks>
    public JobQueryBuilder WhereProperties(IReadOnlyDictionary<string, JsonElement> properties)
    {
        ArgumentNullException.ThrowIfNull(properties);
        
        if (properties.Count == 0)
        {
            return this;
        }

        var filters = _propertyFilters ?? [];
        foreach (var kvp in properties)
        {
            filters = filters.SetItem(kvp.Key, kvp.Value);
        }
        
        return new(_httpClient, _options, _wait, _detailed, filters);
    }

    /// <summary>
    /// Gets the status of a specific job by its unique identifier.
    /// </summary>
    /// <param name="jobId">The unique job identifier.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>A task containing the job status.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if jobId is empty.</exception>
    public Task<Result<JobStatus>> GetByIdAsync(Guid jobId, CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfEqual(jobId, Guid.Empty, nameof(jobId));

        var uri = BuildUri($"jobs/{jobId}");
        return _httpClient.GetAsync<JobStatus>(uri, cancellationToken);
    }

    /// <summary>
    /// Gets the status of jobs by their batch token from a previous submission.
    /// </summary>
    /// <param name="token">The batch token from a previous job submission.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>A task containing the job status collection.</returns>
    /// <exception cref="ArgumentException">Thrown if token is null or whitespace.</exception>
    public Task<Result<JobStatusCollection>> GetByTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(token);

        var uri = BuildUri("jobs", token: token);
        return _httpClient.GetAsync<JobStatusCollection>(uri, cancellationToken);
    }

    /// <summary>
    /// Executes a query for jobs matching the configured property filters.
    /// </summary>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>A task containing the matching jobs.</returns>
    /// <exception cref="InvalidOperationException">Thrown if no property filters are configured.</exception>
    /// <remarks>
    /// Use WhereProperty or <see cref="WhereProperties"/> to add filters before calling this method.
    /// </remarks>
    public Task<Result<JobStatusCollection>> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        if (_propertyFilters is null || _propertyFilters.Count == 0)
        {
            throw new InvalidOperationException("At least one property filter must be specified. Use WhereProperty() or WhereProperties() to add filters.");
        }

        var request = new QueryJobsRequest { Properties = _propertyFilters };
        var uri = BuildUri("jobs/query");
        return _httpClient.PostAsync<QueryJobsRequest, JobStatusCollection>(uri, request, cancellationToken);
    }

    /// <summary>
    /// Cancels a specific job by its unique identifier.
    /// </summary>
    /// <param name="jobId">The unique job identifier.</param>
    /// <param name="force">If true, cancels even if the job is processing. Default is true.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>A task representing the cancellation operation.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if jobId is empty.</exception>
    public Task<Result<Unit>> CancelAsync(Guid jobId, bool force = true, CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfEqual(jobId, Guid.Empty, nameof(jobId));

        var uri = BuildUri($"jobs/{jobId}", force: force);
        return _httpClient.DeleteAsync<Unit>(uri, cancellationToken);
    }

    /// <summary>
    /// Cancels jobs by their batch token.
    /// </summary>
    /// <param name="token">The batch token from a previous job submission.</param>
    /// <param name="force">If true, cancels even if jobs are processing. Default is true.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>A task representing the cancellation operation.</returns>
    /// <exception cref="ArgumentException">Thrown if token is null or whitespace.</exception>
    public Task<Result<Unit>> CancelAsync(string token, bool force = true, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(token);

        var uri = BuildUri("jobs", token: token, force: force);
        return _httpClient.DeleteAsync<Unit>(uri, cancellationToken);
    }

    /// <summary>
    /// Marks a specific job as tainted by its unique identifier.
    /// </summary>
    /// <param name="jobId">The unique job identifier.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>A task representing the taint operation.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if jobId is empty.</exception>
    public Task<Result<Unit>> TaintAsync(Guid jobId, CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfEqual(jobId, Guid.Empty, nameof(jobId));

        var uri = _options.GetApiPath($"jobs/{jobId}/taint");
        return _httpClient.PutAsync<Unit>(uri, cancellationToken);
    }

    /// <summary>
    /// Marks jobs as tainted by their batch token.
    /// </summary>
    /// <param name="token">The batch token from a previous job submission.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>A task representing the taint operation.</returns>
    /// <exception cref="ArgumentException">Thrown if token is null or whitespace.</exception>
    public Task<Result<Unit>> TaintAsync(string token, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(token);

        var uri = BuildUri("jobs/taint", token: token);
        return _httpClient.PutAsync<Unit>(uri, cancellationToken);
    }

    private string BuildUri(
        string relativePath,
        string? token = null,
        bool? force = null)
    {
        var query = new QueryStringBuilder()
            .Append("token", token)
            .AppendIf("wait", _wait)
            .AppendIf("detailed", _detailed)
            .Append("force", force);
        return query.BuildUri(_options.GetApiPath(relativePath));
    }
}
