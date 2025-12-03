namespace CivitaiSharp.Core.Request;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CivitaiSharp.Core.Http;
using CivitaiSharp.Core.Response;

/// <summary>
/// Immutable base record for API request builders. Encapsulates common paging, sorting, and filter logic
/// to support a consistent fluent API across all resource types. Instances are thread-safe and can be
/// cached and reused; each fluent method returns a new builder instance with the updated state.
/// </summary>
/// <typeparam name="TBuilder">The derived builder type (for fluent chaining).</typeparam>
/// <typeparam name="TEntity">The entity type returned by requests.</typeparam>
public abstract record RequestBuilder<TBuilder, TEntity>
    where TBuilder : RequestBuilder<TBuilder, TEntity>
{
    private const string LimitParameterName = "limit";
    private const string CursorParameterName = "cursor";
    private const string SortParameterName = "sort";

    private readonly IPagedHttpClient _httpClient;
    private readonly ImmutableDictionary<string, object?> _filters;
    private readonly string? _sort;
    private readonly int? _resultsLimit;

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestBuilder{TBuilder, TEntity}"/> record.
    /// </summary>
    /// <param name="httpClient">The HTTP client used to execute requests.</param>
    /// <exception cref="ArgumentNullException">Thrown when httpClient is null.</exception>
    private protected RequestBuilder(IPagedHttpClient httpClient)
        : this(
            httpClient ?? throw new ArgumentNullException(nameof(httpClient)),
            ImmutableDictionary<string, object?>.Empty.WithComparers(StringComparer.OrdinalIgnoreCase),
            sort: null,
            resultsLimit: null)
    {
    }

    /// <summary>
    /// Internal constructor for creating new instances with modified state.
    /// </summary>
    private protected RequestBuilder(
        IPagedHttpClient httpClient,
        ImmutableDictionary<string, object?> filters,
        string? sort,
        int? resultsLimit)
    {
        _httpClient = httpClient;
        _filters = filters;
        _sort = sort;
        _resultsLimit = resultsLimit;
    }

    /// <summary>
    /// Gets the API endpoint path segment for this resource type (e.g., "models", "images").
    /// </summary>
    protected abstract string Endpoint { get; }

    /// <summary>
    /// Gets the minimum allowed value for results limit for this endpoint.
    /// </summary>
    protected abstract int MinResultsLimit { get; }

    /// <summary>
    /// Gets the maximum allowed value for results limit for this endpoint.
    /// </summary>
    protected abstract int MaxResultsLimit { get; }

    /// <summary>
    /// Gets the default results limit when none is specified for this endpoint.
    /// </summary>
    protected abstract int DefaultResultsLimit { get; }

    /// <summary>
    /// Gets a value indicating whether this endpoint supports sorting.
    /// Override to return true in derived classes that support sorting.
    /// </summary>
    protected virtual bool SupportsSorting => false;

    /// <summary>
    /// Gets the immutable dictionary of request filter parameters with typed values.
    /// </summary>
    protected ImmutableDictionary<string, object?> Filters => _filters;

    /// <summary>
    /// Gets the sort option value.
    /// </summary>
    protected string? Sort => _sort;

    /// <summary>
    /// Gets the number of results to return per page.
    /// </summary>
    protected int? ResultsLimit => _resultsLimit;

    /// <summary>
    /// Creates a new builder instance with the specified state changes.
    /// Derived classes must implement this to return the correct derived type.
    /// </summary>
    /// <param name="filters">The new filters dictionary with typed values.</param>
    /// <param name="sort">The new sort value.</param>
    /// <param name="resultsLimit">The new results limit.</param>
    /// <returns>A new builder instance with the updated state.</returns>
    protected abstract TBuilder With(
        ImmutableDictionary<string, object?> filters,
        string? sort,
        int? resultsLimit);

    /// <summary>
    /// Gets the HTTP client for derived classes to use.
    /// </summary>
    private protected IPagedHttpClient HttpClient => _httpClient;

    /// <summary>
    /// Builds a query string with all filters, pagination, and sorting parameters.
    /// Arrays are expanded to repeated parameters (e.g., <c>ids=1&amp;ids=2</c>).
    /// Enums are converted using <see cref="Extensions.ApiStringRegistry"/> and URL-encoded.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The Civitai API requires all parameters to be passed as query string parameters for GET requests.
    /// JSON request bodies are completely ignored.
    /// </para>
    /// <para>
    /// Array parameters are formatted as repeated query parameters (<c>key=value1&amp;key=value2</c>), which is
    /// the safest approach. While some parameters like <c>ids</c> also accept comma-separated values, others
    /// like <c>baseModels</c> return a 400 error with comma separation.
    /// </para>
    /// </remarks>
    private string BuildQueryString(int? resultsLimit = null, string? cursor = null)
    {
        var parts = new List<string>();

        if (resultsLimit.HasValue)
        {
            parts.Add($"{LimitParameterName}={resultsLimit.Value.ToString(CultureInfo.InvariantCulture)}");
        }

        if (!string.IsNullOrWhiteSpace(cursor))
        {
            parts.Add($"{CursorParameterName}={Uri.EscapeDataString(cursor)}");
        }

        if (SupportsSorting && !string.IsNullOrWhiteSpace(_sort))
        {
            parts.Add($"{SortParameterName}={Uri.EscapeDataString(_sort)}");
        }

        foreach (var (key, value) in _filters)
        {
            if (value is null)
            {
                continue;
            }

            parts.AddRange(FormatFilterValue(key, value));
        }

        return parts.Count > 0 ? "?" + string.Join("&", parts) : string.Empty;
    }

    /// <summary>
    /// Formats a filter value for inclusion in a query string.
    /// Handles arrays (repeated params), enums (API string conversion), and primitives.
    /// </summary>
    private static IEnumerable<string> FormatFilterValue(string key, object value)
    {
        var encodedKey = Uri.EscapeDataString(key);

        // Handle arrays by expanding to repeated parameters
        if (value is System.Collections.IEnumerable enumerable and not string)
        {
            foreach (var item in enumerable)
            {
                if (item is not null)
                {
                    yield return $"{encodedKey}={Uri.EscapeDataString(FormatSingleValue(item))}";
                }
            }
        }
        else
        {
            yield return $"{encodedKey}={Uri.EscapeDataString(FormatSingleValue(value))}";
        }
    }

    /// <summary>
    /// Formats a single value to its string representation for query strings.
    /// </summary>
    private static string FormatSingleValue(object value) => value switch
    {
        bool b => b ? "true" : "false",
        Enum e => Extensions.ApiStringRegistry.GetApiString(e),
        _ => value.ToString() ?? string.Empty
    };



    /// <summary>
    /// Adds or updates a filter parameter with a typed value.
    /// The value type is preserved (booleans stay bool, arrays stay arrays, etc.) for consistent query string formatting.
    /// </summary>
    /// <param name="key">The filter key.</param>
    /// <param name="value">The filter value. Can be a primitive, enum, or array. Null values are ignored during query string building.</param>
    /// <returns>A new builder instance with the updated filter.</returns>
    protected TBuilder WithFilter(string key, object? value)
    {
        return With(_filters.SetItem(key, value), _sort, _resultsLimit);
    }

    /// <summary>
    /// Sets the sort option.
    /// </summary>
    /// <param name="sort">The sort option value.</param>
    /// <returns>A new builder instance with the updated sort.</returns>
    protected TBuilder WithSort(string? sort)
    {
        return With(_filters, sort, _resultsLimit);
    }

    /// <summary>
    /// Executes the request and returns paged results with pagination metadata.
    /// </summary>
    /// <param name="resultsLimit">Optional number of results per page. Must be between <see cref="MinResultsLimit"/> and <see cref="MaxResultsLimit"/>. Omit to use the API default.</param>
    /// <param name="cursor">Optional cursor for cursor-based pagination.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous operation. The result contains either the paged items or an error.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if resultsLimit is specified and is less than <see cref="MinResultsLimit"/> or greater than <see cref="MaxResultsLimit"/>.</exception>
    public Task<Result<PagedResult<TEntity>>> ExecuteAsync(
        int? resultsLimit = null,
        string? cursor = null,
        CancellationToken cancellationToken = default)
    {
        if (resultsLimit.HasValue)
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(resultsLimit.Value, MinResultsLimit);
            ArgumentOutOfRangeException.ThrowIfGreaterThan(resultsLimit.Value, MaxResultsLimit);
        }

        var endpoint = _httpClient.Options.GetApiPath(Endpoint);
        var queryString = BuildQueryString(resultsLimit ?? ResultsLimit, cursor);
        return _httpClient.GetPageAsync<TEntity>(endpoint, queryString, cancellationToken);
    }

    /// <summary>
    /// Executes the request and returns only the first result, or null if no results are found.
    /// This is a convenience method that internally limits the request to 1 result for efficiency.
    /// </summary>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The result contains either:
    /// <list type="bullet">
    /// <item>The first entity if results exist</item>
    /// <item>Null if no results match the query</item>
    /// <item>An error if the request failed</item>
    /// </list>
    /// </returns>
    public async Task<Result<TEntity?>> FirstOrDefaultAsync(CancellationToken cancellationToken = default)
    {
        var result = await ExecuteAsync(resultsLimit: 1, cancellationToken: cancellationToken).ConfigureAwait(false);

        return result.Select(pagedResult => pagedResult.Items.FirstOrDefault());
    }
}
