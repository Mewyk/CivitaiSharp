namespace CivitaiSharp.Core.Models.Common;

using System.Text.Json.Serialization;

/// <summary>
/// Simple pagination metadata used by paged responses.
/// </summary>
/// <param name="TotalItems">The total number of items available across all pages. Maps to JSON property "totalItems".</param>
/// <param name="CurrentPage">The current page number being returned. Maps to JSON property "currentPage".</param>
/// <param name="PageSize">The number of items per page in this response. Maps to JSON property "pageSize".</param>
/// <param name="TotalPages">The total number of pages available. Maps to JSON property "totalPages".</param>
/// <param name="NextPage">URL to retrieve the next page of results. Maps to JSON property "nextPage".</param>
/// <param name="PreviousPage">URL to retrieve the previous page of results. Maps to JSON property "prevPage".</param>
/// <param name="NextCursor">Cursor for pagination systems. Used instead of page numbers for cursor-based pagination. Maps to JSON property "nextCursor".</param>
public sealed record PaginationMetadata(
    [property: JsonPropertyName("totalItems")] int? TotalItems = null,
    [property: JsonPropertyName("currentPage")] int? CurrentPage = null,
    [property: JsonPropertyName("pageSize")] int? PageSize = null,
    [property: JsonPropertyName("totalPages")] int? TotalPages = null,
    [property: JsonPropertyName("nextPage")] string? NextPage = null,
    [property: JsonPropertyName("prevPage")] string? PreviousPage = null,
    [property: JsonPropertyName("nextCursor")] string? NextCursor = null);
