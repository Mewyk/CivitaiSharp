---
title: Working with Creators
description: Search for Civitai creators and retrieve profile information using the CreatorBuilder API with page-based pagination.
---

# Working with Creators

Creators are users who publish models on Civitai. The `client.Creators` property allows you to search for creators and retrieve their profiles.

## Querying Creators

### Search by Username

[!code-csharp[Program.cs](examples/Creators/Program.cs#SearchByUsername)]

### List All Creators

Retrieve all creators with pagination:

[!code-csharp[Program.cs](examples/Creators/Program.cs#ListAllCreators)]

## Page-Based Pagination

The creators endpoint uses traditional page-based pagination. See [Pagination](../common/pagination.md) for comparison of pagination methods.

[!code-csharp[Program.cs](examples/Creators/Program.cs#PageBasedPagination)]

## The Creator Record

The `Creator` record contains username, model count, profile link, and avatar image URL. See the API reference for complete details.

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

**Page-Based Pagination**: See [Pagination](../common/pagination.md) for details on page-index vs cursor-based pagination.

**Link Property**: The `Link` property provides a convenience URL, but using `Models.WhereUsername(creator.Username)` is recommended in code.

**Model Count**: The `ModelCount` may be null in some API responses. Always use null-coalescing when displaying counts.

## Endpoint Reliability Warning

> [!WARNING]
> The `/api/v1/creators` endpoint experiences reliability issues including HTTP 500 errors, slow response times (10-30+ seconds), and timeouts. Use generous timeouts (60-120s), implement retry logic, handle failures gracefully, and cache results when successful.

[!code-csharp[Program.cs](examples/Creators/Program.cs#HandleCreatorEndpointUnreliability)]

## Next Steps

- [Working with Models](models.md) - Query models by creator
- [Working with Tags](tags.md) - Find models by tags
- [Pagination](../common/pagination.md) - Page-based vs cursor-based pagination
