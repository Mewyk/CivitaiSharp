namespace CivitaiSharp.Core.Request;

using System;
using System.Collections.Immutable;
using System.Globalization;
using CivitaiSharp.Core.Extensions;
using CivitaiSharp.Core.Http;
using CivitaiSharp.Core.Models;
using CivitaiSharp.Core.Validation;

/// <summary>
/// Immutable, thread-safe request builder for images. Use the fluent methods to compose filters,
/// then call <see cref="RequestBuilder{TBuilder, TEntity}.ExecuteAsync"/> to execute the request.
/// Each fluent method returns a new builder instance, allowing safe reuse and caching of base configurations.
/// </summary>
public sealed record ImageBuilder : RequestBuilder<ImageBuilder, Image>
{
    /// <summary>
    /// The minimum allowed value for <see cref="RequestBuilder{TBuilder, TEntity}.WithResultsLimit"/> (1).
    /// </summary>
    public const int MinimumResultsPerPage = 1;

    /// <summary>
    /// The maximum allowed value for <see cref="RequestBuilder{TBuilder, TEntity}.WithResultsLimit"/> (200).
    /// </summary>
    public const int MaximumResultsPerPage = 200;

    internal ImageBuilder(IPagedHttpClient httpClient)
        : base(httpClient) { }

    private ImageBuilder(
        IPagedHttpClient httpClient,
        ImmutableDictionary<string, object?> filters,
        string? sort,
        int? resultsLimit)
        : base(httpClient, filters, sort, resultsLimit) { }

    /// <inheritdoc />
    protected override string Endpoint => "images";

    /// <inheritdoc />
    /// <remarks>The images endpoint requires a minimum limit of 1. Omit the limit to return all results (up to 200 per page).</remarks>
    protected override int MinResultsLimit => MinimumResultsPerPage;

    /// <inheritdoc />
    /// <remarks>The images endpoint has a maximum limit of 200.</remarks>
    protected override int MaxResultsLimit => MaximumResultsPerPage;

    /// <inheritdoc />
    /// <remarks>The images endpoint returns 100 results by default.</remarks>
    protected override int DefaultResultsLimit => 100;

    /// <inheritdoc />
    protected override bool SupportsSorting => true;

    /// <inheritdoc />
    protected override ImageBuilder With(
        ImmutableDictionary<string, object?> filters,
        string? sort,
        int? resultsLimit) =>
        new(HttpClient, filters, sort, resultsLimit);

    private static class FilterKeys
    {
        public const string ModelId = "modelId";
        public const string ModelVersionId = "modelVersionId";
        public const string PostId = "postId";
        public const string Username = "username";
        public const string Nsfw = "nsfw";
        public const string Period = "period";
        public const string Page = "page";
    }

    /// <summary>
    /// Filter images by model id.
    /// </summary>
    /// <param name="id">The model identifier. Must be a positive value.</param>
    /// <returns>A new builder instance with the filter applied.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if id is less than 1.</exception>
    public ImageBuilder WhereModelId(long id)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(id, 1);
        return WithFilter(FilterKeys.ModelId, id.ToString(CultureInfo.InvariantCulture));
    }

    /// <summary>
    /// Filter images by model version id.
    /// </summary>
    /// <param name="id">The model version identifier. Must be a positive value.</param>
    /// <returns>A new builder instance with the filter applied.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if id is less than 1.</exception>
    public ImageBuilder WhereModelVersionId(long id)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(id, 1);
        return WithFilter(FilterKeys.ModelVersionId, id.ToString(CultureInfo.InvariantCulture));
    }

    /// <summary>
    /// Filter images by post id.
    /// </summary>
    /// <param name="id">The post identifier. Must be a positive value.</param>
    /// <returns>A new builder instance with the filter applied.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if id is less than 1.</exception>
    public ImageBuilder WherePostId(long id)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(id, 1);
        return WithFilter(FilterKeys.PostId, id.ToString(CultureInfo.InvariantCulture));
    }

    /// <summary>
    /// Filter images by the creator's username.
    /// </summary>
    /// <param name="username">The username to filter by. Must contain only letters, numbers, and underscores.</param>
    /// <returns>A new builder instance with the filter applied.</returns>
    /// <exception cref="ArgumentException">Thrown if username is null, whitespace, or contains invalid characters.</exception>
    public ImageBuilder WhereUsername(string username)
    {
        CivitaiValidation.ThrowIfInvalidUsername(username, nameof(username));
        return WithFilter(FilterKeys.Username, username);
    }

    /// <summary>
    /// Filter images by NSFW level.
    /// </summary>
    /// <param name="level">The NSFW level to filter by.</param>
    /// <returns>A new builder instance with the filter applied.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if level is not a defined enum value.</exception>
    public ImageBuilder WhereNsfwLevel(ImageNsfwLevel level)
    {
        level.ThrowIfUndefined();
        return WithFilter(FilterKeys.Nsfw, level.ToApiString());
    }

    /// <summary>
    /// Order images by the given sort option.
    /// </summary>
    /// <param name="sort">The sort option to apply.</param>
    /// <returns>A new builder instance with the sort applied.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if sort is not a defined enum value.</exception>
    public ImageBuilder OrderBy(ImageSort sort)
    {
        sort.ThrowIfUndefined();
        return WithSort(sort.ToApiString());
    }

    /// <summary>
    /// Restrict results to a specific time period.
    /// </summary>
    /// <param name="period">The time period to filter by.</param>
    /// <returns>A new builder instance with the filter applied.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if period is not a defined enum value.</exception>
    public ImageBuilder WherePeriod(TimePeriod period)
    {
        period.ThrowIfUndefined();
        return WithFilter(FilterKeys.Period, period.ToApiString());
    }

    /// <summary>
    /// Set the page index for page-based pagination.
    /// </summary>
    /// <param name="pageIndex">The 1-based page index. Must be at least 1.</param>
    /// <returns>A new builder instance with the page index applied.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if pageIndex is less than 1.</exception>
    public ImageBuilder WithPageIndex(int pageIndex)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(pageIndex, 1);
        return WithFilter(FilterKeys.Page, pageIndex.ToString(CultureInfo.InvariantCulture));
    }
}
