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
    /// Provides access to image generation jobs: submit, query, cancel, and retrieve results.
    /// </summary>
    IJobsService Jobs { get; }

    /// <summary>
    /// Provides model and resource availability checks to verify assets are ready before submitting jobs.
    /// </summary>
    ICoverageService Coverage { get; }

    /// <summary>
    /// Provides API consumption tracking: credit usage, job counts, and consumption history.
    /// </summary>
    IUsageService Usage { get; }
}
