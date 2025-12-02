namespace CivitaiSharp.Core.Services;

using System.Collections.Generic;
using CivitaiSharp.Core.Models.Common;

/// <summary>
/// Represents a standard API response containing paged items with metadata.
/// This model is used internally to deserialize wrapped paged API responses.
/// </summary>
/// <remarks>
/// The Civitai API returns paged responses with "items" for the data array
/// and "metadata" for pagination information.
/// </remarks>
internal sealed record PagedApiResponse<T>(
    IReadOnlyList<T> Items,
    PaginationMetadata? Metadata = null);
