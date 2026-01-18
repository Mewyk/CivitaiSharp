---
title: Working with Creators
description: Search for Civitai creators and retrieve profile information using the CreatorBuilder API with page-based pagination.
---

# Working with Creators

Creators are users who publish models on Civitai. The `CreatorBuilder` allows you to search for creators and retrieve their profiles.

## Querying Creators

### Search by Username

[!code-csharp[Program.cs](examples/Creators/Program.cs#SearchByUsername)]

### List All Creators

Retrieve all creators with pagination:

[!code-csharp[Program.cs](examples/Creators/Program.cs#ListAllCreators)]

## Page-Based Pagination

The creators endpoint uses traditional page-based pagination. See [Pagination](pagination.md) for comparison of pagination methods.

[!code-csharp[Program.cs](examples/Creators/Program.cs#PageBasedPagination)]

## The Creator Record

The `Creator` record contains profile information:

| Property | Type | Description |
|----------|------|-------------|
| `Username` | `string` | The creator's username |
| `ModelCount` | `int?` | Total number of published models |
| `Link` | `string?` | URL to retrieve the creator's models |
| `Image` | `string?` | Avatar image URL |

### Accessing Creator Information

[!code-csharp[Program.cs](examples/Creators/Program.cs#AccessingCreatorInformation)]

## Working with Model Creators

Models include a `Creator` property with basic profile information:

[!code-csharp[Program.cs](examples/Creators/Program.cs#WorkingWithModelCreators)]

## Common Use Cases

### List Creators

[!code-csharp[Program.cs](examples/Creators/Program.cs#FindPopularCreators)]

> **Note**: Creators are returned in the order provided by the API, not sorted by model count or popularity.

### Search for Specific Creator

[!code-csharp[Program.cs](examples/Creators/Program.cs#SearchForSpecificCreator)]

### Get All Models by Creator

[!code-csharp[Program.cs](examples/Creators/Program.cs#GetAllModelsByCreator)]

## Best Practices

**Username Matching**: The `WhereName` filter performs partial username matching. Searching for "art" will match "artist", "artworks", "art123", etc.

**Page-Based Pagination**: See [Pagination](pagination.md) for details on page-index vs cursor-based pagination.

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

[!code-csharp[Program.cs](examples/Creators/Program.cs#HandleCreatorEndpointUnreliability)]

## Next Steps

- [Working with Models](models.md) - Query models by creator
- [Working with Tags](tags.md) - Find models by tags
- [Pagination](pagination.md) - Page-based vs cursor-based pagination
