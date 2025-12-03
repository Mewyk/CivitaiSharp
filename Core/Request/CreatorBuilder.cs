namespace CivitaiSharp.Core.Request;

using System;
using System.Collections.Immutable;
using System.Globalization;
using CivitaiSharp.Core.Http;
using CivitaiSharp.Core.Models;

/// <summary>
/// Immutable, thread-safe request builder for creators. This endpoint uses page-based pagination
/// instead of cursor-based; use <see cref="WithPageIndex"/> to navigate between pages.
/// Each fluent method returns a new builder instance, allowing safe reuse and caching of base configurations.
/// </summary>
/// <remarks>
/// <para>
/// <strong>Known Issue:</strong> The <c>/api/v1/creators</c> endpoint has shown reliability issues including
/// request timeouts (10+ seconds with no response) and occasional 500 Internal Server Error responses.
/// Consider implementing extended timeout handling or retry logic when using this endpoint.
/// </para>
/// <para>
/// The resilience policies configured via <see cref="Microsoft.Extensions.Http.Resilience"/> will automatically
/// handle transient failures, but persistent timeouts may still occur.
/// </para>
/// </remarks>
public sealed record CreatorBuilder : RequestBuilder<CreatorBuilder, Creator>
{
    /// <summary>
    /// The minimum allowed value for the <c>resultsLimit</c> parameter in <c>ExecuteAsync</c> (1).
    /// </summary>
    public const int MinimumResultsPerPage = 1;

    /// <summary>
    /// The maximum allowed value for the <c>resultsLimit</c> parameter in <c>ExecuteAsync</c> (200).
    /// </summary>
    public const int MaximumResultsPerPage = 200;

    internal CreatorBuilder(IPagedHttpClient httpClient)
        : base(httpClient) { }

    private CreatorBuilder(
        IPagedHttpClient httpClient,
        ImmutableDictionary<string, object?> filters,
        string? sort,
        int? resultsLimit)
        : base(httpClient, filters, sort, resultsLimit) { }

    /// <inheritdoc />
    protected override string Endpoint => "creators";

    /// <inheritdoc />
    /// <remarks>The creators endpoint requires a minimum limit of 1. Omit the limit to return all results (up to 200 per page).</remarks>
    protected override int MinResultsLimit => MinimumResultsPerPage;

    /// <inheritdoc />
    /// <remarks>The creators endpoint has a maximum limit of 200.</remarks>
    protected override int MaxResultsLimit => MaximumResultsPerPage;

    /// <inheritdoc />
    /// <remarks>The creators endpoint returns 20 results by default.</remarks>
    protected override int DefaultResultsLimit => 20;

    /// <inheritdoc />
    protected override CreatorBuilder With(
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
    /// Filter creators by username (partial match via search query parameter).
    /// </summary>
    /// <param name="name">The username to search for.</param>
    /// <returns>A new builder instance with the filter applied.</returns>
    /// <exception cref="ArgumentException">Thrown if name is null or whitespace.</exception>
    public CreatorBuilder WhereName(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        return WithFilter(FilterKeys.Query, name);
    }

    /// <summary>
    /// Set the page index for page-based pagination.
    /// The creators endpoint uses page-based pagination instead of cursor-based.
    /// </summary>
    /// <param name="pageIndex">The 1-based page index. Must be at least 1.</param>
    /// <returns>A new builder instance with the page index applied.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if pageIndex is less than 1.</exception>
    public CreatorBuilder WithPageIndex(int pageIndex)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(pageIndex, 1);
        return WithFilter(FilterKeys.Page, pageIndex.ToString(CultureInfo.InvariantCulture));
    }
}
