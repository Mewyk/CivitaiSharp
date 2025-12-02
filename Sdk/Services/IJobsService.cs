namespace CivitaiSharp.Sdk.Services;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CivitaiSharp.Core.Response;
using CivitaiSharp.Sdk.Models.Jobs;
using CivitaiSharp.Sdk.Models.Results;

/// <summary>
/// Service for submitting, tracking, and managing image generation jobs.
/// </summary>
public interface IJobsService
{
    /// <summary>
    /// Submits a single text-to-image generation job.
    /// </summary>
    /// <param name="request">The job request.</param>
    /// <param name="wait">If true, blocks until the job completes (up to ~10 minutes).</param>
    /// <param name="detailed">If true, includes the original job specification in the response.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>A task containing the job status collection with a token for polling.</returns>
    Task<Result<JobStatusCollection>> SubmitAsync(
        TextToImageJobRequest request,
        bool wait = false,
        bool detailed = false,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Submits multiple text-to-image generation jobs as a batch.
    /// </summary>
    /// <param name="requests">The job requests to submit.</param>
    /// <param name="wait">If true, blocks until all jobs complete (up to ~10 minutes).</param>
    /// <param name="detailed">If true, includes the original job specifications in the response.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>A task containing the job status collection with a token for polling.</returns>
    Task<Result<JobStatusCollection>> SubmitBatchAsync(
        IEnumerable<TextToImageJobRequest> requests,
        bool wait = false,
        bool detailed = false,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the status of a specific job by its ID.
    /// </summary>
    /// <param name="jobId">The unique job identifier.</param>
    /// <param name="detailed">If true, includes the original job specification in the response.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>A task containing the job status.</returns>
    Task<Result<JobStatus>> GetByIdAsync(
        Guid jobId,
        bool detailed = false,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the status of jobs by their batch token.
    /// </summary>
    /// <param name="token">The batch token from a previous job submission.</param>
    /// <param name="wait">If true, blocks until all jobs complete (up to ~10 minutes).</param>
    /// <param name="detailed">If true, includes the original job specifications in the response.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>A task containing the job status collection.</returns>
    Task<Result<JobStatusCollection>> GetByTokenAsync(
        string token,
        bool wait = false,
        bool detailed = false,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Queries jobs by custom properties.
    /// </summary>
    /// <param name="request">The query request containing property filters.</param>
    /// <param name="detailed">If true, includes the original job specifications in the response.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>A task containing the matching jobs.</returns>
    Task<Result<JobStatusCollection>> QueryAsync(
        QueryJobsRequest request,
        bool detailed = false,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Cancels a specific job by its ID.
    /// </summary>
    /// <param name="jobId">The unique job identifier.</param>
    /// <param name="force">If true, cancels even if the job is processing. Default is true.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>A task representing the cancellation operation.</returns>
    Task<Result<Unit>> CancelByIdAsync(
        Guid jobId,
        bool force = true,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Cancels jobs by their batch token.
    /// </summary>
    /// <param name="token">The batch token from a previous job submission.</param>
    /// <param name="force">If true, cancels even if jobs are processing. Default is true.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>A task representing the cancellation operation.</returns>
    Task<Result<Unit>> CancelByTokenAsync(
        string token,
        bool force = true,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Marks a specific job as tainted by its ID.
    /// </summary>
    /// <param name="jobId">The unique job identifier.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>A task representing the taint operation.</returns>
    Task<Result<Unit>> TaintByIdAsync(
        Guid jobId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Marks jobs as tainted by their batch token.
    /// </summary>
    /// <param name="token">The batch token from a previous job submission.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>A task representing the taint operation.</returns>
    Task<Result<Unit>> TaintByTokenAsync(
        string token,
        CancellationToken cancellationToken = default);
}
