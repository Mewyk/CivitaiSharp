namespace CivitaiSharp.Core.Request;

using System;
using System.Collections.Immutable;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using CivitaiSharp.Core.Extensions;
using CivitaiSharp.Core.Http;
using CivitaiSharp.Core.Models;
using CivitaiSharp.Core.Response;
using CivitaiSharp.Core.Validation;

/// <summary>
/// Immutable, thread-safe request builder for models. Use the fluent methods to compose filters,
/// then call <see cref="RequestBuilder{TBuilder, TEntity}.ExecuteAsync"/> to execute the request.
/// Each fluent method returns a new builder instance, allowing safe reuse and caching of base configurations.
/// </summary>
public sealed record ModelBuilder : RequestBuilder<ModelBuilder, Model>
{
    /// <summary>
    /// The minimum allowed value for <see cref="RequestBuilder{TBuilder, TEntity}.WithResultsLimit"/> (1).
    /// </summary>
    public const int MinimumResultsPerPage = 1;

    /// <summary>
    /// The maximum allowed value for <see cref="RequestBuilder{TBuilder, TEntity}.WithResultsLimit"/> (100).
    /// </summary>
    public const int MaximumResultsPerPage = 100;

    internal ModelBuilder(IPagedHttpClient httpClient)
        : base(httpClient) { }

    private ModelBuilder(
        IPagedHttpClient httpClient,
        ImmutableDictionary<string, object?> filters,
        string? sort,
        int? resultsLimit)
        : base(httpClient, filters, sort, resultsLimit) { }

    /// <inheritdoc />
    protected override string Endpoint => "models";

    /// <inheritdoc />
    /// <remarks>The models endpoint requires a minimum limit of 1.</remarks>
    protected override int MinResultsLimit => MinimumResultsPerPage;

    /// <inheritdoc />
    /// <remarks>The models endpoint has a maximum limit of 100.</remarks>
    protected override int MaxResultsLimit => MaximumResultsPerPage;

    /// <inheritdoc />
    /// <remarks>The models endpoint returns 100 results by default.</remarks>
    protected override int DefaultResultsLimit => 100;

    /// <inheritdoc />
    protected override bool SupportsSorting => true;

    /// <inheritdoc />
    protected override ModelBuilder With(
        ImmutableDictionary<string, object?> filters,
        string? sort,
        int? resultsLimit) =>
        new(HttpClient, filters, sort, resultsLimit);

