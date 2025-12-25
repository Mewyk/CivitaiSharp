namespace CivitaiSharp.Sdk.Request;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using CivitaiSharp.Core.Response;
using CivitaiSharp.Sdk.Http;
using CivitaiSharp.Sdk.Models.Jobs;
using CivitaiSharp.Sdk.Models.Results;

/// <summary>
/// Immutable, thread-safe builder for querying and managing job status.
/// Each fluent method returns a new builder instance, allowing safe reuse and caching of base configurations.
/// </summary>
public sealed record JobQueryBuilder
{
    private readonly SdkHttpClient _httpClient;
    private readonly SdkClientOptions _options;
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
    internal JobQueryBuilder(SdkHttpClient httpClient, SdkClientOptions options)
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
        SdkClientOptions options,
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
    /// Adds a custom property filter to the query.
    /// </summary>
    /// <param name="key">The property key to filter by.</param>
    /// <param name="value">The property value to match (must be JSON-serializable).</param>
    /// <returns>A new builder instance with the added property filter.</returns>
    /// <remarks>
    /// Multiple property filters are combined with AND logic - all must match.
    /// Use <see cref="JsonSerializer.SerializeToElement{T}(T, System.Text.Json.JsonSerializerOptions?)"/> to create JsonElement values.
    /// </remarks>
    public JobQueryBuilder WhereProperty(string key, JsonElement value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        var filters = _propertyFilters ?? [];
        return new(_httpClient, _options, _wait, _detailed, filters.SetItem(key, value));
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
    /// Use <see cref="WhereProperty"/> or <see cref="WhereProperties"/> to add filters before calling this method.
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
