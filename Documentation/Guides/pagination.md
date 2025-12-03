---
title: Pagination
description: Navigate large result sets with CivitaiSharp's cursor-based and page-based pagination support for efficient API traversal.
---

# Pagination

CivitaiSharp supports pagination for endpoints that return multiple results. The library uses cursor-based pagination for efficient traversal of large result sets.

## Understanding Pagination

When you execute a query, the result includes pagination metadata:

```csharp
var result = await apiClient.Models
    .WhereType(ModelType.Lora)
    .ExecuteAsync(resultsLimit: 10);

if (result is Result<PagedResult<Model>>.Success success)
{
    var paged = success.Data;
    
    // The items in this page
    var models = paged.Items;
    
    // Pagination metadata
    var metadata = paged.Metadata;
}
```

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

[!code-csharp[Program.cs](Pagination/Program.cs#page-size)]

### Iterating Through Pages

Use cursor-based pagination to iterate through all pages:

[!code-csharp[Program.cs](Pagination/Program.cs#cursor-pagination)]

## Page-Based Pagination

The Models, Tags, and Creators endpoints support page index-based pagination:

[!code-csharp[Program.cs](Pagination/Program.cs#page-index)]

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

```csharp
public static class PaginationExtensions
{
    public static async IAsyncEnumerable<T> AsAsyncEnumerable<T>(
        this ModelBuilder builder,
        int pageSize = 20,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        string? cursor = null;
        
        do
        {
            var result = await builder.ExecuteAsync(pageSize, cursor, cancellationToken);
            
            if (result is not Result<PagedResult<Model>>.Success success)
            {
                yield break;
            }
            
            foreach (var item in success.Data.Items)
            {
                yield return (T)(object)item;
            }
            
            cursor = success.Data.Metadata?.NextCursor;
        }
        while (!string.IsNullOrEmpty(cursor));
    }
}

// Usage
await foreach (var model in apiClient.Models.WhereType(ModelType.Lora).AsAsyncEnumerable<Model>())
{
    Console.WriteLine(model.Name);
}
```

## Getting the First Result

When you only need the first result, use `FirstOrDefaultAsync`:

```csharp
var result = await apiClient.Models
    .WhereName("specific-model")
    .FirstOrDefaultAsync();

if (result is Result<Model?>.Success { Data: { } model })
{
    Console.WriteLine($"Found: {model.Name}");
}
```

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
