---
title: Pagination
description: Navigate large result sets with CivitaiSharp's cursor-based and page-based pagination support for efficient API traversal.
---

# Pagination

CivitaiSharp supports two types of pagination depending on the endpoint:

| Endpoint | Pagination Method | Page Size Limits |
|----------|------------------|------------------|
| **Images** | Cursor-based | 1-200 (default: 100) |
| **Models** | Page-index | 1-100 (default: 100) |
| **Tags** | Page-index | 1-200 (default: 20) |
| **Creators** | Page-index | 1-200 (default: 20) |

**Cursor-based** pagination uses `NextCursor` strings for efficient traversal. **Page-index** pagination uses `WithPageIndex(n)` for direct page access.

## Understanding Pagination

When you execute a query, the result includes pagination metadata:

[!code-csharp[Program.cs](examples/Pagination/Program.cs#PaginationMetadata)]

## Pagination Metadata

The `PaginationMetadata` record contains:

| Property | Type | Description |
|----------|------|-------------|
| `TotalItems` | `int?` | Total number of items across all pages |
| `CurrentPage` | `int?` | Current page number (1-based) |
| `PageSize` | `int?` | Number of items per page |
| `TotalPages` | `int?` | Total number of pages |
| `NextCursor` | `string?` | Cursor for the next page |
| `NextPage` | `string?` | URL for the next page |
| `PrevPage` | `string?` | URL for the previous page |

## Basic Pagination

### Setting Page Size

Use the `resultsLimit` parameter in `ExecuteAsync` to control how many items are returned per page:

[!code-csharp[Program.cs](examples/Pagination/Program.cs#PageSize)]

### Iterating Through Pages

Use cursor-based pagination to iterate through all pages:

[!code-csharp[Program.cs](examples/Pagination/Program.cs#CursorPagination)]

## Page-Based Pagination

The Models, Tags, and Creators endpoints support page index-based pagination:

[!code-csharp[Program.cs](examples/Pagination/Program.cs#PageIndex)]

## Results Per Page Limits

Each endpoint has its own limits for how many results can be returned per page:

| Endpoint | Min | Max | Default |
|----------|-----|-----|---------|
| Models | 1 | 100 | 100 |
| Images | 1 | 200 | 100 |
| Tags | 1 | 200 | 20 |
| Creators | 1 | 200 | 20 |

> **Note:** These limits control the number of items returned in a single request, not the total number of pages available.

## Async Enumeration Pattern

For a more convenient iteration pattern, you can create an extension method:

[!code-csharp[Program.cs](examples/Pagination/Program.cs#AsyncEnumerationExtension)]

## Getting the First Result

When you only need the first result, use `FirstOrDefaultAsync`:

[!code-csharp[Program.cs](examples/Pagination/Program.cs#GetFirstResult)]

This is more efficient than `ExecuteAsync` with a limit of 1 because:
- It clearly expresses intent
- It returns a single item or null, not a paged result

## Performance Considerations

1. **Choose appropriate page sizes** - Larger pages mean fewer requests but more memory usage
2. **Use cursor-based pagination** - It's more efficient for large datasets
3. **Cancel when done** - Pass a `CancellationToken` to stop early if needed
4. **Consider parallel processing** - For independent items, process pages concurrently

## Next Steps

- [Request Builders](request-builders.md) - Learn more about query building
- [Error Handling](error-handling.md) - Handle pagination errors gracefully