    /// <summary>
    /// Gets a model by its unique identifier.
    /// </summary>
    /// <param name="id">The model identifier. Must be a positive value.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous operation. The result contains either the model or an error.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if id is less than 1.</exception>
    public Task<Result<Model>> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(id, 1);
        var uri = HttpClient.Options.GetApiPath($"models/{id.ToString(CultureInfo.InvariantCulture)}");
        return HttpClient.GetAsync<Model>(uri, cancellationToken);
    }

    private static class FilterKeys
    {
        public const string Query = "query";
        public const string Tag = "tag";
        public const string Username = "username";
        public const string Types = "types";
        public const string Ids = "ids";
        public const string BaseModels = "baseModels";
        public const string Favorites = "favorites";
        public const string Hidden = "hidden";
        public const string PrimaryFileOnly = "primaryFileOnly";
        public const string AllowNoCredit = "allowNoCredit";
        public const string AllowDerivatives = "allowDerivatives";
        public const string AllowDifferentLicenses = "allowDifferentLicenses";
        public const string AllowCommercialUse = "allowCommercialUse";
        public const string Nsfw = "nsfw";
        public const string SupportsGeneration = "supportsGeneration";
        public const string Period = "period";
        public const string Page = "page";
    }

    /// <summary>
    /// Filter models by name (search query parameter).
    /// </summary>
    /// <param name="name">The model name to search for.</param>
    /// <returns>A new builder instance with the filter applied.</returns>
    /// <exception cref="ArgumentException">Thrown if name is null or whitespace.</exception>
    public ModelBuilder WhereName(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        return WithFilter(FilterKeys.Query, name);
    }

    /// <summary>
    /// Filter models by a single tag.
    /// </summary>
    /// <param name="tag">The tag to filter by.</param>
    /// <returns>A new builder instance with the filter applied.</returns>
    /// <exception cref="ArgumentException">Thrown if tag is null or whitespace.</exception>
    public ModelBuilder WhereTag(string tag)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(tag);
        return WithFilter(FilterKeys.Tag, tag);
    }

    /// <summary>
    /// Filter models by creator username.
    /// </summary>
    /// <param name="username">The username to filter by. Must contain only letters, numbers, and underscores.</param>
    /// <returns>A new builder instance with the filter applied.</returns>
    /// <exception cref="ArgumentException">Thrown if username is null, whitespace, or contains invalid characters.</exception>
    public ModelBuilder WhereUsername(string username)
    {
        CivitaiValidation.ThrowIfInvalidUsername(username, nameof(username));
        return WithFilter(FilterKeys.Username, username);
    }

    /// <summary>
    /// Filter models by model type.
    /// </summary>
    /// <param name="type">The model type to filter by.</param>
    /// <returns>A new builder instance with the filter applied.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if type is not a defined enum value.</exception>
    public ModelBuilder WhereType(ModelType type)
    {
        type.ThrowIfUndefined();
        return WithFilter(FilterKeys.Types, type.ToApiString());
    }

    /// <summary>
    /// Restrict results to favorites for the authenticated user.
    /// </summary>
    /// <returns>A new builder instance with the filter applied.</returns>
    /// <remarks>
    /// <strong>Requires authentication.</strong> This filter requires a valid API key to be configured via
    /// <see cref="ApiClientOptions.ApiKey"/>. Without authentication, this filter will return 0 results.
    /// </remarks>
    public ModelBuilder WhereFavorites() =>
        WithFilter(FilterKeys.Favorites, true);

    /// <summary>
    /// Restrict results to hidden models.
    /// </summary>
    /// <returns>A new builder instance with the filter applied.</returns>
    /// <remarks>
    /// <strong>Requires authentication.</strong> This filter requires a valid API key to be configured via
    /// <see cref="ApiClientOptions.ApiKey"/>. Without authentication, this filter will return 0 results.
    /// Hidden models are models that the authenticated user has explicitly hidden from their feed.
    /// </remarks>
    public ModelBuilder WhereHidden() =>
        WithFilter(FilterKeys.Hidden, true);

    /// <summary>
    /// Only include primary file models.
    /// </summary>
    /// <returns>A new builder instance with the filter applied.</returns>
    public ModelBuilder WherePrimaryFileOnly() =>
        WithFilter(FilterKeys.PrimaryFileOnly, true);

    /// <summary>
    /// Filter by allowNoCredit permission.
    /// </summary>
    /// <param name="allowed">True to filter for models that allow use without credit, false otherwise.</param>
    /// <returns>A new builder instance with the filter applied.</returns>
    /// <remarks>
    /// <strong>Note:</strong> Testing shows this filter may return 0 results regardless of the value. This could be
    /// an API-side behavior, a data condition requirement, or require additional context. Use with caution and verify
    /// results match expectations.
    /// </remarks>
    public ModelBuilder WhereAllowNoCredit(bool allowed) =>
        WithFilter(FilterKeys.AllowNoCredit, allowed);

    /// <summary>
    /// Filter by allowDerivatives permission.
    /// </summary>
    /// <param name="allowed">True to filter for models that allow derivatives, false otherwise.</param>
    /// <returns>A new builder instance with the filter applied.</returns>
    /// <remarks>
    /// <strong>Note:</strong> Testing shows this filter may return 0 results regardless of the value. This could be
    /// an API-side behavior, a data condition requirement, or require additional context. Use with caution and verify
    /// results match expectations.
    /// </remarks>
    public ModelBuilder WhereAllowDerivatives(bool allowed) =>
        WithFilter(FilterKeys.AllowDerivatives, allowed);

    /// <summary>
    /// Filter by allowDifferentLicenses permission.
    /// </summary>
    /// <param name="allowed">True to filter for models that allow different licenses on derivatives, false otherwise.</param>
    /// <returns>A new builder instance with the filter applied.</returns>
    public ModelBuilder WhereAllowDifferentLicenses(bool allowed) =>
        WithFilter(FilterKeys.AllowDifferentLicenses, allowed);

    /// <summary>
    /// Filter by commercial use permissions. The API accepts multiple permission levels and returns models matching any of them (OR logic).
    /// </summary>
    /// <param name="permissions">The commercial use permission levels to filter by. Must contain at least one value.</param>
    /// <returns>A new builder instance with the filter applied.</returns>
    /// <exception cref="ArgumentException">Thrown if permissions is null or empty.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if any permission is not a defined enum value.</exception>
    /// <remarks>
    /// <para>
    /// Testing shows that the API requires at least two permission values to return results. Single values
    /// (e.g., only <see cref="CommercialUsePermission.None"/>) typically return 0 results. This behavior may be
    /// API-specific or reflect the underlying data model.
    /// </para>
    /// <para>
    /// Multiple values are sent as repeated query parameters (e.g., <c>allowCommercialUse=Image&amp;allowCommercialUse=Sell</c>).
    /// The API interprets this as an OR filter, returning models that match any of the specified permissions.
    /// </para>
    /// </remarks>
    public ModelBuilder WhereCommercialUse(params CommercialUsePermission[] permissions)
    {
        ArgumentNullException.ThrowIfNull(permissions);
        if (permissions.Length == 0)
        {
            throw new ArgumentException("At least one permission must be provided.", nameof(permissions));
        }

        foreach (var permission in permissions)
        {
            permission.ThrowIfUndefined();
        }

        var permissionStrings = permissions.Select(p => p.ToApiString()).ToArray();
        return WithFilter(FilterKeys.AllowCommercialUse, permissionStrings);
    }

    /// <summary>
    /// Filter NSFW models.
    /// </summary>
    /// <param name="nsfw">True to include only NSFW models, false to exclude them.</param>
    /// <returns>A new builder instance with the filter applied.</returns>
    public ModelBuilder WhereNsfw(bool nsfw) =>
        WithFilter(FilterKeys.Nsfw, nsfw);

    /// <summary>
    /// Filter models that support generation.
    /// </summary>
    /// <param name="supported">True to include only models that support generation, false otherwise.</param>
    /// <returns>A new builder instance with the filter applied.</returns>
    public ModelBuilder WhereSupportsGeneration(bool supported) =>
        WithFilter(FilterKeys.SupportsGeneration, supported);

    /// <summary>
    /// Order results by a model sort option.
    /// </summary>
    /// <param name="sort">The sort option to apply.</param>
    /// <returns>A new builder instance with the sort applied.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if sort is not a defined enum value.</exception>
    /// <remarks>
    /// Sort values containing spaces (e.g., "Highest Rated", "Most Downloaded") are automatically URL-encoded.
    /// The API accepts both URL-encoded and raw space characters, but encoding is recommended for reliability.
    /// </remarks>
    public ModelBuilder OrderBy(ModelSort sort)
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
    public ModelBuilder WherePeriod(TimePeriod period)
    {
        period.ThrowIfUndefined();
        return WithFilter(FilterKeys.Period, period.ToApiString());
    }

    /// <summary>
    /// Filter models by specific model IDs. This filter is ignored if a query is also provided.
    /// </summary>
    /// <param name="ids">The model IDs to filter by. Must contain at least one value, and all values must be positive.</param>
    /// <returns>A new builder instance with the filter applied.</returns>
    /// <exception cref="ArgumentException">Thrown if ids is null or empty.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if any id is less than 1.</exception>
    /// <remarks>
    /// Multiple IDs are sent as repeated query parameters (e.g., <c>ids=122359&amp;ids=58390</c>) or can be
    /// comma-separated (e.g., <c>ids=122359,58390</c>). Both formats work correctly for the <c>ids</c> parameter.
    /// </remarks>
    public ModelBuilder WhereIds(params long[] ids)
    {
        ArgumentNullException.ThrowIfNull(ids);
        if (ids.Length == 0)
        {
            throw new ArgumentException("At least one ID must be provided.", nameof(ids));
        }

        foreach (var id in ids)
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(id, 1);
        }

        return WithFilter(FilterKeys.Ids, ids);
    }

    /// <summary>
    /// Filter models by base model type (e.g., "SD 1.5", "SDXL 1.0", "Flux.1 D").
    /// </summary>
    /// <param name="baseModel">The base model type to filter by.</param>
    /// <returns>A new builder instance with the filter applied.</returns>
    /// <exception cref="ArgumentException">Thrown if baseModel is null or whitespace.</exception>
    public ModelBuilder WhereBaseModel(string baseModel)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(baseModel);
        return WithFilter(FilterKeys.BaseModels, baseModel);
    }

    /// <summary>
    /// Filter models by multiple base model types (e.g., "SD 1.5", "SDXL 1.0", "Flux.1 D").
    /// </summary>
    /// <param name="baseModels">The base model types to filter by. Must contain at least one value.</param>
    /// <returns>A new builder instance with the filter applied.</returns>
    /// <exception cref="ArgumentException">Thrown if baseModels is null, empty, or contains null/whitespace values.</exception>
    /// <remarks>
    /// <para>
    /// Multiple base models are sent as repeated query parameters (e.g., <c>baseModels=SD%201.5&amp;baseModels=SDXL%201.0</c>).
    /// Values containing spaces are automatically URL-encoded.
    /// </para>
    /// <para>
    /// <strong>Important:</strong> Unlike the <c>ids</c> parameter, comma-separated values for <c>baseModels</c> will result
    /// in a 400 Bad Request error. Only the repeated parameter format is supported for base models.
    /// </para>
    /// </remarks>
    public ModelBuilder WhereBaseModels(params string[] baseModels)
    {
        ArgumentNullException.ThrowIfNull(baseModels);
        if (baseModels.Length == 0)
        {
            throw new ArgumentException("At least one base model must be provided.", nameof(baseModels));
        }

        foreach (var baseModel in baseModels)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(baseModel);
        }

        return WithFilter(FilterKeys.BaseModels, baseModels);
    }

    /// <summary>
    /// Set the page index for page-based pagination.
    /// </summary>
    /// <param name="pageIndex">The 1-based page index. Must be at least 1.</param>
    /// <returns>A new builder instance with the page index applied.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if pageIndex is less than 1.</exception>
    public ModelBuilder WithPageIndex(int pageIndex)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(pageIndex, 1);
        return WithFilter(FilterKeys.Page, pageIndex.ToString(CultureInfo.InvariantCulture));
    }
}
