namespace CivitaiSharp.Sdk.Services;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CivitaiSharp.Core.Response;
using CivitaiSharp.Sdk.Http;
using CivitaiSharp.Sdk.Models.Jobs;
using CivitaiSharp.Sdk.Models.Results;
using CivitaiSharp.Sdk.Request;

/// <summary>
/// Implementation of the Jobs service for managing image generation jobs.
/// </summary>
/// <remarks>
/// This service is registered as a singleton and holds references to the HTTP client and options.
/// It is created via dependency injection and should not be instantiated directly.
/// </remarks>
/// <param name="httpClient">The HTTP client for API requests.</param>
/// <param name="options">The SDK client options.</param>
/// <exception cref="ArgumentNullException">Thrown when <paramref name="httpClient"/> or <paramref name="options"/> is null.</exception>
internal sealed class JobsService(SdkHttpClient httpClient, SdkClientOptions options) : IJobsService
{
    /// <inheritdoc />
    public TextToImageJobBuilder CreateTextToImage()
        => TextToImageJobBuilder.Create(this);

    /// <inheritdoc />
    public Task<Result<JobStatusCollection>> SubmitAsync(
        TextToImageJobRequest request,
        bool wait = false,
        bool detailed = false,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        // Validate ControlNet configurations if present
        if (request.ControlNets is not null)
        {
            foreach (var controlNet in request.ControlNets)
            {
                controlNet.Validate();
            }
        }

        var uri = BuildJobsUri(wait, detailed);
        return httpClient.PostAsync<TextToImageJobRequest, JobStatusCollection>(uri, request, cancellationToken);
    }

    /// <inheritdoc />
    public Task<Result<JobStatusCollection>> SubmitBatchAsync(
        IEnumerable<TextToImageJobRequest> requests,
        bool wait = false,
        bool detailed = false,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(requests);

        // Materialize to list to validate and for the batch request
        var jobsList = requests as IReadOnlyList<TextToImageJobRequest> ?? requests.ToList();
        if (jobsList.Count == 0)
        {
            throw new ArgumentException("At least one job request is required.", nameof(requests));
        }

        // Validate all ControlNet configurations if present
        foreach (var request in jobsList)
        {
            if (request.ControlNets is not null)
            {
                foreach (var controlNet in request.ControlNets)
                {
                    controlNet.Validate();
                }
            }
        }

        var uri = BuildJobsUri(wait, detailed);
        var batchRequest = new BatchJobRequest { Jobs = jobsList };
        return httpClient.PostAsync<BatchJobRequest, JobStatusCollection>(uri, batchRequest, cancellationToken);
    }

    /// <inheritdoc />
    public Task<Result<JobStatus>> GetByIdAsync(
        Guid jobId,
        bool detailed = false,
        CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfEqual(jobId, Guid.Empty, nameof(jobId));

        var uri = BuildUri($"jobs/{jobId}", detailed: detailed);
        return httpClient.GetAsync<JobStatus>(uri, cancellationToken);
    }

    /// <inheritdoc />
    public Task<Result<JobStatusCollection>> GetByTokenAsync(
        string token,
        bool wait = false,
        bool detailed = false,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(token);

        var uri = BuildUri("jobs", token: token, wait: wait, detailed: detailed);
        return httpClient.GetAsync<JobStatusCollection>(uri, cancellationToken);
    }

    /// <inheritdoc />
    public Task<Result<JobStatusCollection>> QueryAsync(
        QueryJobsRequest request,
        bool detailed = false,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var uri = BuildUri("jobs/query", detailed: detailed);
        return httpClient.PostAsync<QueryJobsRequest, JobStatusCollection>(uri, request, cancellationToken);
    }

    /// <inheritdoc />
    public Task<Result<Unit>> CancelByIdAsync(
        Guid jobId,
        bool force = true,
        CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfEqual(jobId, Guid.Empty, nameof(jobId));

        var uri = BuildUri($"jobs/{jobId}", force: force);
        return httpClient.DeleteAsync<Unit>(uri, cancellationToken);
    }

    /// <inheritdoc />
    public Task<Result<Unit>> CancelByTokenAsync(
        string token,
        bool force = true,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(token);

        var uri = BuildUri("jobs", token: token, force: force);
        return httpClient.DeleteAsync<Unit>(uri, cancellationToken);
    }

    /// <inheritdoc />
    public Task<Result<Unit>> TaintByIdAsync(
        Guid jobId,
        CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfEqual(jobId, Guid.Empty, nameof(jobId));

        var uri = options.GetApiPath($"jobs/{jobId}/taint");
        return httpClient.PutAsync<Unit>(uri, cancellationToken);
    }

    /// <inheritdoc />
    public Task<Result<Unit>> TaintByTokenAsync(
        string token,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(token);

        var uri = BuildUri("jobs/taint", token: token);
        return httpClient.PutAsync<Unit>(uri, cancellationToken);
    }

    private string BuildJobsUri(bool wait, bool detailed)
    {
        var query = new QueryStringBuilder()
            .AppendIf("wait", wait)
            .AppendIf("detailed", detailed);
        return query.BuildUri(options.GetApiPath("jobs"));
    }

    private string BuildUri(
        string relativePath,
        string? token = null,
        bool wait = false,
        bool detailed = false,
        bool? force = null)
    {
        var query = new QueryStringBuilder()
            .Append("token", token)
            .AppendIf("wait", wait)
            .AppendIf("detailed", detailed)
            .Append("force", force);
        return query.BuildUri(options.GetApiPath(relativePath));
    }
}
