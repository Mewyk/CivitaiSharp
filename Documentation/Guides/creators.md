---
title: Working with Creators
description: Search for Civitai creators and retrieve profile information using the CreatorBuilder API with page-based pagination.
---

# Working with Creators

Creators are users who publish models on Civitai. The `CreatorBuilder` allows you to search for creators and retrieve their profiles.

## Querying Creators

### Search by Username

[!code-csharp[Program.cs](Creators/Program.cs#Search-by-Username)]

### List All Creators

Retrieve all creators with pagination:

[!code-csharp[Program.cs](Creators/Program.cs#List-All-Creators)]

## Page-Based Pagination

Unlike other endpoints that use cursor-based pagination, the creators endpoint uses traditional page-based pagination:

[!code-csharp[Program.cs](Creators/Program.cs#Page-Based-Pagination)]

## The Creator Record

The `Creator` record contains profile information:

| Property | Type | Description |
|----------|------|-------------|
| `Username` | `string` | The creator's username |
| `ModelCount` | `int?` | Total number of published models |
| `Link` | `string?` | URL to retrieve the creator's models |
| `Image` | `string?` | Avatar image URL |

### Accessing Creator Information

```csharp
if (result is Result<PagedResult<Creator>>.Success success)
{
    foreach (var creator in success.Data.Items)
    {
        Console.WriteLine($"Creator: {creator.Username}");
        Console.WriteLine($"  Models: {creator.ModelCount ?? 0}");
        Console.WriteLine($"  Avatar: {creator.Image ?? "None"}");
        Console.WriteLine($"  Profile: {creator.Link ?? "None"}");
    }
}
```

## Working with Model Creators

Models include a `Creator` property with basic profile information:

```csharp
var models = await apiClient.Models
    .WhereUsername("username")
    .ExecuteAsync();

if (models is Result<PagedResult<Model>>.Success success)
{
    foreach (var model in success.Data.Items)
    {
        if (model.Creator is { } creator)
        {
            Console.WriteLine($"{model.Name} by {creator.Username}");
            Console.WriteLine($"  Creator has {creator.ModelCount ?? 0} models");
        }
    }
}
```

## Common Use Cases

### Find Popular Creators

```csharp
var topCreators = await apiClient.Creators
    .ExecuteAsync(resultsLimit: 50);

if (topCreators is Result<PagedResult<Creator>>.Success success)
{
    foreach (var creator in success.Data.Items)
    {
        Console.WriteLine($"{creator.Username}: {creator.ModelCount ?? 0} models");
    }
}
```

### Search for Specific Creator

```csharp
var result = await apiClient.Creators
    .WhereName("artist")
    .ExecuteAsync(resultsLimit: 50);
```

### Get All Models by Creator

```csharp
var creatorModels = await apiClient.Models
    .WhereUsername("specific-creator")
    .OrderBy(ModelSort.Newest)
    .ExecuteAsync(resultsLimit: 100);

if (creatorModels is Result<PagedResult<Model>>.Success success)
{
    Console.WriteLine($"Found {success.Data.Items.Count} models");
    
    foreach (var model in success.Data.Items)
    {
        Console.WriteLine($"  {model.Name}");
        Console.WriteLine($"    Downloads: {model.Stats?.DownloadCount ?? 0}");
    }
}
```

## Best Practices

**Username Matching**: The `WhereName` filter performs partial username matching. Searching for "art" will match "artist", "artworks", "art123", etc.

**Page-Based Pagination**: Creators (along with Models and Tags) use page indexes (starting from 1) instead of cursor-based pagination. Use `WithPageIndex(n)` to navigate pages. Only Images endpoint uses cursor pagination.

**Link Property**: The `Link` property provides a convenience URL, but using `Models.WhereUsername(creator.Username)` is recommended in code.

**Model Count**: The `ModelCount` may be null in some API responses. Always use null-coalescing when displaying counts.

## Endpoint Reliability Warning

> [!WARNING]
> The `/api/v1/creators` endpoint is known to experience intermittent reliability issues:
> 
> - **HTTP 500 errors**: The endpoint frequently returns server errors, especially under load
> - **Slow response times**: Requests may take significantly longer than other endpoints (10-30+ seconds)
> - **Timeout failures**: Long response times can exceed client timeout thresholds

### Recommendations

1. **Implement generous timeouts**: Set timeouts of 60-120 seconds for Creator endpoint requests
2. **Use retry logic**: The built-in resilience handler will retry on 500 errors, but success is not guaranteed
3. **Handle failures gracefully**: Your application should degrade gracefully when Creator data is unavailable
4. **Cache results aggressively**: When requests succeed, cache the results to reduce API load

```csharp
// Example: Handling Creator endpoint unreliability
var result = await client.Creators.ExecuteAsync(resultsLimit: 10);

if (!result.IsSuccess)
{
    // Log the error but continue with degraded functionality
    logger.LogWarning("Creator data unavailable: {Error}", result.ErrorInfo.Message);
    return GetCachedCreators(); // Fallback to cached data
}
```

## Next Steps

- [Working with Models](models.md) - Query models by creator
- [Working with Tags](tags.md) - Find models by tags
- [Pagination](pagination.md) - Page-based vs cursor-based pagination
