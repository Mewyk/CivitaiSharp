namespace CivitaiSharp.Sdk;

using CivitaiSharp.Sdk.Services;

/// <summary>
/// Client interface for the Civitai Generator API (orchestration endpoints) for image generation,
/// model availability checking, and usage tracking. For the public API (models, images, tags, creators),
/// use <see cref="CivitaiSharp.Core.IApiClient"/> instead.
/// </summary>
public interface ISdkClient
{
    /// <summary>
    /// Image generation jobs: submit, query, cancel, and retrieve results.
    /// </summary>
    IJobsService Jobs { get; }

    /// <summary>
    /// Model and resource availability checks before submitting jobs.
    /// </summary>
    ICoverageService Coverage { get; }

    /// <summary>
    /// API consumption tracking: credits, job counts, and history.
    /// </summary>
    IUsageService Usage { get; }
}
