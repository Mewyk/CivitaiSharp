namespace CivitaiSharp.Core.Request;

using System;
using System.Collections.Immutable;
using System.Globalization;
using CivitaiSharp.Core.Http;
using CivitaiSharp.Core.Models;

/// <summary>
/// Immutable, thread-safe request builder for tags. Use the fluent methods to compose filters,
/// then call <see cref="RequestBuilder{TBuilder, TEntity}.ExecuteAsync"/> to execute the request.
/// Each fluent method returns a new builder instance, allowing safe reuse and caching of base configurations.
/// </summary>
public sealed record TagBuilder : RequestBuilder<TagBuilder, Tag>
{
    /// <summary>
    /// The minimum allowed value for the <c>resultsLimit</c> parameter in <c>ExecuteAsync</c> (1).
    /// </summary>
    public const int MinimumResultsPerPage = 1;

    /// <summary>
    /// The maximum allowed value for the <c>resultsLimit</c> parameter in <c>ExecuteAsync</c> (200).
    /// </summary>
    public const int MaximumResultsPerPage = 200;

    internal TagBuilder(IPagedHttpClient httpClient)
        : base(httpClient) { }

    private TagBuilder(
        IPagedHttpClient httpClient,
        ImmutableDictionary<string, object?> filters,
        string? sort,
        int? resultsLimit)
        : base(httpClient, filters, sort, resultsLimit) { }

    /// <inheritdoc />
    protected override string Endpoint => "tags";

    /// <inheritdoc />
    /// <remarks>The tags endpoint requires a minimum limit of 1. Omit the limit to return all results (up to 200 per page).</remarks>
    protected override int MinResultsLimit => MinimumResultsPerPage;

    /// <inheritdoc />
    /// <remarks>The tags endpoint has a maximum limit of 200.</remarks>
    protected override int MaxResultsLimit => MaximumResultsPerPage;

    /// <inheritdoc />
    /// <remarks>The tags endpoint returns 20 results by default.</remarks>
    protected override int DefaultResultsLimit => 20;

    /// <inheritdoc />
    protected override TagBuilder With(
        ImmutableDictionary<string, object?> filters,
        string? sort,
        int? resultsLimit) =>
        new(HttpClient, filters, sort, resultsLimit);

    private static class FilterKeys
    {
        public const string Query = "query";
        public const string Page = "page";
    }

    /// <summary>
    /// Filter tags by name (partial match via search query parameter).
    /// </summary>
    /// <param name="name">The tag name to search for.</param>
    /// <returns>A new builder instance with the filter applied.</returns>
    /// <exception cref="ArgumentException">Thrown if name is null or whitespace.</exception>
    public TagBuilder WhereName(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        return WithFilter(FilterKeys.Query, name);
    }

    /// <summary>
    /// Set the page index for page-based pagination.
    /// </summary>
    /// <param name="pageIndex">The 1-based page index. Must be at least 1.</param>
    /// <returns>A new builder instance with the page index applied.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if pageIndex is less than 1.</exception>
    public TagBuilder WithPageIndex(int pageIndex)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(pageIndex, 1);
        return WithFilter(FilterKeys.Page, pageIndex.ToString(CultureInfo.InvariantCulture));
    }
}
